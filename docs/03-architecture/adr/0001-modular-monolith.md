# ADR-0001: Modular monolith

**Status:** Accepted

**Context.** The system has five bounded contexts. The domain is still being learned, and the goal is to show boundaries, not to operate distributed infrastructure.

**Decision.** One deployable (`FulfillmentFlow.Api`) hosting all modules. Each context is a set of projects with enforced reference rules (architecture tests), its own tables, and communicates only via integration events on an in-process bus.

**Alternatives.** Microservices: real process isolation, but brokers, deployment and distributed debugging cost far more than they teach here.

**Consequences.** Cheap refactoring of boundaries; one SQLite file; the bus abstraction and per-module outboxes mean a broker could replace the in-process bus without touching domain code.

**Defense in one breath.** Boundaries matter more than processes. A monolith keeps them cheap to change while the domain is still being learned, and modules can be split later.
