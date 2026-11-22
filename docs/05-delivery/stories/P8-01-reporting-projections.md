# P8-01 Reporting projections, views API, rebuild

**Phase:** 8 · **Depends on:** P7-01 · **Requirements:** DASH-1 (data side), DASH-3, SAGA-7.1 (data side), API-7

## Goal
Read models for every dashboard view, updated from integration events and rebuildable from the outbox log.

## Read first
- `docs/03-architecture/adr/0009-read-models-reporting-module.md`
- `docs/03-architecture/architecture.md` — Read side
- `docs/01-product/requirements.md` — §9 Dashboard read models
- `docs/04-contracts/openapi.yaml` — Views and Admin tags

## Scope
`FulfillmentFlow.Reporting`, `FulfillmentFlow.Api/Endpoints/ViewEndpoints.cs`, `AdminEndpoints.cs`, IntegrationTests

## Tasks (test-first)
1. `ReportingDbContext` tables: `reporting_StockLevels`, `reporting_LowStock`, `reporting_OrderBoard`, `reporting_OrderProcess` (+ steps), `reporting_Shipments`, `reporting_Activity`, `reporting_Inbox`.
2. Projections (idempotent via inbox): stock levels upsert from every Inventory event's resulting levels; low stock add/remove; order board from Ordering events (+ total from `OrderPlaced`, step from `OrderFulfillmentStepChanged`); process view; shipments; activity (every event, human summary, keep last 200).
3. Query services + `GET /views/*` endpoints.
4. `IReadModelRebuilder`: truncate reporting tables + inbox, replay all module outbox rows ordered by `OccurredAt`, `Sequence` into Reporting handlers only. `POST /admin/read-models/rebuild`.
5. Test DASH-3: run the full happy-path + one failure scenario, snapshot every view, rebuild, compare → identical.
6. Test DASH-1: receive stock → drain → stock level view updated.

## Acceptance
Listed IDs pass; Reporting references only Contracts and BuildingBlocks (architecture test).

## Done when
Standard.
