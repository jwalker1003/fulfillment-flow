# Open Questions and Review Log

The ten gaps found in the v1.0 spec were resolved with defaults so building can start. **Brad to review.** Change a decision by editing the ADR and the affected requirement IDs in the same change.

## Resolved in v1.1 (review pending)

| # | Gap in v1.0 | Decision | Where |
| --- | --- | --- | --- |
| 1 | FUL-1 creates the pick list on `StockReserved`; SAGA-1 creates it after payment | Pick list is created from `FulfillmentRequested`, which the saga publishes after payment is captured | FUL-1.1, ADR-0016 |
| 2 | `StockReserved` is per line; Fulfillment and the saga need "all lines reserved" | The saga counts `StockReserved` per order line; Fulfillment never sees `StockReserved` | SAGA-1.1, ADR-0015 |
| 3 | `ReservationFailed` had no producer | Raised by Inventory's `OrderPlaced` handler when the strategy or `Reserve` fails | INV-2.6, ADR-0015 |
| 4 | "Nearest warehouse" had nothing to measure from | Locations have coordinates; orders carry a shipping address with coordinates; great-circle distance | INV-6.1, ORD-1.7, ORD-2.4, ADR-0008 |
| 5 | Draft totals need prices before ORD-5 captures them | Ordering keeps a product copy from `ProductChanged`; draft lines use current price, place re-captures | ORD-1.2, ORD-1.5, ORD-5.1, ADR-0004 |
| 6 | Reservation lifetime and expiry trigger unspecified | 15 minutes (configurable); background worker every 30 s; confirmed reservations never expire | INV-2.5, INV-3.4, ADR-0017 |
| 7 | INV-7 "raised once" — when does it reset? | Re-arms when available rises above threshold; threshold is per stock item | INV-7.2, INV-7.3 |
| 8 | Refunds missing for paid-order cancellation | Saga publishes `RefundRequested`; Billing flags the payment | SAGA-4.2, SAGA-5.1, BIL-3 |
| 9 | INV-8: who approves the proposed adjustment? | Closing the count is the approval | INV-8.3 |
| 10 | Saga diagram lost in export | Redrawn in Mermaid | `docs/02-domain/saga.md` |

## Also changed

- **BIL-1 trigger.** Billing listening to `OrderPlaced` contradicted SAGA-2 ("payment is never requested"). Billing now listens to `PaymentRequested`.
- **`PickCompleted` renamed to `PickListCompleted`** to keep the ubiquitous language precise.
- **Commands to Ordering inside the saga.** To keep NFR-3, the saga writes internal commands (`MarkOrderPaid`, `MarkOrderShipped`, `CancelOrder`) to the outbox instead of changing `Order` in the same transaction (ADR-0007).
- **Dashboard calls the API over HTTP** inside the same process to honour DASH-2 literally (ADR-0019).
- **FluentAssertions replaced by Shouldly** (FluentAssertions v8 needs a commercial licence) (ADR-0013).

## Still open (sensible default in place, revisit later)

1. **Cancel vs dispatch race.** A customer can cancel a Paid order at the same moment the warehouse dispatches it. Fulfillment ignores the cancel (FUL-4.2) and the saga ignores the late `ShipmentDispatched` (SAGA-6.2), leaving a shipped order marked Cancelled. Default: accept and show it in the activity feed. Fix later by making Fulfillment publish `CancellationRejected`.
2. **Count adjustment rejected by INV-5.2.** Default: log to the activity feed, no retry. Could instead keep the count open.
3. **Late `StockReserved` after cancellation** is cleaned up only by expiry (up to 15 minutes of phantom reservation). Acceptable for a demo.
4. **Seed data.** Default: 3 locations (Seattle, Chicago, Atlanta), 6 products, stock at each location. Defined in story P8-04.
5. **Reporting module as a sixth module.** It is not a bounded context of the business, just a read side. Confirm you are happy defending it that way (ADR-0009).
