# ADR-0016: Pick list created from `FulfillmentRequested` after payment

**Status:** Proposed. Resolves v1.0 gap 1.

**Context.** FUL-1 created the pick list on `StockReserved`; SAGA-1 created it after payment. Fulfillment also needs per-line location and reservation IDs.

**Decision.** After `PaymentCaptured`, the saga publishes `FulfillmentRequested(orderId, picks[{orderLineId, sku, locationId, quantity, reservationId}])`. Fulfillment creates the pick list from it. Inventory also handles it to confirm the reservations (ADR-0017).

**Alternatives.** Fulfillment listening to `StockReserved` and `PaymentCaptured` and assembling the state itself (duplicates the saga's job).

**Consequences.** The warehouse never picks unpaid orders. Fulfillment depends only on Ordering's contract.

**Defense in one breath.** You don't pick what hasn't been paid for, and the saga already knows what was reserved where, so it hands Fulfillment a complete instruction.
