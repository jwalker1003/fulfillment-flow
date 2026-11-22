# ADR-0004: SKU reference only, local copies, minimal shared kernel

**Status:** Accepted

**Context.** Ordering needs prices and valid SKUs; Inventory and Fulfillment need only the SKU. Sharing a `Product` class would couple every context to Catalog.

**Decision.**
- Contexts refer to products by SKU only.
- Ordering keeps `CatalogProduct` (SKU, unit price, active flag, catalog version), updated from `ProductChanged`. Draft lines use its current price; `Place` re-captures prices from it (ORD-1.2, ORD-5.1). Older versions are ignored (CAT-2.2).
- `FulfillmentFlow.SharedKernel` holds only primitives every context agrees on: `Sku`, `Quantity`, `Money`, `GeoPoint`, base classes, `Result`. Adding a type needs a new ADR.
- Contracts use primitives, not SharedKernel types, so they can be versioned independently.

**Alternatives.** Shared `Product` class; synchronous lookup from Ordering to Catalog.

**Consequences.** Eventual consistency: a product created a moment ago may not be orderable yet. A price change never affects a placed order (ORD-5.2).

**Defense in one breath.** Sharing a model couples contexts. Each context keeps what it needs and the meaning of Product stays local.
