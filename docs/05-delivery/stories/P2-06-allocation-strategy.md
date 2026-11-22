# P2-06 Allocation strategies

**Phase:** 2 · **Depends on:** P2-01 · **Requirements:** INV-6.1, INV-6.2, INV-6.3, INV-6.4, INV-6.5

## Goal
A swappable domain service chooses which location supplies a line.

## Read first
- `docs/02-domain/contexts/inventory.md` — Allocation strategies
- `docs/03-architecture/adr/0008-allocation-strategy.md`

## Scope
Inventory.Domain (`Allocation/`), Inventory.Domain.Tests

## Tasks (test-first)
1. `AllocationRequest(Sku, Quantity, GeoPoint Destination)`, `StockAvailability(LocationId, GeoPoint, Quantity Available)`.
2. `IAllocationStrategy.Choose(AllocationRequest, IReadOnlyList<StockAvailability>)` → `Result<LocationId>`.
3. `NearestWarehouseAllocationStrategy`: filter `Available >= requested`; order by `GeoPoint.DistanceKmTo(destination)`, then `LocationId` ordinal; none → `inventory.no-location-can-supply`.
4. `MostAvailableAllocationStrategy`: same filter; order by available desc, then `LocationId`.
5. INV-6.3 test: one `StockItem` instance reserves using the location chosen by each strategy; no `StockItem` code references any strategy (assert by reflection that `StockItem` has no dependency on `IAllocationStrategy`).
6. INV-6.5: the caller only passes stock items that exist; test that an empty list → no-location-can-supply.

## Acceptance
INV-6.1–6.5 tests pass using realistic coordinates (Seattle 47.61,−122.33; Chicago 41.88,−87.63; Atlanta 33.75,−84.39).

## Out of scope
Loading candidates from a repository (P3-05).

## Done when
Standard.
