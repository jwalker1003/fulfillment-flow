# P6-01 Catalog CRUD and `ProductChanged`

**Phase:** 6 · **Depends on:** P4-02 · **Requirements:** CAT-1.1, CAT-1.2, CAT-1.3, CAT-2.1

## Goal
Products can be created and updated; every change publishes a full snapshot through the outbox. Deliberately plain CRUD.

## Read first
- `docs/02-domain/contexts/catalog.md`
- `docs/03-architecture/adr/0010-catalog-crud-billing-stub.md`

## Scope
`FulfillmentFlow.Catalog`, `FulfillmentFlow.Catalog.Contracts`, Application.Tests / IntegrationTests

## Tasks (test-first)
1. Contracts: `ProductChangedIntegrationEvent`.
2. `Product` EF entity (public getters, private setters are fine here — it is not an aggregate; the NFR-8 rule only targets `Entity<>`/`AggregateRoot<>` types).
3. `ProductService`: `Create`, `Update`, `Get`, `List` returning `Result`; validation (`catalog.invalid-product`, `shared.invalid-sku`); duplicate (`catalog.duplicate-sku`); `Version` increments; outbox row written in the same `SaveChanges`.
4. `CatalogDbContext`, migration, `AddCatalogModule`.

## Acceptance
Listed IDs pass; a code comment at the top of `ProductService` points to ADR-0010 explaining why this is not DDD.

## Out of scope
Endpoints (P7-04).

## Done when
Standard.
