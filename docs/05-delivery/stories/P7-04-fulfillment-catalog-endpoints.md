# P7-04 Fulfillment and Catalog endpoints

**Phase:** 7 · **Depends on:** P7-01 · **Requirements:** API-3 (Fulfillment, Catalog codes), FUL-2, FUL-3, CAT-1, CAT-2 over HTTP

## Goal
Pick lists can be worked and dispatched, and products managed, over HTTP.

## Read first
- `docs/04-contracts/openapi.yaml` — Fulfillment and Catalog tags

## Scope
`FulfillmentFlow.Api/Endpoints/FulfillmentEndpoints.cs`, `CatalogEndpoints.cs`, Fulfillment.Application queries, IntegrationTests

## Tasks (test-first)
1. Fulfillment: list (filter by status), get, complete pick, complete list, dispatch.
2. Catalog: list, get, create, update.
3. Full HTTP happy path test: create product → receive stock → create/place order → drain → complete picks → dispatch → `GET /orders/{id}` shows Shipped.

## Acceptance
Tests pass; documented status codes for every error.

## Done when
Standard.
