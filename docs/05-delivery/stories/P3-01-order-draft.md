# P3-01 Draft order

**Phase:** 3 · **Depends on:** P1-02 · **Requirements:** ORD-1.1, ORD-1.2, ORD-1.3, ORD-1.4, ORD-1.6, ORD-1.7, ORD-5.3

## Goal
A Draft `Order` can be created and edited, and its total always equals its lines.

## Read first
- `docs/02-domain/contexts/ordering.md`

## Scope
`src/Modules/Ordering/FulfillmentFlow.Ordering.Domain/` (`Orders/`, `Events/`), `tests/FulfillmentFlow.Ordering.Domain.Tests/`

## Tasks (test-first)
1. IDs `OrderId`, `OrderLineId`; `CustomerReference` VO; `ShippingAddress` VO (`ordering.invalid-shipping-address`); `OrderStatus` enum.
2. `Order.Create(CustomerReference, now)` → Draft, no lines, raises `OrderCreated`.
3. `AddLine(Sku, Quantity, Money currentPrice)`: not Draft → `ordering.order-not-draft`; quantity 0 → `ordering.quantity-must-be-positive`; existing SKU → merge quantity (keep the newer price); internal `OrderLineAdded`.
4. `ChangeLineQuantity(Sku, Quantity)`, `RemoveLine(Sku)` (`ordering.line-not-found`).
5. `SetShippingAddress(ShippingAddress)`.
6. `Total` computed = Σ quantity × unit price. Property-based style test: after any sequence of add/change/remove, total equals the sum (ORD-5.3).

## Acceptance
Listed IDs pass. `Lines` is `IReadOnlyList<OrderLine>`; no setters.

## Out of scope
SKU validity against the product copy (ORD-1.5 is application-level, P3-04/P6-02); placing.

## Done when
Standard.
