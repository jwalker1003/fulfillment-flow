# P2-05 Adjust stock

**Phase:** 2 · **Depends on:** P2-01 · **Requirements:** INV-5.1, INV-5.2, INV-5.3, INV-5.4

## Goal
Manual corrections to on-hand always carry a reason and can never break the invariants.

## Read first
- `docs/02-domain/contexts/inventory.md`

## Scope
Inventory.Domain, Inventory.Domain.Tests

## Tasks (test-first)
1. `Adjust(int quantityChange, string? reason, DateTimeOffset now)` → `Result`, checks in this order:
   - reason null/whitespace → `inventory.adjustment-reason-required`
   - change == 0 → `inventory.adjustment-must-change-quantity`
   - new on-hand < 0 or < reserved → `inventory.on-hand-below-reserved`
2. `StockAdjusted(old on-hand, new on-hand, reason, reserved)`.

## Acceptance
INV-5.1–5.4 tests pass, including the exact example (10 on-hand, 3 reserved, adjust −8 → rejected).

## Out of scope
Low-stock reaction (P2-07).

## Done when
Standard.
