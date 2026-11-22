# ADR-0005: Events with eventual consistency between contexts

**Status:** Accepted

**Context.** Contexts must stay independent and failures contained.

**Decision.** Contexts never call each other. They publish integration events from their `*.Contracts` project; other contexts subscribe. The saga sends commands only to Ordering itself (ADR-0007).

**Alternatives.** Synchronous in-process calls between application services: simpler to read, but couples modules and hides failure handling.

**Consequences.** Partial states are visible (an order is Placed but not yet reserved); the saga and dashboard make them explicit. All handlers must be idempotent (NFR-5).

**Defense in one breath.** Events keep contexts independent and failures contained, at the cost of handling partial states.
