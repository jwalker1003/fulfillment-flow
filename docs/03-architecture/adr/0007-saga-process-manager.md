# ADR-0007: Saga as a process manager in Ordering

**Status:** Proposed

**Context.** Placing an order spans Ordering, Inventory, Billing and Fulfillment with no shared transaction. Failure paths need compensation. NFR-3 forbids changing two aggregates in one transaction, and contexts integrate through events only.

**Decision.**
- `OrderFulfillmentProcess` is an orchestrating process manager owned by **Ordering**. State lives in `ordering_FulfillmentProcesses`.
- It reacts to integration events and **publishes** integration events for other contexts (`PaymentRequested`, `FulfillmentRequested`, `RefundRequested`, `OrderFulfillmentStepChanged`).
- To change `Order`, it writes **internal commands** (`MarkOrderPaid`, `MarkOrderShipped`, `CancelOrder`) to Ordering's outbox; Ordering's command handlers change `Order` in their own transaction. The process state is the one aggregate of the saga's transaction.
- Compensation = cancel the order. Inventory releases reservations and Fulfillment cancels the pick list in response to `OrderCancelled`. A refund is requested when the process enters `Cancelled` after payment was captured.
- The process logic is pure (`Handle(event, now)`) and unit-tested without infrastructure.

**Alternatives.** Choreography only (no central state; SAGA-7 visibility is harder). A separate saga module (cleaner, more projects, still needs Ordering's commands). Changing `Order` directly in the saga's transaction (simpler, breaks NFR-3).

**Consequences.** One extra outbox hop between a saga decision and the order status change. The state machine and history are easy to show on the dashboard.

**Defense in one breath.** There is no cross-context transaction. A process manager makes the steps and failure paths explicit, and routing its order changes through the outbox keeps one aggregate per transaction.
