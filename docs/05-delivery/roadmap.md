# Roadmap

Build in phase order. **Do not start a phase until the previous gate passes.** Within a phase, stories run in the order listed unless "Parallel with" says otherwise. Stopping after phase 3 still leaves a complete, defensible demo.

Each story file in `stories/` is one unit of work for the software factory (one branch, one PR). Every story follows the workflow in `CLAUDE.md`: write failing tests from the acceptance criteria → make them pass → run the full suite → update `traceability.md`.

## Phase 1 — Foundation (language & design are done by these docs)

Gate: solution builds with zero warnings; SharedKernel tests pass; architecture tests run (and pass on the empty modules).

| Story | Title | Depends on |
| --- | --- | --- |
| [P1-01](stories/P1-01-solution-scaffold.md) | Solution scaffold | — |
| [P1-02](stories/P1-02-shared-kernel.md) | Shared kernel primitives | P1-01 |
| [P1-03](stories/P1-03-architecture-tests.md) | Architecture tests | P1-01 |

## Phase 2 — Inventory domain (no database)

Gate: INV-1 … INV-8 pass as plain unit tests; NFR-1, NFR-6, NFR-7, NFR-8 architecture tests green.

| Story | Title | Depends on |
| --- | --- | --- |
| [P2-01](stories/P2-01-stockitem-receive.md) | `StockItem` and receive | P1-02 |
| [P2-02](stories/P2-02-reserve-stock.md) | Reserve stock | P2-01 |
| [P2-03](stories/P2-03-release-expire-confirm.md) | Release, expire, confirm | P2-02 |
| [P2-04](stories/P2-04-commit-reservation.md) | Commit a reservation | P2-02 |
| [P2-05](stories/P2-05-adjust-stock.md) | Adjust stock | P2-01 |
| [P2-06](stories/P2-06-allocation-strategy.md) | Allocation strategies | P2-01 |
| [P2-07](stories/P2-07-low-stock.md) | Low-stock detection | P2-02, P2-05 |
| [P2-08](stories/P2-08-cycle-count.md) | Cycle count | P2-05 |

## Phase 3 — Ordering domain and in-process events

Gate: ORD-1 … ORD-5 pass; placing an order reserves stock through events only, in memory (NFR-2, NFR-3).

| Story | Title | Depends on |
| --- | --- | --- |
| [P3-01](stories/P3-01-order-draft.md) | Draft order | P1-02 |
| [P3-02](stories/P3-02-place-order.md) | Place order and price capture | P3-01 |
| [P3-03](stories/P3-03-order-lifecycle-cancel.md) | Lifecycle and cancel | P3-02 |
| [P3-04](stories/P3-04-application-building-blocks.md) | Application building blocks and in-memory bus | P1-02 |
| [P3-05](stories/P3-05-reserve-on-order-placed.md) | Inventory reserves on `OrderPlaced` (in memory) | P2-06, P3-02, P3-04 |

## Phase 4 — Persistence and outbox

Gate: aggregates save through repositories to SQLite; messages survive a simulated crash; handlers are idempotent (NFR-4, NFR-5, NFR-10, NFR-11).

| Story | Title | Depends on |
| --- | --- | --- |
| [P4-01](stories/P4-01-sqlite-persistence.md) | SQLite persistence for Inventory and Ordering | P3-05 |
| [P4-02](stories/P4-02-outbox.md) | Outbox write and processor | P4-01 |
| [P4-03](stories/P4-03-inbox-idempotency.md) | Inbox idempotency, retry, dead letters | P4-02 |
| [P4-04](stories/P4-04-crash-and-concurrency-tests.md) | Crash and concurrency tests | P4-03 |
| [P4-05](stories/P4-05-reservation-expiry-job.md) | Reservation expiry job | P4-02, P2-03 |

## Phase 5 — Fulfillment and the saga

Gate: SAGA-1 … SAGA-7 pass end to end (integration tests with SQLite and drained outboxes), including every failure path. Billing events are published by the tests until phase 6.

| Story | Title | Depends on |
| --- | --- | --- |
| [P5-01](stories/P5-01-picklist-domain.md) | `PickList` domain | P1-02 |
| [P5-02](stories/P5-02-fulfillment-application.md) | Fulfillment application and persistence | P5-01, P4-03 |
| [P5-03](stories/P5-03-saga-domain.md) | `OrderFulfillmentProcess` state machine | P3-03 |
| [P5-04](stories/P5-04-saga-wiring.md) | Saga handler, internal commands, persistence | P5-03, P4-03 |
| [P5-05](stories/P5-05-inventory-saga-reactions.md) | Inventory reactions: release, confirm, commit | P4-03 |
| [P5-06](stories/P5-06-saga-end-to-end.md) | Saga end-to-end tests | P5-02, P5-04, P5-05 |

## Phase 6 — Catalog and Billing

Gate: CAT-1, CAT-2, BIL-1 … BIL-3 pass; saga end-to-end tests now run with real Billing.

| Story | Title | Depends on |
| --- | --- | --- |
| [P6-01](stories/P6-01-catalog-crud.md) | Catalog CRUD and `ProductChanged` | P4-02 |
| [P6-02](stories/P6-02-ordering-product-copy.md) | Ordering product copy | P6-01 |
| [P6-03](stories/P6-03-billing-stub-acl.md) | Billing stub and ACL | P4-03 |

## Phase 7 — API

Gate: API-1 … API-7 pass; rule violations return problem details, never 500.

| Story | Title | Depends on |
| --- | --- | --- |
| [P7-01](stories/P7-01-api-host-problem-details.md) | API host, error catalog, problem details | P6-03 |
| [P7-02](stories/P7-02-ordering-endpoints.md) | Ordering endpoints | P7-01 |
| [P7-03](stories/P7-03-inventory-endpoints.md) | Inventory endpoints | P7-01 |
| [P7-04](stories/P7-04-fulfillment-catalog-endpoints.md) | Fulfillment and Catalog endpoints | P7-01 |
| [P7-05](stories/P7-05-idempotency-key.md) | `Idempotency-Key` | P7-02 |

## Phase 8 — Read models and dashboard

Gate: DASH-1 … DASH-3 pass; read models rebuild from the log; SAGA-7 visible.

| Story | Title | Depends on |
| --- | --- | --- |
| [P8-01](stories/P8-01-reporting-projections.md) | Reporting projections, views API, rebuild | P7-01 |
| [P8-02](stories/P8-02-dashboard-shell-stock.md) | Dashboard shell, stock views | P8-01 |
| [P8-03](stories/P8-03-dashboard-orders-shipments.md) | Order board, process view, shipments, activity | P8-02 |
| [P8-04](stories/P8-04-seed-and-demo.md) | Seed data and demo script | P8-03 |

## Phase 9 — Defense pass

Gate: every ADR complete; architecture tests green; traceability has a test for every sub-ID; Brad can explain every ADR aloud.

| Story | Title | Depends on |
| --- | --- | --- |
| [P9-01](stories/P9-01-defense-pass.md) | Defense pass | all |
