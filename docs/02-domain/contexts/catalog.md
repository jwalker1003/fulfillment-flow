# Catalog Context (supporting, plain CRUD)

Requirements: CAT-1, CAT-2. **Deliberately not DDD** (ADR-0010): there are no invariants beyond field validation, so aggregates and domain events here would be ceremony. Say so openly in the defense.

## Shape

One project, `FulfillmentFlow.Catalog`, plus `FulfillmentFlow.Catalog.Contracts`.

- `Product` — a plain EF entity: `Sku` (string, PK), `Name`, `UnitPrice` (decimal), `WeightKg` (decimal), `LengthCm`, `WidthCm`, `HeightCm` (decimal), `IsActive`, `Version` (long, incremented on every change).
- `ProductService` — `Create`, `Update`, `Get`, `List`. Validates with a simple validator (CAT-1.3), checks duplicate SKU (CAT-1.2), and in the same `SaveChanges` writes a `ProductChangedIntegrationEvent` with the full snapshot to `catalog_Outbox` (CAT-2.1).
- Errors still use `Result`/`DomainError` codes so the API maps them the same way as other contexts.

It does **not** reference SharedKernel's `Sku` type in its entity (it stores the string) but validates SKU format with the same rule (`Sku.TryCreate`), which is allowed because SharedKernel is shared.

## Persistence (prefix `catalog_`)

`catalog_Products`, `catalog_Outbox`.
