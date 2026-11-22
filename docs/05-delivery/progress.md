# Progress

The single record of where the build stands. `/run-phase` and `/implement-story` read and update this file; Brad signs off gates here.

**Statuses:** `todo` · `in-progress` · `review` · `done` · `blocked`
**Rules for the factory**
- Pick the first `todo` story in the current phase whose dependencies (see `roadmap.md`) are all `done`.
- Set `in-progress` when starting, `review` while the reviewer runs, `done` with the implementation commit's short hash when finished.
- `blocked` needs a reason in Notes, and stops the phase.
- Never change a **Gate** line except to write the gate report. Only Brad marks a gate `✅ signed off`.

---

## Phase 1 — Foundation
**Gate:** ⏳ not reached
| Story | Title | Status | Commit | Notes |
| --- | --- | --- | --- | --- |
| P1-01 | Solution scaffold | done | fb1bdef | 1 review round. Open question before P6-03: should Billing.Tests reference Billing.Infrastructure to test the FakePay translator? That conflicts with NFR-6 (no EF Core in domain test projects). Microsoft packages pinned to 9.0.14 to match the installed runtime. ADR-tagged tests are listed in a new "Architecture decisions" table in traceability.md. |
| P1-02 | Shared kernel primitives | done | 172805a | 1 review round. Brad chose to keep the public `SharedErrors` class (the shared.invalid-* codes), which ADR-0004 does not list; ADR-0004 needs amending at the gate. Doc conflict: the story says no `ValueObject` base, but architecture.md:15 and context-map.md:52 list one. `AggregateRoot.IncrementVersion()` is internal, with InternalsVisibleTo for the tests only, so P4-01 must expose it to BuildingBlocks.Infrastructure or set Version another way. ADR-0014 still has status Proposed. SharedKernel.Tests is not in architecture.md layout or RepositoryLayout.ExpectedProjects. |
| P1-03 | Architecture tests | todo | | |

## Phase 2 — Inventory domain
**Gate:** ⏳ not reached
| Story | Title | Status | Commit | Notes |
| --- | --- | --- | --- | --- |
| P2-01 | `StockItem` and receive | todo | | |
| P2-02 | Reserve stock | todo | | |
| P2-03 | Release, expire, confirm | todo | | |
| P2-04 | Commit a reservation | todo | | |
| P2-05 | Adjust stock | todo | | |
| P2-06 | Allocation strategies | todo | | |
| P2-07 | Low-stock detection | todo | | |
| P2-08 | Cycle count | todo | | |

## Phase 3 — Ordering domain and in-process events
**Gate:** ⏳ not reached
| Story | Title | Status | Commit | Notes |
| --- | --- | --- | --- | --- |
| P3-01 | Draft order | todo | | |
| P3-02 | Place order and price capture | todo | | |
| P3-03 | Lifecycle and cancel | todo | | |
| P3-04 | Application building blocks and in-memory bus | todo | | |
| P3-05 | Inventory reserves on `OrderPlaced` | todo | | |

## Phase 4 — Persistence and outbox
**Gate:** ⏳ not reached
| Story | Title | Status | Commit | Notes |
| --- | --- | --- | --- | --- |
| P4-01 | SQLite persistence | todo | | |
| P4-02 | Outbox write and processor | todo | | |
| P4-03 | Inbox idempotency, retry, dead letters | todo | | |
| P4-04 | Crash and concurrency tests | todo | | |
| P4-05 | Reservation expiry job | todo | | |

## Phase 5 — Fulfillment and the saga
**Gate:** ⏳ not reached
| Story | Title | Status | Commit | Notes |
| --- | --- | --- | --- | --- |
| P5-01 | `PickList` domain | todo | | |
| P5-02 | Fulfillment application and persistence | todo | | |
| P5-03 | `OrderFulfillmentProcess` state machine | todo | | |
| P5-04 | Saga handler, internal commands, persistence | todo | | |
| P5-05 | Inventory reactions: release, confirm, commit | todo | | |
| P5-06 | Saga end-to-end tests | todo | | |

## Phase 6 — Catalog and Billing
**Gate:** ⏳ not reached
| Story | Title | Status | Commit | Notes |
| --- | --- | --- | --- | --- |
| P6-01 | Catalog CRUD and `ProductChanged` | todo | | |
| P6-02 | Ordering product copy | todo | | |
| P6-03 | Billing stub and ACL | todo | | |

## Phase 7 — API
**Gate:** ⏳ not reached
| Story | Title | Status | Commit | Notes |
| --- | --- | --- | --- | --- |
| P7-01 | API host, error catalog, problem details | todo | | |
| P7-02 | Ordering endpoints | todo | | |
| P7-03 | Inventory endpoints | todo | | |
| P7-04 | Fulfillment and Catalog endpoints | todo | | |
| P7-05 | `Idempotency-Key` | todo | | |

## Phase 8 — Read models and dashboard
**Gate:** ⏳ not reached
| Story | Title | Status | Commit | Notes |
| --- | --- | --- | --- | --- |
| P8-01 | Reporting projections, views API, rebuild | todo | | |
| P8-02 | Dashboard shell, stock views | todo | | |
| P8-03 | Order board, process view, shipments, activity | todo | | |
| P8-04 | Seed data and demo script | todo | | |

## Phase 9 — Defense pass
**Gate:** ⏳ not reached
| Story | Title | Status | Commit | Notes |
| --- | --- | --- | --- | --- |
| P9-01 | Defense pass | todo | | |
