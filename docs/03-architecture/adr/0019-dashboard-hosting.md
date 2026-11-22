# ADR-0019: Dashboard hosted in-process, calls the API over HTTP

**Status:** Proposed

**Context.** One deployable (ADR-0001), and DASH-2 says any button calls an API command.

**Decision.** The Blazor Server dashboard is a Razor Class Library hosted by `FulfillmentFlow.Api`. It talks to the API through a typed `HttpClient` against its own base address and has its own DTOs. It references no module.

**Alternatives.** Dashboard injecting application services directly (faster, but bypasses the API contract, idempotency and problem details); a separate SPA or host (a second deployable).

**Consequences.** The dashboard exercises exactly what any client would. The loopback overhead is irrelevant here.

**Defense in one breath.** The dashboard is just another client of the API, so it can't sneak in business rules or shortcuts.
