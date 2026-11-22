# ADR-0008: Allocation strategy behind an interface; nearest by coordinates

**Status:** Proposed

**Context.** Which location supplies a line is business policy that changes (INV-6). "Nearest" needs something to measure from, which v1.0 did not define.

**Decision.** `IAllocationStrategy` is a domain service in `Inventory.Domain`: a pure function of the request (SKU, quantity, destination `GeoPoint`) and the candidate availabilities. Implementations: `NearestWarehouseAllocationStrategy` (haversine distance, tie → lower `LocationId`) and `MostAvailableAllocationStrategy`. Locations carry coordinates; orders carry a shipping address with coordinates (no geocoding). Selected by config `Inventory:AllocationStrategy`.

**Alternatives.** Hard-coded in `StockItem` (it can't see other locations, and it mixes policy with invariants); region distance tables (more data, same idea).

**Consequences.** `StockItem` never changes when policy changes (INV-6.3). Allocation reads several stock items but modifies only the chosen one.

**Defense in one breath.** The rule changes with business policy, so it is a domain service that can be swapped and tested alone.
