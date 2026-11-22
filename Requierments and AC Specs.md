# Warehouse & Order Fulfillment: Requirements and Acceptance Criteria

Nov 21, 2026 · @Brad

## Purpose and scope

This system exists to demonstrate Domain-Driven Design in C#: five bounded contexts, rich aggregates, domain events, and a saga that coordinates them. The business is a warehouse that takes customer orders, reserves stock, and ships. The domain is a vehicle for the design, so features are chosen for how well they exercise DDD, not for completeness.

**How to read the acceptance criteria.** Each requirement has an ID (INV-1, ORD-2, and so on) and Given / When / Then criteria. Each one is written so it can become a domain-level unit test with no database. That is the point: if a rule can be tested without infrastructure, it lives in the domain where it belongs.

**In scope**

- Five bounded contexts: Inventory and Fulfillment (core), Ordering and Catalog (supporting), Billing (generic, stubbed)
- A modular monolith: one deployable, strict module boundaries
- A REST API and a web dashboard
- Integration between contexts through events only

**Out of scope**

- Real payments, real carrier integrations, authentication and multi-tenancy
- Microservices and a real message broker (the in-process bus is designed so one could replace it)
- Returns, purchasing and supplier management, and multi-currency

**Working assumptions** (change any of these and the requirements shift)

- One company, several warehouse locations
- Stock is tracked per SKU per location
- One shipment per order (no split shipments in v1)

## Ubiquitous language

Every term below means one thing inside one context. The same word can mean something different in another context, and that is deliberate. In code, class and method names must use these words exactly.

| Term | Context | Definition |
| --- | --- | --- |
| SKU | Shared | A stock-keeping unit identifier. Other contexts hold only the SKU, never the product |
| Product | Catalog | A sellable item with name, dimensions and weight, identified by SKU |
| Stock item | Inventory | The count of one SKU at one location: on-hand and reserved quantities |
| On-hand | Inventory | Units physically present at a location |
| Reserved | Inventory | Units promised to orders but not yet shipped |
| Available | Inventory | On-hand minus reserved. This is the only number that may be promised |
| Reservation | Inventory | A time-limited claim on stock for one order line |
| Adjustment | Inventory | A manual correction to on-hand, always with a reason |
| Receipt | Inventory | Stock arriving at a location, which increases on-hand |
| Stock count | Inventory | A cycle count session that reconciles counted vs expected quantity |
| Order | Ordering | A customer's request to buy, made of order lines |
| Order line | Ordering | One SKU and a quantity within an order |
| Placed | Ordering | The point after which an order's lines can no longer change |
| Pick list | Fulfillment | Instructions for gathering reserved items from locations |
| Pick | Fulfillment | Taking units for one line from a location |
| Shipment | Fulfillment | A packed order handed to a carrier, with a tracking number |
| Allocation strategy | Inventory | The rule choosing which location supplies a reservation |

**Words to avoid.** "Item", "product" and "stock" are ambiguous. Use the terms above. "Allocate" and "reserve" are not synonyms here: reserving is the Inventory act, and the allocation strategy decides where from.

## Inventory context

Inventory is the heart of the system. Its invariant is that available stock can never go negative, and every rule below exists to protect it.

**INV-1: Receive stock**

- Given a SKU at a location with 10 on-hand, when 5 units are received, then on-hand is 15 and a `StockReceived` event is raised.
- Given a receipt of zero or negative units, when it is attempted, then it is rejected and nothing changes.
- Given a SKU that has never been stocked at a location, when units are received, then a stock item is created for it.

**INV-2: Reserve stock**

- Given 10 on-hand and 0 reserved, when 4 are reserved, then reserved is 4, available is 6, and `StockReserved` is raised.
- Given 10 on-hand and 8 reserved, when 3 are reserved, then the reservation is rejected as insufficient stock and nothing changes.
- Given available is exactly 5, when 5 are reserved, then it succeeds and available is 0.
- Given a reservation for an order line that already has one, when the same reserve request arrives again, then no second reservation is created (idempotent).

**INV-3: Release and expire reservations**

- Given an active reservation, when it is released, then reserved decreases by its quantity and `ReservationReleased` is raised.
- Given a reservation past its expiry time, when the expiry check runs, then it is released and `ReservationExpired` is raised.
- Given a released reservation, when it is released again, then nothing changes.

**INV-4: Commit a reservation (ship)**

- Given an active reservation of 4, when it is committed, then both on-hand and reserved decrease by 4.
- Given a released or expired reservation, when it is committed, then it is rejected.

**INV-5: Adjust stock**

- Given any stock item, when an adjustment is made without a reason, then it is rejected.
- Given 10 on-hand and 3 reserved, when an adjustment would reduce on-hand to 2, then it is rejected because on-hand cannot fall below reserved.
- Given a valid adjustment, when it is applied, then `StockAdjusted` is raised carrying the old quantity, new quantity and reason.

**INV-6: Allocation strategy**

- Given two locations that can both supply a line, when the strategy is nearest-warehouse, then the nearer location is chosen.
- Given no single location can supply the full line, when a reservation is requested, then it is rejected (no partial allocation in v1).
- Given the strategy is swapped for another, when the same request is made, then no change to the stock item code is needed.

**INV-7: Low-stock detection**

- Given a reorder threshold of 5 for a SKU, when available falls to 5 or below, then `LowStockDetected` is raised once, not on every later change.

**INV-8: Cycle count**

- Given a stock count session, when counted quantity differs from on-hand, then a variance is recorded and a reasoned adjustment is proposed.
- Given a completed count, when it is closed, then it cannot be modified.

**Design notes for the defense.** `StockItem` is the aggregate root and holds its reservations, because INV-2 needs on-hand, reserved and the reservation list to change in one transaction. `Quantity` and `Sku` are value objects that make invalid values unrepresentable. The allocation strategy is a domain service behind an interface.

## Ordering context

Ordering is a supporting subdomain. The model stays simple, but the `Order` aggregate still protects real rules: a state machine and a total that always matches its lines.

**ORD-1: Create and edit a draft order**

- Given a new order, when it is created, then its status is Draft and it has no lines.
- Given a Draft order, when a line is added with a valid SKU and a quantity of at least 1, then the line is added and the total updates.
- Given a line with quantity 0 or negative, when it is added, then it is rejected.
- Given a Draft order with a line for SKU A, when another line for SKU A is added, then the quantities merge into one line.

**ORD-2: Place an order**

- Given a Draft order with at least one line, when it is placed, then its status is Placed and `OrderPlaced` is raised carrying its lines.
- Given a Draft order with no lines, when it is placed, then it is rejected.
- Given a Placed order, when a line is added, changed or removed, then it is rejected.

**ORD-3: Order lifecycle**

- The valid transitions are Draft to Placed, Placed to Paid, Paid to Shipped, and Cancelled from Draft, Placed or Paid.
- Given any other transition, when it is attempted, then it is rejected and the status is unchanged.
- Given a Shipped or Cancelled order, when any change is attempted, then it is rejected (terminal states).

**ORD-4: Cancel an order**

- Given a Placed order, when it is cancelled with a reason, then status is Cancelled and `OrderCancelled` is raised.
- Given a Shipped order, when cancellation is attempted, then it is rejected.

**ORD-5: Price capture**

- Given a line being placed, when the order is placed, then the unit price is captured at that moment.
- Given a later catalog price change, when the order is read, then its captured prices are unchanged.
- Given any order, then its total always equals the sum of quantity times unit price across its lines.

**Design notes for the defense.** `Order` is the root and `OrderLine` is an entity inside it, so the total rule is enforced in one place. Lines reference products by `Sku` only, never by object, which keeps Ordering independent of Catalog. The status is a state machine inside the aggregate, not a string set from outside.

## Fulfillment, Catalog and Billing

**Fulfillment (core)**

- **FUL-1: Create a pick list.** Given a `StockReserved` event for an order, when it is handled, then a pick list is created with one pick per reserved line. Given the same event again, then no second pick list is created.
- **FUL-2: Complete picks.** Given a pick list, when every pick is marked complete, then `PickCompleted` is raised. Given a pick list with an incomplete pick, when completion is attempted, then it is rejected.
- **FUL-3: Dispatch a shipment.** Given a completed pick list, when a shipment is dispatched with a carrier and tracking number, then `ShipmentDispatched` is raised. Given a pick list that is not complete, then dispatch is rejected. Given an empty tracking number, then it is rejected.
- **FUL-4: Cancel.** Given a pick list not yet dispatched, when the order is cancelled, then the pick list is cancelled. Given a dispatched shipment, then cancellation is ignored.

**Catalog (supporting, simple CRUD by design)**

- **CAT-1: Manage products.** Given a new product, when it is created with a unique SKU, name and positive weight, then it exists. Given a duplicate SKU, then it is rejected.
- **CAT-2: Publish changes.** Given a price or dimension change, when it is saved, then `ProductChanged` is raised. Other contexts keep their own copies of what they need.
- **Why this is plain CRUD:** there are no meaningful invariants beyond validation, so adding aggregates and events here would be ceremony. This is a deliberate "when not to use DDD" example.

**Billing (generic, stubbed)**

- **BIL-1: Capture payment.** Given an `OrderPlaced` event, when it is handled, then the stub returns `PaymentCaptured` (or `PaymentFailed` for a configured test trigger such as a specific order total).
- **BIL-2: Anti-corruption layer.** Given a payment provider's response format, when it is received, then an adapter translates it into Billing's own terms. No provider type may appear in the domain model.

## Order flow across contexts (saga)

Placing an order touches Ordering, Inventory, Billing and Fulfillment, and no single transaction spans them. A saga coordinates the steps through events and undoes earlier steps when a later one fails.

&#91;embedded content: order saga · 5 steps, 2 failure paths\]

The happy path runs left to right. Stock or payment failure sends the order down to a cancel step that undoes what already happened.

**SAGA-1: Happy path**

- Given a placed order, when stock is reserved and payment is captured, then a pick list is created, and when it is picked and dispatched, then the order is Shipped and the reservation is committed.

**SAGA-2: Insufficient stock**

- Given a placed order where any line cannot be reserved, when `ReservationFailed` is received, then any reservations made for the other lines are released, the order is cancelled with reason "insufficient stock", and payment is never requested.

**SAGA-3: Payment failure (compensation)**

- Given stock is reserved and `PaymentFailed` is received, then all reservations for the order are released and the order is cancelled with reason "payment failed".

**SAGA-4: Customer cancellation**

- Given a Placed or Paid order that has not shipped, when it is cancelled, then reservations are released and any pick list is cancelled.

**SAGA-5: Reservation expiry**

- Given a reservation that expires before payment is captured, when `ReservationExpired` is received, then the order is cancelled and payment, if already captured, is flagged for refund.

**SAGA-6: Duplicate and out-of-order events**

- Given any event delivered twice, then the saga's state and side effects are the same as if delivered once.
- Given an event arriving after the saga has already ended (such as payment captured after cancellation), then it is recorded and ignored, never applied.

**SAGA-7: Visibility**

- Given any order, then the dashboard can show which saga step it is on and why it ended.

## API and dashboard

The API and dashboard are thin. They call application services and never touch domain objects directly. The dashboard reads from purpose-built read models (pragmatic CQRS), not from the aggregates.

**API requirements**

- **API-1:** Given a valid request, when `POST /orders` is called, then a Draft order is created and its ID returned.
- **API-2:** Given a Draft order, when `POST /orders/{id}/lines` and `POST /orders/{id}/place` are called, then they follow ORD-1 and ORD-2.
- **API-3:** Given a domain rule violation (such as insufficient stock), when it occurs, then the API returns 409 or 422 with a problem-details body naming the rule, never a 500.
- **API-4:** Given `POST /inventory/receipts` with SKU, location and quantity, then INV-1 applies.
- **API-5:** Given `POST /inventory/adjustments` without a reason, then it returns 422.
- **API-6:** Given any command endpoint, when the same request is retried with the same idempotency key, then the effect happens once.

**Dashboard read models**

| View | Shows | Updated by |
| --- | --- | --- |
| Stock levels | On-hand, reserved and available per SKU and location | Inventory events |
| Low-stock list | SKUs at or below threshold | `LowStockDetected` |
| Order board | Orders by status with age | Ordering and Fulfillment events |
| Shipment tracker | Dispatched shipments with tracking number | `ShipmentDispatched` |
| Activity feed | Recent domain events, newest first | All events |

- **DASH-1:** Given a stock change, when the event is processed, then the stock levels view reflects it within one refresh.
- **DASH-2:** Given the dashboard, then it contains no business rules. Any button calls an API command and shows the result.
- **DASH-3:** Given a read model is empty or stale, then rebuilding it from events must be possible.

## Non-functional and architecture requirements

These are the rules that keep the design honest. Most are enforced by automated tests, so the architecture cannot quietly erode.

- **NFR-1: Domain purity.** Given the Domain project of any context, then it references no framework, ORM, or other context. An architecture test (NetArchTest) fails the build if it does.
- **NFR-2: Context isolation.** Given any two contexts, then neither references the other's domain code. They communicate only through integration event contracts.
- **NFR-3: One aggregate per transaction.** Given any command, then it modifies exactly one aggregate. Anything else happens through events and eventual consistency.
- **NFR-4: Reliable events (outbox).** Given an aggregate change and its events, then both are saved in one transaction, and events are published afterward. A crash between the two cannot lose an event.
- **NFR-5: Idempotent handlers.** Given any event handler, when the same event is delivered twice, then the result is the same as once.
- **NFR-6: Domain tests need no infrastructure.** Given the domain test suite, then it runs without a database, network or mocks of the domain itself, and completes in seconds.
- **NFR-7: Invalid states unrepresentable.** Given a value object such as `Quantity` or `Sku`, then it cannot be constructed in an invalid state. Constructors validate.
- **NFR-8: No public setters.** Given any aggregate, then state changes only through methods named in the ubiquitous language.
- **NFR-9: Decisions recorded.** Given any significant design choice, then it is written in the decision log below with its alternatives.

## Decision log and interview defense

Each row is a decision to be able to defend out loud. The right column is the short answer. Keep this table current as decisions change.

| Decision | Chosen | Alternative | Defense in one breath |
| --- | --- | --- | --- |
| Deployment shape | Modular monolith | Microservices | Boundaries matter more than processes. A monolith keeps them cheap to change while the domain is still being learned, and modules can be split later |
| Core subdomains | Inventory and Fulfillment | Ordering as core | Stock accuracy and fulfillment flow are the hard, differentiating rules, so they get the richest model |
| Aggregate boundary | `StockItem` holds its reservations | Separate `Reservation` aggregate | Reserve must check and change on-hand, reserved and reservation list atomically. A separate aggregate would need cross-aggregate consistency |
| Cross-context contract | `Sku` reference only | Shared `Product` class | Sharing a model couples contexts. Each context keeps what it needs and the meaning of Product stays local |
| Between contexts | Events with eventual consistency | Synchronous calls | Events keep contexts independent and failures contained, at the cost of handling partial states |
| Event reliability | Outbox pattern | Publish after save | Publishing after save can lose events on a crash. The outbox makes save and publish atomic |
| Multi-step order flow | Saga (process manager) | Distributed transaction | There is no cross-context transaction. A saga makes failure paths explicit with compensating actions |
| Allocation rule | Strategy behind an interface | Hard-coded in the aggregate | The rule changes with business policy, so it is a domain service that can be swapped and tested alone |
| Reads for the dashboard | Separate read models | Query the aggregates | Aggregates are shaped for rules, not for display. Read models keep the domain model free of reporting needs |
| Catalog and Billing | Plain CRUD and a stub | Full DDD model | DDD costs effort. Spend it where the complexity is, and say so openly |
| Event dispatch | Hand-written dispatcher | MediatR | Few moving parts, no licensing questions, and it shows the mechanism rather than hiding it |
| Domain tests | Plain unit tests, no mocks | Mock-heavy tests | Rules are pure logic. If testing needs mocks, the rule is probably in the wrong layer |

**Likely follow-up questions.** Why not one big aggregate? (Contention and size.) What happens if a handler fails? (Retry, idempotency, and a dead-letter list.) How would this split into services? (Each context is already a module with event contracts.) When would you not use DDD? (Simple CRUD domains, such as Catalog here.)

## Build order and definition of done

Build in this order. Each phase has a gate. Do not start the next until the gate passes. If time runs short, stopping after phase 3 still leaves a complete, defensible demo.

1. **Language and design.** Gate: glossary agreed, context map drawn, event storm covers the happy and failure paths.
2. **Inventory domain (no database).** Gate: INV-1 to INV-8 pass as plain unit tests, and the domain project references nothing external (NFR-1, NFR-6, NFR-7, NFR-8).
3. **Ordering domain and in-process events.** Gate: ORD-1 to ORD-5 pass, and placing an order reserves stock through events only (NFR-2, NFR-3).
4. **Persistence and outbox.** Gate: aggregates save through repositories, events survive a simulated crash, handlers are idempotent (NFR-4, NFR-5).
5. **Fulfillment and the saga.** Gate: SAGA-1 to SAGA-7 pass end to end, including every failure path.
6. **Catalog and Billing stubs.** Gate: CAT-1, CAT-2, BIL-1, BIL-2 pass.
7. **API.** Gate: API-1 to API-6 pass, and rule violations return problem-details, not 500s.
8. **Dashboard and read models.** Gate: DASH-1 to DASH-3 pass, and read models rebuild from events.
9. **Defense pass.** Gate: the decision log is complete, architecture tests are green, and you can explain every row aloud without notes.

**Working with AI on the code.** For every phase, write or paste the acceptance criteria first and have the AI turn them into failing tests. Then have it write the code to pass them. Reviewing the tests is where your understanding is built and tested, and the tests are your proof that the AI's code does what the rules say.
