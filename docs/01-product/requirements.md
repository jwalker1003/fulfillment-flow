# Warehouse & Order Fulfillment: Requirements and Acceptance Criteria (v1.1)

Source: `Requierments and AC Specs.md` (v1.0, Nov 21 2026). This document is the **authoritative** version for building. It keeps every original requirement ID, gives each acceptance criterion a sub-ID (for example `INV-2.3`), and resolves the gaps found in review.

Markers:

- **(Clarified)** — the original criterion was ambiguous or contradicted another; the text here is the decision. The ADR named explains why.
- **(Added)** — a new criterion needed to make the system buildable or consistent.

Every sub-ID becomes at least one test, tagged `[Trait("Req", "INV-2.3")]`. See `docs/06-quality/test-strategy.md`.

---

## 1. Purpose and scope

This system demonstrates Domain-Driven Design in C#: five bounded contexts, rich aggregates, domain events, and a saga that coordinates them. The business is a warehouse that takes customer orders, reserves stock, and ships. Features are chosen for how well they exercise DDD, not for completeness.

**In scope**

- Five bounded contexts: Inventory and Fulfillment (core), Ordering and Catalog (supporting), Billing (generic, stubbed)
- A modular monolith: one deployable, strict module boundaries (ADR-0001)
- A REST API and a Blazor Server dashboard hosted in the same process (ADR-0019)
- Integration between contexts through events only (ADR-0005)
- A Reporting module that owns the dashboard read models (ADR-0009)

**Out of scope**

- Real payments, real carrier integrations, authentication and multi-tenancy
- Microservices and a real message broker (the in-process bus is designed so one could replace it)
- Returns, purchasing, supplier management, multi-currency (single currency: USD)
- Geocoding (shipping addresses carry coordinates directly)

**Working assumptions**

- One company, several warehouse locations, each with geographic coordinates
- Stock is tracked per SKU per location
- One shipment per order (no split shipments in v1)
- No partial allocation: each order line is reserved from exactly one location

**Technology** (ADR-0013): .NET 9, C# 13, ASP.NET Core minimal APIs, Blazor Server, EF Core 9 with SQLite, xUnit, Shouldly, NetArchTest.

---

## 2. Ubiquitous language

See `docs/02-domain/glossary.md`. That file is the single source of names; class, method and event names must use its terms exactly.

---

## 3. Inventory context

Inventory's invariant: **available stock can never go negative**, and **on-hand can never fall below reserved**.

`StockItem` (one SKU at one location) is the aggregate root and holds its reservations (ADR-0003).

### INV-1: Receive stock

- **INV-1.1** Given a SKU at a location with 10 on-hand, when 5 units are received, then on-hand is 15 and `StockReceived` is raised.
- **INV-1.2** Given a receipt of zero or negative units, when it is attempted, then it is rejected (`inventory.quantity-must-be-positive`) and nothing changes.
- **INV-1.3** Given a SKU that has never been stocked at a location, when units are received, then a stock item is created for it with that on-hand and 0 reserved.
- **INV-1.4 (Added)** Given a receipt for a location that does not exist, when it is attempted, then it is rejected (`inventory.unknown-location`).

### INV-2: Reserve stock

- **INV-2.1** Given 10 on-hand and 0 reserved, when 4 are reserved, then reserved is 4, available is 6, and `StockReserved` is raised.
- **INV-2.2** Given 10 on-hand and 8 reserved, when 3 are reserved, then the reservation is rejected (`inventory.insufficient-stock`) and nothing changes.
- **INV-2.3** Given available is exactly 5, when 5 are reserved, then it succeeds and available is 0.
- **INV-2.4** Given a stock item that already holds a reservation (in any status) for an order line, when the same reserve request arrives again, then no second reservation is created, no event is raised, and the existing reservation is returned (idempotent).
- **INV-2.5 (Added)** Given a reservation is created at time `now`, then its expiry time is `now + reservation lifetime` (default 15 minutes, configured in the application layer, ADR-0017).
- **INV-2.6 (Clarified, ADR-0015)** Given an `OrderPlaced` integration event, when Inventory handles it, then for each line in order it asks the allocation strategy for a location and reserves on that stock item, one stock item per transaction. If any line cannot be reserved, Inventory raises `ReservationFailed` for that line and stops processing the remaining lines. `ReservationFailed` is raised by the application service, not by the aggregate.

### INV-3: Release, expire and confirm reservations

- **INV-3.1** Given an active reservation, when it is released, then reserved decreases by its quantity and `ReservationReleased` is raised.
- **INV-3.2** Given an active, unconfirmed reservation past its expiry time, when the expiry check runs, then it is released and `ReservationExpired` is raised.
- **INV-3.3** Given a released, expired or committed reservation, when it is released again, then nothing changes and no event is raised.
- **INV-3.4 (Added, ADR-0017)** Given an active reservation, when it is confirmed (because payment was captured and fulfillment requested), then it no longer expires; a later expiry check leaves it active.
- **INV-3.5 (Added)** Given an `OrderCancelled` integration event, when Inventory handles it, then every active reservation for that order, across all stock items, is released (one stock item per transaction).

### INV-4: Commit a reservation (ship)

- **INV-4.1** Given an active reservation of 4, when it is committed, then both on-hand and reserved decrease by 4 and `ReservationCommitted` is raised.
- **INV-4.2** Given a released or expired reservation, when it is committed, then it is rejected (`inventory.reservation-not-active`).
- **INV-4.3 (Added)** Given a committed reservation, when it is committed again, then nothing changes and no event is raised (idempotent, NFR-5).
- **INV-4.4 (Added)** Given a `ShipmentDispatched` integration event, when Inventory handles it, then each reservation it lists is committed.

### INV-5: Adjust stock

- **INV-5.1** Given any stock item, when an adjustment is made without a reason (null, empty or whitespace), then it is rejected (`inventory.adjustment-reason-required`).
- **INV-5.2** Given 10 on-hand and 3 reserved, when an adjustment would reduce on-hand to 2, then it is rejected (`inventory.on-hand-below-reserved`).
- **INV-5.3** Given a valid adjustment, when it is applied, then `StockAdjusted` is raised carrying the old on-hand, new on-hand and reason.
- **INV-5.4 (Clarified)** An adjustment is expressed as a signed quantity change (non-zero). A change of zero is rejected (`inventory.adjustment-must-change-quantity`). On-hand can never become negative.

### INV-6: Allocation strategy

- **INV-6.1** Given two locations that can both supply a line, when the strategy is nearest-warehouse, then the location nearest to the order's shipping coordinates (great-circle distance) is chosen.
- **INV-6.2** Given no single location can supply the full line, when a reservation is requested, then it is rejected (`inventory.no-location-can-supply`); no partial allocation.
- **INV-6.3** Given the strategy is swapped for another (`MostAvailableAllocationStrategy`), when the same request is made, then no change to `StockItem` code is needed.
- **INV-6.4 (Added)** Given two candidate locations at equal distance, then the one with the lower `LocationId` (ordinal string comparison) is chosen, so allocation is deterministic.
- **INV-6.5 (Added)** Given a location with no stock item for the SKU, then it is not a candidate.

### INV-7: Low-stock detection

- **INV-7.1** Given a reorder threshold of 5 on a stock item, when available falls to 5 or below, then `LowStockDetected` is raised once, not on every later change.
- **INV-7.2 (Added)** Given low stock was detected, when available rises above the threshold, then detection re-arms, and a later fall to or below the threshold raises `LowStockDetected` again.
- **INV-7.3 (Clarified)** The threshold belongs to the stock item (SKU at a location). A stock item with no threshold never raises `LowStockDetected`. Setting a threshold at or above current available raises `LowStockDetected` immediately.

### INV-8: Cycle count

`StockCount` is its own aggregate: one count session at one location.

- **INV-8.1** Given a stock count session, when a counted quantity is recorded that differs from the expected quantity (on-hand captured when the line was recorded), then a variance is recorded and a reasoned adjustment is proposed (quantity change = counted − expected, reason "Cycle count {countId}").
- **INV-8.2** Given a closed count, when any change is attempted, then it is rejected (`inventory.stock-count-closed`).
- **INV-8.3 (Clarified)** Closing the count is the approval. Closing raises `StockCountClosed` carrying the proposed adjustments; Inventory then applies each as an adjustment on its stock item (one per transaction). An adjustment rejected by INV-5.2 is recorded in the activity feed and not retried.
- **INV-8.4 (Added)** Given a line whose counted quantity equals expected, then no adjustment is proposed for it.
- **INV-8.5 (Added)** Given a count with no recorded lines, when it is closed, then it is rejected (`inventory.stock-count-empty`).

---

## 4. Ordering context

`Order` is the root; `OrderLine` is an entity inside it. Lines reference products by `Sku` only. Ordering keeps its own copy of SKU, price and active flag, fed by `ProductChanged` (ADR-0004).

### ORD-1: Create and edit a draft order

- **ORD-1.1** Given a new order, when it is created with a customer reference, then its status is Draft and it has no lines.
- **ORD-1.2** Given a Draft order, when a line is added with a valid SKU and a quantity of at least 1, then the line is added at the SKU's current price and the total updates.
- **ORD-1.3** Given a line with quantity 0 or negative, when it is added, then it is rejected (`ordering.quantity-must-be-positive`).
- **ORD-1.4** Given a Draft order with a line for SKU A, when another line for SKU A is added, then the quantities merge into one line.
- **ORD-1.5 (Clarified)** A SKU is valid when it exists in Ordering's product copy and is active. Otherwise adding is rejected (`ordering.unknown-sku`).
- **ORD-1.6 (Added)** Given a Draft order, when a line's quantity is changed (to at least 1) or a line is removed, then the total updates.
- **ORD-1.7 (Added)** Given a Draft order, when a shipping address is set, then it is stored. A shipping address has line 1, city, postal code, country code and coordinates (latitude −90..90, longitude −180..180).

### ORD-2: Place an order

- **ORD-2.1** Given a Draft order with at least one line and a shipping address, when it is placed, then its status is Placed and `OrderPlaced` is raised carrying its lines, captured prices, total and shipping coordinates.
- **ORD-2.2** Given a Draft order with no lines, when it is placed, then it is rejected (`ordering.order-has-no-lines`).
- **ORD-2.3** Given a Placed order, when a line is added, changed or removed, then it is rejected (`ordering.order-not-draft`).
- **ORD-2.4 (Added)** Given a Draft order without a shipping address, when it is placed, then it is rejected (`ordering.shipping-address-required`).

### ORD-3: Order lifecycle

- **ORD-3.1** The valid transitions are Draft→Placed, Placed→Paid, Paid→Shipped, and →Cancelled from Draft, Placed or Paid.
- **ORD-3.2** Given any other transition, when it is attempted, then it is rejected (`ordering.invalid-status-transition`) and the status is unchanged.
- **ORD-3.3** Given a Shipped or Cancelled order, when any change is attempted, then it is rejected (terminal states).
- **ORD-3.4 (Added)** Placed→Paid raises `OrderPaid`; Paid→Shipped raises `OrderShipped`.

### ORD-4: Cancel an order

- **ORD-4.1** Given a Placed order, when it is cancelled with a reason, then status is Cancelled and `OrderCancelled` is raised carrying the reason and the previous status.
- **ORD-4.2** Given a Shipped order, when cancellation is attempted, then it is rejected.
- **ORD-4.3 (Added)** Given a cancellation without a reason, then it is rejected (`ordering.cancellation-reason-required`).

### ORD-5: Price capture

- **ORD-5.1** Given an order being placed, when it is placed, then each line's unit price is re-read from Ordering's product copy and captured at that moment.
- **ORD-5.2** Given a later catalog price change, when the order is read, then its captured prices are unchanged.
- **ORD-5.3** Given any order, then its total always equals the sum of quantity × unit price across its lines.
- **ORD-5.4 (Added)** Given a line whose SKU became inactive or unknown before placing, when the order is placed, then placing is rejected (`ordering.unknown-sku`).

---

## 5. Fulfillment context (core)

`PickList` is the aggregate root; `Pick` and `Shipment` are entities inside it (one shipment per order).

- **FUL-1.1 (Clarified, ADR-0016)** Given a `FulfillmentRequested` event for an order (published by the saga once stock is reserved and payment captured), when it is handled, then a pick list is created with one pick per reserved line (SKU, location, quantity, reservation ID) and `PickListCreated` is raised.
- **FUL-1.2** Given the same event again, then no second pick list is created.
- **FUL-2.1 (Clarified)** Given a pick list, when every pick is marked complete and the pick list is completed, then `PickListCompleted` is raised. (The original name `PickCompleted` is renamed because a *pick* is one line; the event is about the whole list.)
- **FUL-2.2** Given a pick list with an incomplete pick, when completion is attempted, then it is rejected (`fulfillment.picks-incomplete`).
- **FUL-2.3 (Added)** Given a pick already complete, when it is completed again, then nothing changes.
- **FUL-3.1** Given a completed pick list, when a shipment is dispatched with a carrier and tracking number, then `ShipmentDispatched` is raised carrying the order ID, carrier, tracking number and reservation IDs.
- **FUL-3.2** Given a pick list that is not complete, then dispatch is rejected (`fulfillment.pick-list-not-completed`).
- **FUL-3.3** Given an empty carrier or tracking number, then dispatch is rejected (`fulfillment.tracking-number-required` / `fulfillment.carrier-required`).
- **FUL-4.1** Given a pick list not yet dispatched, when `OrderCancelled` is handled, then the pick list is cancelled and `PickListCancelled` is raised.
- **FUL-4.2** Given a dispatched shipment, then cancellation is ignored.

---

## 6. Catalog context (supporting, plain CRUD by design)

There are no meaningful invariants beyond validation, so Catalog uses no aggregates (ADR-0010). It still publishes `ProductChanged` through the outbox.

- **CAT-1.1** Given a new product, when it is created with a unique SKU, name, positive unit price and positive weight, then it exists.
- **CAT-1.2** Given a duplicate SKU, then creation is rejected (`catalog.duplicate-sku`).
- **CAT-1.3 (Added)** Given a non-positive price, weight or dimension, or an empty name, then it is rejected (`catalog.invalid-product`).
- **CAT-2.1 (Clarified)** Given a product is created or its price, name, dimensions, weight or active flag changes, when it is saved, then `ProductChanged` is raised carrying the full product snapshot and a version number. Consumers keep their own copies.
- **CAT-2.2 (Added)** Given `ProductChanged` arrives with a version lower than or equal to the copy Ordering holds, then it is ignored.

---

## 7. Billing context (generic, stubbed)

- **BIL-1.1 (Clarified, ADR-0015)** Given a `PaymentRequested` event (published by the saga after all lines are reserved), when it is handled, then the stub gateway is charged and `PaymentCaptured` is raised. Billing does **not** react to `OrderPlaced`, so payment is never requested when stock fails (SAGA-2).
- **BIL-1.2** Given the order amount equals the configured failure trigger (`Billing:FailWhenAmountEquals`, default 13.13), then `PaymentFailed` is raised instead.
- **BIL-1.3 (Added)** Given the same `PaymentRequested` twice, then the gateway is charged once.
- **BIL-2.1** Given a payment provider's response format, when it is received, then an adapter translates it into Billing's own terms. No provider type appears in Billing's domain or application layers (architecture test).
- **BIL-3.1 (Added)** Given a `RefundRequested` event for a captured payment, when it is handled, then the payment is flagged for refund and `RefundFlagged` is raised. No money moves (stub).
- **BIL-3.2 (Added)** Given a `RefundRequested` for an order with no captured payment, then it is recorded and ignored.

---

## 8. Order flow across contexts (saga)

The saga (`OrderFulfillmentProcess`) is a process manager owned by Ordering (ADR-0007). It reacts to integration events, records its state, publishes integration events for other contexts, and sends internal commands to Ordering. Full state machine: `docs/02-domain/saga.md`.

- **SAGA-1.1** Given a placed order, when `StockReserved` has been received for every line, then the saga publishes `PaymentRequested`.
- **SAGA-1.2** When `PaymentCaptured` is received, then the order is marked Paid and the saga publishes `FulfillmentRequested` with one pick per reservation.
- **SAGA-1.3** When `ShipmentDispatched` is received, then the order is marked Shipped, Inventory commits the reservations, and the saga completes.
- **SAGA-2.1** Given a placed order where any line cannot be reserved, when `ReservationFailed` is received, then the order is cancelled with reason "insufficient stock", reservations already made are released (Inventory reacting to `OrderCancelled`), and `PaymentRequested` is never published.
- **SAGA-3.1** Given stock is reserved and `PaymentFailed` is received, then the order is cancelled with reason "payment failed" and all reservations for the order are released.
- **SAGA-4.1** Given a Placed or Paid order that has not shipped, when the customer cancels it, then reservations are released and any pick list is cancelled.
- **SAGA-4.2 (Added)** Given a Paid order is cancelled, then the saga publishes `RefundRequested`.
- **SAGA-5.1** Given a reservation expires before the order ships, when `ReservationExpired` is received, then the order is cancelled with reason "reservation expired", and if payment was already captured, `RefundRequested` is published.
- **SAGA-6.1** Given any event delivered twice, then the saga's state and side effects are the same as if delivered once.
- **SAGA-6.2** Given an event arriving after the saga has ended (such as `StockReserved` or `PaymentFailed` after cancellation), then it is recorded in the saga history and ignored.
- **SAGA-6.3 (Clarified)** Exception to 6.2: `PaymentCaptured` arriving after cancellation publishes `RefundRequested` (money was taken and must be flagged back). A late `StockReserved` after cancellation is left for reservation expiry to clean up.
- **SAGA-7.1** Given any order, then the dashboard can show which saga step it is on and why it ended (via `OrderFulfillmentStepChanged` and the saga history).

---

## 9. API and dashboard

The API and dashboard are thin: they call application services and never touch domain objects. Full contract: `docs/04-contracts/openapi.yaml`. Errors: `docs/04-contracts/error-catalog.md`.

- **API-1** Given a valid request, when `POST /orders` is called, then a Draft order is created and `201` with its ID is returned.
- **API-2** Given a Draft order, when `POST /orders/{id}/lines`, `PUT /orders/{id}/shipping-address` and `POST /orders/{id}/place` are called, then they follow ORD-1 and ORD-2.
- **API-3** Given a domain rule violation, when it occurs, then the API returns `409` (state conflict) or `422` (invalid input) with an RFC 9457 problem-details body naming the rule (`code`, `requirement`), never a `500`. Unknown resources return `404`.
- **API-4** Given `POST /inventory/receipts` with SKU, location and quantity, then INV-1 applies.
- **API-5** Given `POST /inventory/adjustments` without a reason, then it returns `422`.
- **API-6** Given any `POST`/`PUT`/`DELETE` command endpoint, when the same request is retried with the same `Idempotency-Key` header, then the effect happens once and the original response is replayed. The same key with a different body returns `422` (`api.idempotency-key-reused`).
- **API-7 (Added)** Read-model queries are exposed under `GET /views/...` and admin operations (rebuild read models, list dead letters) under `/admin/...`.

**Dashboard read models** (Reporting module)

| View | Shows | Updated by |
| --- | --- | --- |
| Stock levels | On-hand, reserved and available per SKU and location | Inventory events |
| Low-stock list | Stock items at or below threshold | `LowStockDetected`, Inventory events (removed when re-armed) |
| Order board | Orders by status with age | Ordering events |
| Shipment tracker | Dispatched shipments with carrier and tracking number | `ShipmentDispatched` |
| Activity feed | Recent integration events, newest first (last 200) | All events |
| Order process | Current saga step and end reason per order | `OrderFulfillmentStepChanged` |

- **DASH-1** Given a stock change, when the event is processed, then the stock levels view reflects it within one refresh (dashboard polls every 2 s).
- **DASH-2** Given the dashboard, then it contains no business rules. Every button calls an API command over HTTP and shows the result (including problem-details errors).
- **DASH-3** Given a read model is empty or stale, then rebuilding it from the event log (retained outbox messages, ADR-0006) is possible via `POST /admin/read-models/rebuild` and yields the same state.

---

## 10. Non-functional and architecture requirements

- **NFR-1 Domain purity.** A context's Domain project references only `FulfillmentFlow.SharedKernel` and the BCL: no framework, ORM, or other context. Enforced by an architecture test.
- **NFR-2 Context isolation.** No context references another context's Domain, Application or Infrastructure. They communicate only through `*.Contracts` integration events. Enforced by an architecture test.
- **NFR-3 One aggregate per transaction.** Every command or handler transaction modifies exactly one aggregate (the saga state counts as one) plus outbox/inbox rows. Anything else happens through events.
- **NFR-4 Reliable events (outbox).** Aggregate changes and their outgoing messages are saved in one transaction; messages are published afterward. A crash between the two cannot lose a message.
- **NFR-5 Idempotent handlers.** Every handler produces the same result when a message is delivered twice (inbox table and/or natural-key checks, ADR-0018).
- **NFR-6 Domain tests need no infrastructure.** Domain test projects reference no EF Core, SQLite, ASP.NET or mocking library, and the full domain suite runs in under 10 seconds.
- **NFR-7 Invalid states unrepresentable.** Value objects (`Sku`, `Quantity`, `Money`, `GeoPoint`, IDs) validate in their factory and cannot be constructed invalid.
- **NFR-8 No public setters.** Aggregates and entities expose no public setters; state changes only through methods named in the ubiquitous language. Enforced by an architecture test.
- **NFR-9 Decisions recorded.** Every significant design choice has an ADR in `docs/03-architecture/adr/`.
- **NFR-10 (Added) Optimistic concurrency.** Every aggregate has a `Version` concurrency token. A conflicting write fails; handlers retry (via outbox retry), the API returns `409` (`api.concurrency-conflict`).
- **NFR-11 (Added) Failed message handling.** A message whose handler fails is retried up to 5 times with backoff, then marked dead-lettered and listed at `GET /admin/dead-letters`.

---

## 11. Build order and definition of done

See `docs/05-delivery/roadmap.md` for phases, gates and stories, and `docs/06-quality/definition-of-done.md`. Stopping after phase 3 still leaves a complete, defensible demo.
