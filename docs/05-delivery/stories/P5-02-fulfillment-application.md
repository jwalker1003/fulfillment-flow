# P5-02 Fulfillment application and persistence

**Phase:** 5 · **Depends on:** P5-01, P4-03 · **Requirements:** FUL-1.1, FUL-1.2, FUL-4.1, FUL-4.2

## Goal
Fulfillment reacts to `FulfillmentRequested` and `OrderCancelled`, and exposes commands for picking and dispatch, persisted with outbox and inbox.

## Read first
- `docs/02-domain/contexts/fulfillment.md`
- `docs/04-contracts/integration-events.md` — Fulfillment and Ordering sections

## Scope
`Fulfillment.Contracts`, `Fulfillment.Application`, `Fulfillment.Infrastructure`, Application.Tests, IntegrationTests

## Tasks (test-first)
1. Contracts: all Fulfillment integration events.
2. Application: `IPickListRepository` (`GetByOrderId`, `Get`, `Save`), handlers `CreatePickList` (FulfillmentRequested; existing pick list for order → no-op), `CancelPickList` (OrderCancelled; none → ignore), commands `CompletePick`, `CompletePickList`, `DispatchShipment`; mapper.
3. Infrastructure: `FulfillmentDbContext` (unique `OrderId`), repositories, migration, outbox/inbox, `AddFulfillmentModule`.
4. Tests: duplicate `FulfillmentRequested` → one pick list; cancel before dispatch → Cancelled; cancel after dispatch → unchanged.

## Acceptance
Listed IDs pass at application and integration level.

## Out of scope
Endpoints (P7-04).

## Done when
Standard.
