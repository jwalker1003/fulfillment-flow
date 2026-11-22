# P7-03 Inventory endpoints

**Phase:** 7 · **Depends on:** P7-01 · **Requirements:** API-4, API-5, INV-1.4, INV-8.3 (application part)

## Goal
All `/inventory` endpoints work, including cycle counts whose closure applies adjustments.

## Read first
- `docs/04-contracts/openapi.yaml` — Inventory tag
- `docs/02-domain/contexts/inventory.md` — Application layer

## Scope
`FulfillmentFlow.Api/Endpoints/InventoryEndpoints.cs`, Inventory.Application (stock count commands, `ApplyStockCountAdjustments` handler, queries), IntegrationTests

## Tasks (test-first)
1. Endpoints: locations (POST/GET), receipts, adjustments, reorder threshold, stock counts (start, get, record line, close).
2. `RecordCount` reads expected = current on-hand of the stock item (0 if none).
3. `ApplyStockCountAdjustments : IIntegrationEventHandler<StockCountClosedIntegrationEvent>` → `Adjust` per line; rejection logged (open question 2).
4. HTTP tests: INV-1 via receipts; receipt for unknown location → 422 `inventory.unknown-location`; adjustment without reason → 422 (API-5); adjustment below reserved → 409; count close applies adjustments after drain.

## Acceptance
Listed IDs pass.

## Done when
Standard.
