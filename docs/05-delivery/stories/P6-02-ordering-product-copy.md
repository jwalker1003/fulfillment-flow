# P6-02 Ordering product copy

**Phase:** 6 · **Depends on:** P6-01 · **Requirements:** CAT-2.2, ORD-1.5, ORD-5.1, ORD-5.4 (with real data)

## Goal
Ordering's `CatalogProduct` copy is fed by `ProductChanged`, so prices and SKU validity come from Catalog without coupling.

## Read first
- `docs/02-domain/contexts/ordering.md`
- `docs/03-architecture/adr/0004-sku-reference-and-local-copies.md`

## Scope
Ordering.Domain (`CatalogProducts/`), Ordering.Application, Ordering.Infrastructure, tests

## Tasks (test-first)
1. `CatalogProduct.Create(...)` and `ApplyChange(price, isActive, version)` → returns false (ignored) when version ≤ current.
2. `UpdateProductCopy : IIntegrationEventHandler<ProductChangedIntegrationEvent>` (upsert, inbox).
3. EF-backed `IPriceListReader` over `ordering_CatalogProducts`.
4. Integration test: create product in Catalog → drain → add line in Ordering uses its price; update price → draft line keeps old price until `Place` re-captures; deactivate → `Place` rejected with `ordering.unknown-sku`; out-of-order older version ignored.

## Acceptance
Listed IDs pass end to end.

## Done when
Standard.
