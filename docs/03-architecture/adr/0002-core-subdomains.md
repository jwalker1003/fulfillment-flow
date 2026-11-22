# ADR-0002: Inventory and Fulfillment are the core subdomains

**Status:** Accepted

**Context.** Effort should go where complexity and differentiation are.

**Decision.** Inventory and Fulfillment get the richest models (aggregates with invariants, domain services, exhaustive tests). Ordering is supporting (one aggregate plus the saga). Catalog is CRUD. Billing is a stub.

**Alternatives.** Ordering as core: orders are central to the business but their rules are simple (a state machine and a total).

**Consequences.** Most tests and design discussion live in Inventory and Fulfillment.

**Defense in one breath.** Stock accuracy and fulfillment flow are the hard, differentiating rules, so they get the richest model.
