# P3-02 Place order and price capture

**Phase:** 3 · **Depends on:** P3-01 · **Requirements:** ORD-2.1, ORD-2.2, ORD-2.3, ORD-2.4, ORD-5.1, ORD-5.2, ORD-5.4

## Goal
Placing freezes the order, captures prices, and raises `OrderPlaced` with everything downstream contexts need.

## Read first
- `docs/02-domain/contexts/ordering.md`
- `docs/04-contracts/integration-events.md` — `OrderPlacedIntegrationEvent` (the domain event must carry the same data)

## Scope
Ordering.Domain, Ordering.Domain.Tests

## Tasks (test-first)
1. `IPriceList` domain interface: `Money? FindActivePrice(Sku)`. Tests use a simple in-test dictionary implementation (not a mock).
2. `Place(IPriceList, now)` → `Result`, checks in order: not Draft → `ordering.order-not-draft`; no lines → `ordering.order-has-no-lines`; no address → `ordering.shipping-address-required`; any SKU without active price → `ordering.unknown-sku`. On success: re-price every line, status Placed, `PlacedAt`, raise `OrderPlaced(orderId, customerRef, lines(lineId, sku, qty, unitPrice), total, shipTo GeoPoint)`.
3. ORD-2.3: after placing, add/change/remove/set address all return `ordering.order-not-draft`.
4. ORD-5.2: changing the price list after placing doesn't change line prices or total.

## Acceptance
Listed IDs pass; a failed `Place` leaves the order Draft with original prices and no events.

## Out of scope
Publishing to other contexts (P3-04/P3-05).

## Done when
Standard.
