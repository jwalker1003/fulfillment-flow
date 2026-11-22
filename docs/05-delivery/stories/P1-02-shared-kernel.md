# P1-02 Shared kernel primitives

**Phase:** 1 · **Depends on:** P1-01 · **Requirements:** NFR-7, NFR-8, ADR-0004, ADR-0014

## Goal
`FulfillmentFlow.SharedKernel` provides the base types and value objects every domain uses, each impossible to construct invalid.

## Read first
- `docs/02-domain/glossary.md` (Shared terms)
- `docs/03-architecture/adr/0014-result-type-for-rule-violations.md`
- `docs/04-contracts/error-catalog.md` (Shared kernel codes)

## Scope
`src/BuildingBlocks/FulfillmentFlow.SharedKernel/`, `src/BuildingBlocks/FulfillmentFlow.BuildingBlocks.Contracts/`, a new test project `tests/FulfillmentFlow.SharedKernel.Tests/` (add to sln).

## Tasks (test-first)
1. `DomainError(string Code, string Message)` record; `Result` and `Result<T>` with `IsSuccess`, `IsFailure`, `Error`, `Value` (throws if failure), `Success()`, `Failure(error)`, implicit conversion from `DomainError`.
2. `Entity<TId>` (identity equality), `AggregateRoot<TId>` (private `List<IDomainEvent>`, `protected Raise(IDomainEvent)`, `IReadOnlyList<IDomainEvent> DomainEvents`, `ClearDomainEvents()`, `int Version` with internal increment hook), `IDomainEvent` (`DateTimeOffset OccurredAt`).
3. `ValueObject` approach: use `record` / `readonly record struct` for value objects; no base class needed.
4. `Sku`: `Create(string)` → `Result<Sku>`; trims, uppercases, regex `^[A-Z0-9][A-Z0-9-]{2,31}$`; `From(string)` throws; `ToString()` returns value.
5. `Quantity`: non-negative `int`; `Create(int)`; `Zero`; `+`, `-` (subtraction below zero throws `InvalidOperationException` — callers must check first), comparison operators; `IsPositive`.
6. `Money`: non-negative `decimal`, max 2 decimal places, USD only; `Create(decimal)`; `Zero`; `+`; `* Quantity`.
7. `GeoPoint(double Latitude, double Longitude)`: `Create` validates ranges; `DistanceKmTo(GeoPoint)` (haversine, earth radius 6371 km).
8. `BuildingBlocks.Contracts`: `IIntegrationEvent { Guid MessageId; DateTimeOffset OccurredAt; }`.

## Acceptance
- Tests cover valid and invalid construction for every value object (`shared.invalid-*` codes), equality, arithmetic, haversine (Seattle→Chicago ≈ 2,795 km ± 10).
- No public setters anywhere in SharedKernel.
- SharedKernel has no package references.

## Out of scope
IDs specific to a context (they live in that context's Domain).

## Done when
Standard (see template).
