# P2-02 Reserve stock

**Phase:** 2 · **Depends on:** P2-01 · **Requirements:** INV-2.1, INV-2.2, INV-2.3, INV-2.4, INV-2.5 (domain part)

## Goal
`StockItem.Reserve` protects "available ≥ 0" and is idempotent per order line.

## Read first
- `docs/02-domain/contexts/inventory.md`
- `docs/03-architecture/adr/0003-stockitem-holds-reservations.md`

## Scope
Inventory.Domain (`StockItems/Reservation.cs`, `ReservationStatus.cs`, `StockItem.cs`, events), Inventory.Domain.Tests

## Tasks (test-first)
1. `Reservation` entity: `Id`, `OrderId`, `OrderLineId`, `Quantity`, `Status` (Active, Released, Expired, Committed), `ExpiresAt` (nullable).
2. `Reserve(OrderId, OrderLineId, Quantity, DateTimeOffset expiresAt, DateTimeOffset now)` → `Result<ReservationId>`.
   - quantity must be positive (`inventory.quantity-must-be-positive`)
   - existing reservation for the same `OrderLineId` (any status) → success returning its ID, no event (INV-2.4)
   - quantity > available → `inventory.insufficient-stock`
3. `Reserved` = sum of Active reservations; `Available` = OnHand − Reserved.
4. `StockReserved` event with order/line/reservation IDs, SKU, location, quantity, expiresAt, resulting on-hand and reserved.
5. INV-2.5: test that `ExpiresAt` equals the passed `expiresAt` (lifetime calculation is the application's job).

## Acceptance
All INV-2.1–2.5 tests pass, including "nothing changes" assertions on rejection.

## Out of scope
Choosing the location (P2-06); `ReservationFailed` (application, P3-05).

## Done when
Standard.
