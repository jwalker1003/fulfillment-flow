# P2-08 Cycle count

**Phase:** 2 · **Depends on:** P2-05 · **Requirements:** INV-8.1, INV-8.2, INV-8.3 (domain part), INV-8.4, INV-8.5

## Goal
A `StockCount` aggregate records counted vs expected quantities, proposes reasoned adjustments, and becomes immutable when closed.

## Read first
- `docs/02-domain/contexts/inventory.md` — `StockCount`

## Scope
Inventory.Domain (`StockCounts/`), Inventory.Domain.Tests

## Tasks (test-first)
1. `StockCount.Start(LocationId, now)` → Open, raises `StockCountStarted`.
2. `RecordCount(Sku, Quantity expected, Quantity counted)`: closed → `inventory.stock-count-closed`; recording the same SKU again replaces the line.
3. `StockCountLine.Variance` = counted − expected.
4. `ProposedAdjustments`: lines with variance ≠ 0 → (sku, change = variance, reason "Cycle count {id}").
5. `Close(now)`: closed → `inventory.stock-count-closed`; no lines → `inventory.stock-count-empty`; else Closed + `StockCountClosed(adjustments)`.

## Acceptance
INV-8.1, 8.2, 8.4, 8.5 tests pass; INV-8.3 domain part: the closed event carries exactly the non-zero variances.

## Out of scope
Applying adjustments to stock items (P4/P7: application handler `ApplyStockCountAdjustments`, delivered in P7-03).

## Done when
Standard.
