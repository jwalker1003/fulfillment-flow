# ADR-0015: Per-line reservation on `OrderPlaced`; payment only after all lines reserved

**Status:** Proposed. Resolves v1.0 gaps 2 and 3 and the BIL-1 / SAGA-2 contradiction.

**Context.** `StockReserved` is per stock item, but the flow needs to know when the whole order is reserved. v1.0 had no producer for `ReservationFailed`. BIL-1 charged on `OrderPlaced`, contradicting SAGA-2 ("payment is never requested").

**Decision.**
- Inventory handles `OrderPlaced`: for each line in order, run the allocation strategy and reserve on the chosen stock item (one transaction each). On the first failure, write `ReservationFailed` for that line to the outbox and stop.
- The saga counts `StockReserved` by order line. When all lines are reserved it publishes `PaymentRequested`.
- Billing charges only on `PaymentRequested`.

**Alternatives.** An order-level `OrderStockReserved` event from Inventory (Inventory would need order-level state it doesn't own); reserving all lines in one transaction (breaks NFR-3).

**Consequences.** Reservations made before a later line fails are released by Inventory reacting to `OrderCancelled`. Handler retries are safe because `Reserve` is idempotent by order line (INV-2.4); the handler first looks for an existing reservation for the line across locations.

**Defense in one breath.** Each stock item is its own consistency boundary, so the saga, which owns the order-level view, decides when the order is fully reserved and only then asks for money.
