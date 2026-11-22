# P5-01 `PickList` domain

**Phase:** 5 · **Depends on:** P1-02 · **Requirements:** FUL-1.1 (domain), FUL-2.1, FUL-2.2, FUL-2.3, FUL-3.1, FUL-3.2, FUL-3.3, FUL-4.1 (domain), FUL-4.2

## Goal
The Fulfillment aggregate models picking and dispatch with its rules.

## Read first
- `docs/02-domain/contexts/fulfillment.md`

## Scope
`FulfillmentFlow.Fulfillment.Domain`, `tests/FulfillmentFlow.Fulfillment.Domain.Tests/`

## Tasks (test-first)
1. IDs `PickListId`, `PickId`, `ShipmentId`, `OrderId`; VOs `Carrier`, `TrackingNumber`; `PickInstruction` (domain input record).
2. `PickList.Create(OrderId, instructions, now)`: no instructions → `fulfillment.pick-list-needs-picks`; one pick per instruction; `PickListCreated`.
3. `CompletePick`, `Complete`, `Dispatch`, `Cancel` per the rules table in the context doc.
4. `Shipment` entity created by `Dispatch`; `ShipmentDispatched` carries order ID, carrier, tracking number, and all reservation IDs.

## Acceptance
Listed IDs pass; full status transition coverage (Open → Completed → Dispatched; Open/Completed → Cancelled; Dispatched ignores Cancel).

## Out of scope
Handlers, persistence.

## Done when
Standard.
