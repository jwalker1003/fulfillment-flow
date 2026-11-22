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
**Gate:** 🟡 awaiting Brad's review — 2026-11-22
- Build: ✅ zero warnings · Tests: 207/207 in ~4 s (8 test projects; 117 architecture, 84 SharedKernel, 6 placeholder/discovery)
- Gate criteria:
  - Solution builds with zero warnings → ✅ `dotnet build -warnaserror --no-incremental`: 0 warnings, 0 errors
  - SharedKernel tests pass → ✅ FulfillmentFlow.SharedKernel.Tests 84/84 (NFR-7, NFR-8, ADR-0004, ADR-0014)
  - Architecture tests run and pass on the empty modules → ✅ FulfillmentFlow.ArchitectureTests 117/117 (NFR-1, NFR-2, NFR-6, NFR-8, BIL-2.1, ADR-0001, ADR-0005, ADR-0013, ADR-0020)
  - Traceability → ✅ all 207 tests carry a `Req` trait, and every tagged ID has a row in traceability.md
- Deviations and open questions:
  1. **ADR-0004 amendment needed:** public `SharedErrors` (shared.invalid-* codes) was added to SharedKernel. Brad chose to keep it; ADR-0004 must list it.
  2. **ADR-0014 still has status Proposed**, but P1-02 implements it as binding. Mark it Accepted?
  3. **Doc conflict:** the P1-02 story says no `ValueObject` base, but architecture.md:15 and context-map.md:52 list one. The code follows the story; update the two docs.
  4. **`AggregateRoot.IncrementVersion()` is internal** and visible to SharedKernel.Tests only. P4-01 must expose it to BuildingBlocks.Infrastructure or set Version another way.
  5. **Billing.Tests vs FakePay translator:** if Billing.Tests references Billing.Infrastructure, NFR-6 fails. Decide before P6-03.
  6. **architecture.md layout** doesn't list tests/FulfillmentFlow.SharedKernel.Tests (the ArchitectureTests expected list now does).
  7. **Wording:** story P1-03 task 7 says "ends with IntegrationEvent"; the rule also matches `IntegrationEventV<n>` (integration-events.md rule 4).
  8. Rules without an NFR ID are tagged with ADR IDs (ADR-0001, -0004, -0005, -0013, -0014, -0020), in a new "Architecture decisions" table in traceability.md.
  9. Microsoft packages are pinned to 9.0.14 to match the installed runtime.
  10. Each story took 1 review fix round; no story was blocked.
- Review: `git diff main...phase/1-foundation`

| Story | Title | Status | Commit | Notes |
| --- | --- | --- | --- | --- |
| P1-01 | Solution scaffold | done | fb1bdef | 1 review round. Open question before P6-03: should Billing.Tests reference Billing.Infrastructure to test the FakePay translator? That conflicts with NFR-6 (no EF Core in domain test projects). Microsoft packages pinned to 9.0.14 to match the installed runtime. ADR-tagged tests are listed in a new "Architecture decisions" table in traceability.md. |
| P1-02 | Shared kernel primitives | done | 172805a | 1 review round. Brad chose to keep the public `SharedErrors` class (the shared.invalid-* codes), which ADR-0004 does not list; ADR-0004 needs amending at the gate. Doc conflict: the story says no `ValueObject` base, but architecture.md:15 and context-map.md:52 list one. `AggregateRoot.IncrementVersion()` is internal, with InternalsVisibleTo for the tests only, so P4-01 must expose it to BuildingBlocks.Infrastructure or set Version another way. ADR-0014 still has status Proposed. SharedKernel.Tests is not in architecture.md layout or RepositoryLayout.ExpectedProjects. |
| P1-03 | Architecture tests | done | adc24b4 | 1 review round. Rules not tied to an NFR are tagged ADR-0001 (application layer) and ADR-0005 (contracts shape). The integration-event name rule also matches `IntegrationEventV<n>`, following integration-events.md rule 4; story task 7 says only "ends with IntegrationEvent". NFR-6 also reads obj/project.assets.json, so tests need a restore first. If Billing.Tests ever references Billing.Infrastructure (FakePay translator), NFR-6 fails; P6-03 must decide where those tests go. |

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
