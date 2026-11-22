# P1-01 Solution scaffold

**Phase:** 1 · **Depends on:** — · **Requirements:** NFR-1 (structure), ADR-0013, ADR-0020

## Goal
An empty but complete solution with every project from the architecture layout, correct project references, central package management, and a green `dotnet build` / `dotnet test`.

## Read first
- `docs/03-architecture/architecture.md` — *Solution layout* and *Allowed project references*
- `docs/03-architecture/adr/0013-technology-stack.md`

## Scope
Repository root: `FulfillmentFlow.sln`, `Directory.Build.props`, `Directory.Packages.props`, `.editorconfig`, `.gitignore`, `global.json`, all projects under `src/` and `tests/`.

## Tasks
1. `global.json` pinning the .NET 9 SDK (`rollForward: latestFeature`).
2. `Directory.Build.props`: `net9.0`, `Nullable=enable`, `ImplicitUsings=enable`, `TreatWarningsAsErrors=true`, `LangVersion=latest`, `AnalysisLevel=latest-recommended`.
3. `Directory.Packages.props` with pinned versions for: EF Core 9 + Sqlite + Design, Microsoft.AspNetCore.OpenApi 9, xunit, xunit.runner.visualstudio, Microsoft.NET.Test.Sdk, Shouldly, NetArchTest.Rules, Microsoft.AspNetCore.Mvc.Testing 9, coverlet.collector.
4. Create every project listed in the layout (class libraries; `FulfillmentFlow.Api` as `web`; `FulfillmentFlow.Dashboard` as `razorclasslib`; test projects as `xunit`). Add them to the `.sln` in solution folders matching the tree.
5. Add only the project references allowed by the table. Domain projects reference only SharedKernel.
6. `FulfillmentFlow.Api` `Program.cs`: minimal host with `GET /health` returning `200 "ok"`.
7. One placeholder test per test project so `dotnet test` discovers them.
8. `.gitignore` (standard VisualStudio template plus `*.db`, `*.db-shm`, `*.db-wal`).

## Acceptance
- `dotnet build -warnaserror` succeeds with zero warnings.
- `dotnet test` succeeds.
- `dotnet run --project src/FulfillmentFlow.Api` serves `/health`.
- No package versions in any `.csproj` (all in `Directory.Packages.props`).

## Out of scope
Any domain code, EF DbContexts, endpoints beyond `/health`.

## Done when
Build and tests are green; layout matches the architecture document exactly.
