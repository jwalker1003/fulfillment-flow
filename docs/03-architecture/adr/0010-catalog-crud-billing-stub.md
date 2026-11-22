# ADR-0010: Catalog as CRUD, Billing as a stub with an ACL

**Status:** Accepted

**Context.** DDD has a cost. Catalog has no invariants beyond validation; Billing is generic (bought, not built).

**Decision.** Catalog is a single project with an EF entity and a service; it still publishes `ProductChanged` through its outbox. Billing has a minimal `Payment` aggregate and an anti-corruption layer around a fake provider (`FakePay`) whose types never leave `Billing.Infrastructure/FakePay`.

**Alternatives.** Full DDD in both.

**Consequences.** A clear "when not to use DDD" example. Known stub limitation: a crash between charging and saving may double-charge on retry. A real gateway would accept `OrderId` as an idempotency key, which the port already passes.

**Defense in one breath.** DDD costs effort. Spend it where the complexity is, and say so openly.
