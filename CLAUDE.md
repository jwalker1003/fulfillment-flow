# Fulfillment Flow — instructions for Claude Code

A DDD demonstration in C#: a warehouse order-fulfillment system built as a .NET 9 modular monolith with five bounded contexts, an outbox, and a saga. **The design is the product.** Correct boundaries and readable rules matter more than features.

## Source of truth (read in this order)
1. `docs/00-index.md` — map of all docs.
2. `docs/01-product/requirements.md` — **authoritative** requirements with sub-IDs (`INV-2.3`). The original `Requierments and AC Specs.md` is history only.
3. `docs/02-domain/glossary.md` — every class, method and event name comes from here.
4. The story you are implementing in `docs/05-delivery/stories/`, and only the docs it lists under *Read first*.

If docs conflict: requirements.md > ADRs > context docs > stories. Report any conflict in the PR; do not silently pick one.

## Stack
.NET 9 · C# 13 · ASP.NET Core minimal APIs · Blazor Server · EF Core 9 + SQLite · xUnit + Shouldly · NetArchTest. Package versions only in `Directory.Packages.props`. No MediatR, AutoMapper, FluentAssertions, Moq/NSubstitute in domain tests.

## Commands
```
dotnet build -warnaserror
dotnet test
dotnet test --filter "Req=INV-2.2"
dotnet run --project src/FulfillmentFlow.Api
dotnet ef migrations add <Name> --project src/Modules/<Ctx>/FulfillmentFlow.<Ctx>.Infrastructure --startup-project src/FulfillmentFlow.Api --context <Ctx>DbContext
```

## Workflow for every story
1. Read the story and its *Read first* docs. Do not start a story whose dependencies are not done, or a phase whose previous gate hasn't passed (`docs/05-delivery/roadmap.md`).
2. Write failing tests for every requirement ID in the story, tagged `[Trait("Req", "<ID>")]`. Commit them.
3. Implement the minimum to make them pass. Commit.
4. Run the full `dotnet build -warnaserror` and `dotnet test`.
5. Update `docs/05-delivery/traceability.md`, and the glossary, error catalog or contracts if you added anything.
6. Check `docs/06-quality/definition-of-done.md`.
7. Stay inside the story's *Scope*. Anything else becomes a note in the PR, not code.

## Non-negotiable design rules
- **Domain purity (NFR-1):** `*.Domain` references only `FulfillmentFlow.SharedKernel`. No EF attributes, no `System.Text.Json`, no logging, no clocks.
- **Context isolation (NFR-2):** a module may reference another module's `*.Contracts` only. Never its Domain/Application/Infrastructure or tables.
- **One aggregate per transaction (NFR-3).** Anything else goes through events. The saga changes `Order` only via internal commands in the outbox.
- **Outbox (NFR-4):** never publish directly; repositories write integration events to the module outbox in the same `SaveChanges`.
- **Idempotent handlers (NFR-5):** inbox row or natural-key check in every handler.
- **Rule violations return `Result` with a `DomainError` code** from `docs/04-contracts/error-catalog.md`. No exceptions for business rules. A failed method leaves state unchanged and raises no events.
- **Value objects validate in their factory (NFR-7).** No public setters on entities/aggregates (NFR-8); state changes through methods named in the glossary.
- **Time is a parameter** in the domain (`DateTimeOffset now`); `IClock` elsewhere.
- **Thin edges:** endpoints bind, dispatch, and map results. Dashboard components call the API over HTTP and contain no rules.

## Code conventions
- File-scoped namespaces; one public type per file; `sealed` by default; `internal` unless needed across projects.
- IDs: `readonly record struct XxxId(Guid Value)` with `New()` using `Guid.CreateVersion7()`.
- Value objects: `record` / `readonly record struct` with private constructor + `static Result<T> Create(...)`.
- Domain event names: past tense, glossary terms (`StockReserved`). Integration events: same name + `IntegrationEvent`.
- Error codes: `public static class InventoryErrors { public const string InsufficientStock = "inventory.insufficient-stock"; ... }` per context.
- Tests: `INV_2_2_reserving_more_than_available_is_rejected`; Given/When/Then blocks separated by blank lines.

## Never
- Change `docs/01-product/requirements.md` acceptance criteria to make a test pass. Raise it instead.
- Add a project reference not allowed by `docs/03-architecture/architecture.md`.
- Add a type to SharedKernel without an ADR.
- Delete outbox rows (they are the event log).
- Use `DateTime.Now`/`UtcNow` directly, `Thread.Sleep`/`Task.Delay` in tests, or `[Fact(Skip=…)]`.
- Commit `*.db` files or secrets.
