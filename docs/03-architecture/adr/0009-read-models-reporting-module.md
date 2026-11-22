# ADR-0009: Separate read models in a Reporting module

**Status:** Accepted

**Context.** The dashboard needs cross-context views (stock levels, order board, activity). Aggregates are shaped for rules, not display.

**Decision.** `FulfillmentFlow.Reporting` subscribes to all integration events and maintains denormalised `reporting_*` tables. It references only Contracts. It is a read side, not a business context. Rebuild replays the outbox log (ADR-0006).

**Alternatives.** Query aggregates directly (couples the domain to reporting and crosses boundaries); per-context read models (no cross-context view without joining across modules).

**Consequences.** Views are eventually consistent (DASH-1 allows one refresh). Projections must be idempotent; Inventory events carry resulting levels so projections overwrite rather than accumulate.

**Defense in one breath.** Aggregates are shaped for rules, not for display. Read models keep the domain model free of reporting needs.
