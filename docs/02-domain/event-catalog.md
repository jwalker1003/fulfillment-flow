# Event Catalog

Two kinds of message:

- **Domain events** are raised by aggregates inside a context (`FulfillmentFlow.<Context>.Domain.Events`). They are named exactly as below.
- **Integration events** are the public contract (`FulfillmentFlow.<Context>.Contracts`). Each has the same name plus the suffix `IntegrationEvent`. A module-level mapper turns domain events into integration events when the aggregate is saved; both are written to the outbox in the same transaction (ADR-0006).

Some domain events stay internal (marked *internal*). Field-level contracts: `docs/04-contracts/integration-events.md`.

**Envelope (every integration event):** `MessageId` (Guid v7), `OccurredAt` (DateTimeOffset UTC), `Type` (full CLR name), `Version` (int, starts at 1), `CorrelationId` (OrderId when about an order).

## Inventory

| Event | Raised by | When | Integration consumers |
| --- | --- | --- | --- |
| `StockReceived` | `StockItem.Receive` | INV-1.1, INV-1.3 | Reporting |
| `StockReserved` | `StockItem.Reserve` | INV-2.1 | Ordering (saga), Reporting |
| `ReservationFailed` | `ReserveStockForOrder` handler (application) | INV-2.6 | Ordering (saga), Reporting |
| `ReservationReleased` | `StockItem.ReleaseReservation` | INV-3.1, INV-3.5 | Reporting |
| `ReservationExpired` | `StockItem.ExpireReservations` | INV-3.2 | Ordering (saga), Reporting |
| `ReservationConfirmed` | `StockItem.ConfirmReservation` | INV-3.4 | *internal* (Reporting via activity only) |
| `ReservationCommitted` | `StockItem.CommitReservation` | INV-4.1 | Reporting |
| `StockAdjusted` | `StockItem.Adjust` | INV-5.3 | Reporting |
| `LowStockDetected` | `StockItem` (any change) | INV-7.1 | Reporting |
| `LowStockCleared` | `StockItem` (any change) | INV-7.2 | Reporting |
| `ReorderThresholdSet` | `StockItem.SetReorderThreshold` | INV-7.3 | Reporting |
| `StockCountStarted` | `StockCount.Start` | INV-8 | Reporting |
| `StockCountClosed` | `StockCount.Close` | INV-8.3 | Inventory itself (applies adjustments), Reporting |

Every `StockItem` event carries the resulting `OnHand` and `Reserved` so projections need no state of their own.

## Ordering

| Event | Raised by | When | Integration consumers |
| --- | --- | --- | --- |
| `OrderCreated` | `Order.Create` | ORD-1.1 | Reporting |
| `OrderPlaced` | `Order.Place` | ORD-2.1 | Inventory, Ordering (saga starts), Reporting |
| `OrderPaid` | `Order.MarkPaid` | ORD-3.4 | Reporting |
| `OrderShipped` | `Order.MarkShipped` | ORD-3.4 | Reporting |
| `OrderCancelled` | `Order.Cancel` | ORD-4.1 | Inventory, Fulfillment, Ordering (saga), Reporting |
| `PaymentRequested` | `OrderFulfillmentProcess` | SAGA-1.1 | Billing, Reporting |
| `FulfillmentRequested` | `OrderFulfillmentProcess` | SAGA-1.2 | Fulfillment, Inventory (confirm), Reporting |
| `RefundRequested` | `OrderFulfillmentProcess` | SAGA-4.2, SAGA-5.1, SAGA-6.3 | Billing, Reporting |
| `OrderFulfillmentStepChanged` | `OrderFulfillmentProcess` | every step change | Reporting |

Draft edits (`OrderLineAdded`, `OrderLineChanged`, `OrderLineRemoved`, `ShippingAddressSet`) are *internal* domain events and are not published.

**Internal commands** (Ordering only, carried by the outbox, ADR-0007): `MarkOrderPaid`, `MarkOrderShipped`, `CancelOrder(reason)`.

## Fulfillment

| Event | Raised by | When | Integration consumers |
| --- | --- | --- | --- |
| `PickListCreated` | `PickList.Create` | FUL-1.1 | Reporting |
| `PickCompleted` | `PickList.CompletePick` | FUL-2 (one pick) | *internal* |
| `PickListCompleted` | `PickList.Complete` | FUL-2.1 | Reporting |
| `ShipmentDispatched` | `PickList.Dispatch` | FUL-3.1 | Ordering (saga), Inventory (commit), Reporting |
| `PickListCancelled` | `PickList.Cancel` | FUL-4.1 | Reporting |

## Catalog

| Event | Raised by | When | Integration consumers |
| --- | --- | --- | --- |
| `ProductChanged` | `ProductService` (create/update) | CAT-2.1 | Ordering (product copy), Reporting |

## Billing

| Event | Raised by | When | Integration consumers |
| --- | --- | --- | --- |
| `PaymentCaptured` | `Payment.Captured` (factory) | BIL-1.1 | Ordering (saga), Reporting |
| `PaymentFailed` | `Payment.Failed` (factory) | BIL-1.2 | Ordering (saga), Reporting |
| `RefundFlagged` | `Payment.FlagForRefund` | BIL-3.1 | Reporting |

## Subscription matrix

| Integration event | Inventory | Ordering | Fulfillment | Billing | Reporting |
| --- | :-: | :-: | :-: | :-: | :-: |
| ProductChanged | | ✔ copy | | | ✔ |
| OrderPlaced | ✔ reserve | ✔ saga start | | | ✔ |
| StockReserved | | ✔ saga | | | ✔ |
| ReservationFailed | | ✔ saga | | | ✔ |
| ReservationExpired | | ✔ saga | | | ✔ |
| PaymentRequested | | | | ✔ charge | ✔ |
| PaymentCaptured | | ✔ saga | | | ✔ |
| PaymentFailed | | ✔ saga | | | ✔ |
| FulfillmentRequested | ✔ confirm | | ✔ pick list | | ✔ |
| ShipmentDispatched | ✔ commit | ✔ saga | | | ✔ |
| OrderCancelled | ✔ release | ✔ saga | ✔ cancel | | ✔ |
| RefundRequested | | | | ✔ flag | ✔ |
| StockCountClosed | ✔ adjust | | | | ✔ |
| everything else | | | | | ✔ |
