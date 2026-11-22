# Order Fulfillment Process (Saga)

`OrderFulfillmentProcess` is a process manager in the Ordering module (ADR-0007). One instance per placed order, keyed by `OrderId`, persisted in `ordering_FulfillmentProcesses`.

## Responsibilities

- React to integration events about its order.
- Track which order lines have a reservation.
- Publish `PaymentRequested`, `FulfillmentRequested`, `RefundRequested`, `OrderFulfillmentStepChanged`.
- Send internal commands to Ordering (`MarkOrderPaid`, `MarkOrderShipped`, `CancelOrder`) via the outbox, so `Order` changes in its own transaction (NFR-3).
- Record every message it receives (ID, type, time, outcome) in its history: applied, duplicate, or ignored (SAGA-6, SAGA-7).

It never touches Inventory, Billing or Fulfillment directly.

## State

| Field | Meaning |
| --- | --- |
| `OrderId` | Identity |
| `Step` | `AwaitingStock`, `AwaitingPayment`, `Fulfilling`, `Cancelling`, `Completed`, `Cancelled` |
| `ExpectedLines` | Order line IDs from `OrderPlaced` |
| `Reservations` | Line ID → (reservation ID, SKU, location, quantity) from `StockReserved` |
| `PaymentCaptured` | bool + amount (for refunds) |
| `EndReason` | Set when it reaches `Completed` or `Cancelled` |
| `ProcessedMessageIds` | For de-duplication (SAGA-6.1) |
| `History` | List of entries (at, message type, step before → after, note) |

## State machine

```mermaid
stateDiagram-v2
    [*] --> AwaitingStock: OrderPlaced
    AwaitingStock --> AwaitingStock: StockReserved (not all lines yet)
    AwaitingStock --> AwaitingPayment: StockReserved (all lines) / publish PaymentRequested
    AwaitingStock --> Cancelling: ReservationFailed / CancelOrder("insufficient stock")
    AwaitingStock --> Cancelling: ReservationExpired / CancelOrder("reservation expired")
    AwaitingPayment --> Fulfilling: PaymentCaptured / MarkOrderPaid + publish FulfillmentRequested
    AwaitingPayment --> Cancelling: PaymentFailed / CancelOrder("payment failed")
    AwaitingPayment --> Cancelling: ReservationExpired / CancelOrder("reservation expired")
    Fulfilling --> Completed: ShipmentDispatched / MarkOrderShipped
    Fulfilling --> Cancelling: ReservationExpired / CancelOrder("reservation expired")
    AwaitingStock --> Cancelled: OrderCancelled (customer)
    AwaitingPayment --> Cancelled: OrderCancelled (customer)
    Fulfilling --> Cancelled: OrderCancelled (customer)
    Cancelling --> Cancelled: OrderCancelled
    Completed --> [*]
    Cancelled --> [*]
```

`Cancelling` means "the saga asked Ordering to cancel and is waiting for `OrderCancelled`". The end reason is the saga's reason, or the customer's reason when the customer cancelled.

**Refund rule (one place):** whenever the process enters `Cancelled` and `PaymentCaptured` is true, it publishes `RefundRequested` exactly once. This covers SAGA-4.2, SAGA-5.1 and SAGA-6.3.

## Happy path

```mermaid
sequenceDiagram
    autonumber
    participant API
    participant ORD as Ordering (Order)
    participant SAGA as Ordering (Process)
    participant INV as Inventory
    participant BIL as Billing
    participant FUL as Fulfillment

    API->>ORD: PlaceOrder
    ORD-->>INV: OrderPlaced
    ORD-->>SAGA: OrderPlaced (start, AwaitingStock)
    loop each line
        INV->>INV: strategy picks location, StockItem.Reserve
        INV-->>SAGA: StockReserved
    end
    SAGA-->>BIL: PaymentRequested (AwaitingPayment)
    BIL-->>SAGA: PaymentCaptured
    SAGA->>ORD: MarkOrderPaid (internal command)
    SAGA-->>FUL: FulfillmentRequested (Fulfilling)
    SAGA-->>INV: FulfillmentRequested → confirm reservations
    FUL->>FUL: PickList created, picked, completed
    API->>FUL: Dispatch(carrier, tracking)
    FUL-->>SAGA: ShipmentDispatched
    FUL-->>INV: ShipmentDispatched → commit reservations
    SAGA->>ORD: MarkOrderShipped (Completed)
```

## Failure paths

```mermaid
sequenceDiagram
    autonumber
    participant ORD as Ordering (Order)
    participant SAGA as Ordering (Process)
    participant INV as Inventory
    participant BIL as Billing
    participant FUL as Fulfillment

    Note over SAGA: SAGA-2 insufficient stock
    INV-->>SAGA: ReservationFailed (line 2)
    SAGA->>ORD: CancelOrder("insufficient stock")
    ORD-->>INV: OrderCancelled → release line 1 reservation
    ORD-->>SAGA: OrderCancelled (Cancelled)
    Note over BIL: PaymentRequested never published

    Note over SAGA: SAGA-3 payment failed
    BIL-->>SAGA: PaymentFailed
    SAGA->>ORD: CancelOrder("payment failed")
    ORD-->>INV: OrderCancelled → release all
    ORD-->>SAGA: OrderCancelled (Cancelled)

    Note over SAGA: SAGA-5 reservation expired after payment
    INV-->>SAGA: ReservationExpired
    SAGA->>ORD: CancelOrder("reservation expired")
    ORD-->>INV: OrderCancelled → release remaining
    ORD-->>FUL: OrderCancelled → cancel pick list
    ORD-->>SAGA: OrderCancelled (Cancelled, payment was captured)
    SAGA-->>BIL: RefundRequested
```

## Late and duplicate messages (SAGA-6)

| Message | In a terminal step | Action |
| --- | --- | --- |
| Any message whose ID is already processed | any | History "duplicate"; no change |
| `StockReserved`, `ReservationFailed`, `ReservationExpired`, `PaymentFailed`, `ShipmentDispatched` | `Completed` / `Cancelled` | History "ignored"; no change |
| `PaymentCaptured` | `Cancelling` | Record `PaymentCaptured = true` (refund follows on entering `Cancelled`); no step change |
| `PaymentCaptured` | `Cancelled` | History "ignored" for state; publish `RefundRequested` if not already published (SAGA-6.3) |
| `StockReserved` for a line already reserved | non-terminal | History "duplicate"; no change |

## Rules for the implementation

- The process is persisted with a `Version` concurrency token. Its handler transaction writes the process row, its outbox rows, and the inbox row only.
- Methods are pure: `Handle(StockReservedIntegrationEvent e, DateTimeOffset now)` returns nothing and records outgoing messages in `PendingMessages`, which the repository moves to the outbox. This makes every saga rule a plain unit test.
- `OrderPlaced` for an order that already has a process is a duplicate.
- Messages for an order with no process (for example `OrderCancelled` from a Draft order) are ignored by the saga handler.
