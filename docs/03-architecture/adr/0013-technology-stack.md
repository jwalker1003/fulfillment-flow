# ADR-0013: Technology stack

**Status:** Accepted (chosen by Brad, Nov 21 2026)

| Concern | Choice |
| --- | --- |
| Runtime | .NET 9 (`net9.0`), C# 13 |
| Web | ASP.NET Core minimal APIs, built-in OpenAPI document (`Microsoft.AspNetCore.OpenApi`), RFC 9457 problem details |
| UI | Blazor Server (interactive server render mode) in a Razor Class Library |
| Persistence | EF Core 9 + `Microsoft.EntityFrameworkCore.Sqlite`, one SQLite file |
| Tests | xUnit, Shouldly (assertions), `Microsoft.AspNetCore.Mvc.Testing` (integration), NetArchTest.Rules (architecture) |
| Solution | One `.sln`, central package management (`Directory.Packages.props`) |

**Why Shouldly instead of FluentAssertions.** FluentAssertions v8+ requires a paid licence for commercial use. Shouldly is BSD-licensed and reads well. Plain `Assert` is also acceptable.

**Not used:** MediatR, AutoMapper, any mocking library in domain tests, Docker.
