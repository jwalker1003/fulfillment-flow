# Test Strategy

Tests are the proof that the code does what the rules say. They are written **before** the code, from the acceptance criteria, and reviewed by Brad; reviewing tests is where understanding is built.

## Layers

| Layer | Project(s) | What it proves | Tools | Speed |
| --- | --- | --- | --- | --- |
| Domain | `*.Domain.Tests`, `SharedKernel.Tests`, `Billing.Tests` | Every INV/ORD/FUL/SAGA/BIL rule as pure logic | xUnit, Shouldly | whole suite < 10 s |
| Application | `Application.Tests` | Handlers orchestrate correctly; idempotency by natural keys; cross-module flows via `InMemoryMessageBus` | xUnit, Shouldly, hand-written in-memory repositories | seconds |
| Integration | `IntegrationTests` | Persistence round-trips, outbox/inbox, crash recovery, concurrency, saga end to end, HTTP API, read models | `WebApplicationFactory`, SQLite in-memory (file-based for concurrency), `FakeClock`, `IOutboxDrainer` | < 2 min |
| Architecture | `ArchitectureTests` | NFR-1, NFR-2, NFR-6, NFR-8, BIL-2.1, contract shape | NetArchTest.Rules + reflection | seconds |

## Rules

1. **One requirement, at least one test.** Tag every test `[Trait("Req", "INV-2.2")]` (multiple traits allowed). Name: `INV_2_2_reserving_more_than_available_is_rejected`.
2. **Given/When/Then structure** in the test body with blank lines (no comments needed if names are clear).
3. **Rejections assert three things:** the error code, unchanged state, and no new domain events.
4. **No mocks in domain tests** (ADR-0012). Use real value objects and small in-test implementations of domain interfaces (`IPriceList`).
5. **Time is injected.** Domain tests pass `DateTimeOffset` values; other tests use `FakeClock`. Never `DateTime.Now`.
6. **Integration tests never sleep.** They drain outboxes with `IOutboxDrainer.DrainAsync()` and run jobs by calling their command directly.
7. **No 500s.** The API test base asserts no response has status 500 unless the test expects it.
8. **Deterministic data.** Test data builders per context (`StockItemBuilder`, `OrderBuilder`) live in each test project's `Builders/` folder.

## Helpers to build early

- `DomainAssert.Rejected(result, "inventory.insufficient-stock")`
- `aggregate.ShouldHaveRaised<StockReserved>()` / `ShouldHaveRaisedNothing()` extensions
- `FakeClock` (settable, `Advance(TimeSpan)`)
- `IntegrationTestBase` (factory, scoped services, drainer, HTTP client, `GetOutboxMessages<T>()`)

## Architecture tests

See story P1-03 for the full rule list. They run in every `dotnet test` and fail the build on violation.

## What we don't test

- EF Core itself, ASP.NET routing internals, Blazor rendering (dashboard has no logic; verified manually per DASH-2).
- Performance beyond NFR-6 timing.
