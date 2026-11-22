# P1-03 Architecture tests

**Phase:** 1 · **Depends on:** P1-01 · **Requirements:** NFR-1, NFR-2, NFR-6, NFR-8, BIL-2.1 (rule only)

## Goal
The build fails if any boundary in the architecture document is broken. Rules are written now, against empty projects, so they guard every later story.

## Read first
- `docs/03-architecture/architecture.md` — *Allowed project references*
- `docs/06-quality/test-strategy.md` — *Architecture tests*

## Scope
`tests/FulfillmentFlow.ArchitectureTests/`

## Tasks
Write NetArchTest (plus plain reflection where NetArchTest can't express it) tests, each with `[Trait("Req", "NFR-x")]`:

1. **NFR-1** Each `*.Domain` assembly depends on nothing in `FulfillmentFlow.*` except `FulfillmentFlow.SharedKernel`, and not on `Microsoft.EntityFrameworkCore`, `Microsoft.AspNetCore`, `System.Text.Json`.
2. **NFR-2** For each context X and each other context Y: X.Domain/Application/Infrastructure do not depend on Y.Domain/Application/Infrastructure. `*.Contracts` depend only on `FulfillmentFlow.BuildingBlocks.Contracts`. `FulfillmentFlow.Reporting` depends on no `*.Domain`/`*.Application`/`*.Infrastructure`. `FulfillmentFlow.Dashboard` depends on no `FulfillmentFlow.*` module assembly.
3. **Application layer** `*.Application` does not depend on `Microsoft.EntityFrameworkCore` or `Microsoft.AspNetCore`.
4. **NFR-8** No type deriving from `Entity<>` or `AggregateRoot<>` has a public or internal property setter (reflection: `SetMethod?.IsPublic == true` fails; `init` counts as a setter).
5. **NFR-6** Domain test assemblies reference none of: `Microsoft.EntityFrameworkCore*`, `Microsoft.AspNetCore*`, `Moq`, `NSubstitute`, `FakeItEasy`.
6. **BIL-2.1** No type in `FulfillmentFlow.Billing.Domain` or `FulfillmentFlow.Billing.Application` depends on a namespace containing `.FakePay`.
7. **Contracts shape** Every public type in `*.Contracts` is a sealed record; every type whose name ends with `IntegrationEvent` implements `IIntegrationEvent`.

Load assemblies via a marker type per project (add an empty `internal`-free `public static class AssemblyMarker` to each project if needed).

## Acceptance
- All rules pass on the scaffold.
- Each rule has a deliberately broken sample verified locally (not committed) to prove it fails.

## Out of scope
Error-catalog completeness test (added in P7-01).

## Done when
Standard.
