# P8-03 Order board, process view, shipments, activity

**Phase:** 8 · **Depends on:** P8-02 · **Requirements:** SAGA-7.1, DASH-2 — Phase 8 gate (with P8-04)

## Goal
The dashboard can drive and watch the whole order flow.

## Scope
`FulfillmentFlow.Dashboard`

## Tasks
1. Orders page: board grouped by status with age; "New order" form (customer ref, lines, address picker with the three seed cities' coordinates), Place, Cancel (with reason).
2. Order detail: lines, total, current saga step, end reason, step history timeline (from `/views/orders/{id}/process`).
3. Pick lists page: open pick lists, complete picks, complete list, dispatch (carrier + tracking number).
4. Shipments page; Activity feed page (newest first, auto-refresh).
5. Admin page: rebuild read models button; dead letters table.

## Acceptance
Manual demo in the PR description: happy path order to Shipped; an order with total 13.13 ends Cancelled "payment failed"; an order exceeding stock ends Cancelled "insufficient stock"; each shows its history.

## Done when
Standard.
