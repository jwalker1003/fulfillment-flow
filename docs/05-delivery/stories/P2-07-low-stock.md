# P2-07 Low-stock detection

**Phase:** 2 · **Depends on:** P2-02, P2-05 · **Requirements:** INV-7.1, INV-7.2, INV-7.3

## Goal
A stock item raises `LowStockDetected` once when available crosses down to its threshold, and re-arms when it rises above.

## Read first
- `docs/02-domain/contexts/inventory.md`

## Scope
Inventory.Domain, Inventory.Domain.Tests

## Tasks (test-first)
1. `ReorderThreshold` (`Quantity?`) and `LowStockDetected` (bool) state.
2. `SetReorderThreshold(Quantity? threshold, now)` raises `ReorderThresholdSet` and evaluates immediately.
3. Private `EvaluateLowStock(now)` called at the end of every state-changing method (Receive, Reserve, Release, Expire, Commit, Adjust): if threshold set and available ≤ threshold and not flagged → flag + `LowStockDetected`; if flagged and available > threshold → unflag + `LowStockCleared`.
4. Clearing the threshold (null) unflags without an event.

## Acceptance
INV-7.1–7.3 tests pass, including "two further reservations below threshold raise no second event" and the re-arm cycle.

## Out of scope
Dashboard list (P8-01).

## Done when
Standard.
