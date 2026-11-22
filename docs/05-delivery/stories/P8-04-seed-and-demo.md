# P8-04 Seed data and demo script

**Phase:** 8 · **Depends on:** P8-03 · **Requirements:** demo readiness — Phase 8 gate

## Goal
A fresh database starts with realistic data, and a written script walks through every DDD talking point.

## Scope
`FulfillmentFlow.Api/Seed/` (runs when `Seed:Enabled=true` and the database is empty, **through application commands**, not direct inserts), `docs/demo-script.md`

## Seed data
- Locations: `SEA-1` Seattle (47.61, −122.33), `CHI-1` Chicago (41.88, −87.63), `ATL-1` Atlanta (33.75, −84.39).
- Products (Catalog): 6 SKUs, e.g. `MUG-BLK` 12.50, `MUG-WHT` 12.50, `TEE-M` 20.00, `TEE-L` 20.00, `CAP-RED` 15.00, `BOTTLE-1L` 13.13 (handy for the payment-failure demo).
- Stock: varied on-hand per location; at least one SKU stocked at only one location; one SKU with a reorder threshold close to on-hand.

## Demo script (`docs/demo-script.md`)
Ten-minute walkthrough: context map → place an order and watch the saga → insufficient stock → payment failure → cancel a paid order (refund flagged) → low stock → cycle count → rebuild read models → show architecture tests failing when a forbidden reference is added. Each step names the ADR to talk to.

## Acceptance
`dotnet run` on a fresh checkout shows populated dashboard pages; script steps all work as written.

## Done when
Standard.
