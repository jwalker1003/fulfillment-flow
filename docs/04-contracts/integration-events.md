# Integration Event Contracts

These records live in each context's `*.Contracts` project. They are the **only** types other contexts may reference (NFR-2).

## Rules

1. `public sealed record`, positional, immutable. Primitive and BCL types only (`Guid`, `string`, `int`, `decimal`, `double`, `DateTimeOffset`, `bool`, `IReadOnlyList<T>` of contract-local records). No SharedKernel types.
2. Every event implements `IIntegrationEvent` from `FulfillmentFlow.BuildingBlocks.Contracts`, a tiny project (BCL only) that holds nothing but `public interface IIntegrationEvent { Guid MessageId { get; } DateTimeOffset OccurredAt { get; } }`. Each record has `Guid MessageId` and `DateTimeOffset OccurredAt` as its first two parameters, which satisfy the interface. (The examples below omit `: IIntegrationEvent` for brevity.)
3. Name = domain event name + `IntegrationEvent`. Namespace `FulfillmentFlow.<Context>.Contracts`.
4. Versioning: additive changes only (new optional parameter at the end with a default). A breaking change means a new type `…IntegrationEventV2`, with the old one still published until no consumer needs it.
5. Serialised as JSON (System.Text.Json, camelCase) in the outbox. The outbox `Type` column holds the full type name.
6. Money is `decimal` (USD). Quantities are `int`. SKUs and location IDs are `string`. Order/line/reservation IDs are `Guid`.

## Inventory (`FulfillmentFlow.Inventory.Contracts`)

```csharp
public sealed record StockReceivedIntegrationEvent(
    Guid MessageId, DateTimeOffset OccurredAt,
    string Sku, string LocationId, int QuantityReceived, int OnHand, int Reserved);

public sealed record StockReservedIntegrationEvent(
    Guid MessageId, DateTimeOffset OccurredAt,
    Guid OrderId, Guid OrderLineId, Guid ReservationId,
    string Sku, string LocationId, int Quantity, DateTimeOffset ExpiresAt,
    int OnHand, int Reserved);

public sealed record ReservationFailedIntegrationEvent(
    Guid MessageId, DateTimeOffset OccurredAt,
    Guid OrderId, Guid OrderLineId, string Sku, int Quantity,
    string ReasonCode);            // e.g. "inventory.no-location-can-supply"

public sealed record ReservationReleasedIntegrationEvent(
    Guid MessageId, DateTimeOffset OccurredAt,
    Guid OrderId, Guid OrderLineId, Guid ReservationId,
    string Sku, string LocationId, int Quantity, int OnHand, int Reserved);

public sealed record ReservationExpiredIntegrationEvent(
    Guid MessageId, DateTimeOffset OccurredAt,
    Guid OrderId, Guid OrderLineId, Guid ReservationId,
    string Sku, string LocationId, int Quantity, int OnHand, int Reserved);

public sealed record ReservationCommittedIntegrationEvent(
    Guid MessageId, DateTimeOffset OccurredAt,
    Guid OrderId, Guid ReservationId,
    string Sku, string LocationId, int Quantity, int OnHand, int Reserved);

public sealed record StockAdjustedIntegrationEvent(
    Guid MessageId, DateTimeOffset OccurredAt,
    string Sku, string LocationId, int OldOnHand, int NewOnHand, string Reason, int Reserved);

public sealed record LowStockDetectedIntegrationEvent(
    Guid MessageId, DateTimeOffset OccurredAt,
    string Sku, string LocationId, int Available, int ReorderThreshold);

public sealed record LowStockClearedIntegrationEvent(
    Guid MessageId, DateTimeOffset OccurredAt,
    string Sku, string LocationId, int Available, int ReorderThreshold);

public sealed record ReorderThresholdSetIntegrationEvent(
    Guid MessageId, DateTimeOffset OccurredAt,
    string Sku, string LocationId, int? ReorderThreshold);

public sealed record StockCountStartedIntegrationEvent(
    Guid MessageId, DateTimeOffset OccurredAt, Guid StockCountId, string LocationId);

public sealed record StockCountClosedIntegrationEvent(
    Guid MessageId, DateTimeOffset OccurredAt, Guid StockCountId, string LocationId,
    IReadOnlyList<ProposedAdjustment> Adjustments);

public sealed record ProposedAdjustment(string Sku, int Expected, int Counted, int QuantityChange, string Reason);
```

## Ordering (`FulfillmentFlow.Ordering.Contracts`)

```csharp
public sealed record OrderCreatedIntegrationEvent(
    Guid MessageId, DateTimeOffset OccurredAt, Guid OrderId, string CustomerReference);

public sealed record OrderPlacedIntegrationEvent(
    Guid MessageId, DateTimeOffset OccurredAt,
    Guid OrderId, string CustomerReference,
    IReadOnlyList<PlacedOrderLine> Lines, decimal Total,
    double ShipToLatitude, double ShipToLongitude);

public sealed record PlacedOrderLine(Guid OrderLineId, string Sku, int Quantity, decimal UnitPrice);

public sealed record OrderPaidIntegrationEvent(Guid MessageId, DateTimeOffset OccurredAt, Guid OrderId);

public sealed record OrderShippedIntegrationEvent(Guid MessageId, DateTimeOffset OccurredAt, Guid OrderId);

public sealed record OrderCancelledIntegrationEvent(
    Guid MessageId, DateTimeOffset OccurredAt, Guid OrderId, string Reason, string PreviousStatus);

public sealed record PaymentRequestedIntegrationEvent(
    Guid MessageId, DateTimeOffset OccurredAt, Guid OrderId, decimal Amount);

public sealed record FulfillmentRequestedIntegrationEvent(
    Guid MessageId, DateTimeOffset OccurredAt, Guid OrderId, IReadOnlyList<PickInstruction> Picks);

public sealed record PickInstruction(Guid OrderLineId, string Sku, string LocationId, int Quantity, Guid ReservationId);

public sealed record RefundRequestedIntegrationEvent(
    Guid MessageId, DateTimeOffset OccurredAt, Guid OrderId, decimal Amount, string Reason);

public sealed record OrderFulfillmentStepChangedIntegrationEvent(
    Guid MessageId, DateTimeOffset OccurredAt, Guid OrderId,
    string Step, string? PreviousStep, string? EndReason);
```

Internal commands (Ordering only; in `FulfillmentFlow.Ordering.Application`, **not** in Contracts):

```csharp
internal sealed record MarkOrderPaid(Guid OrderId);
internal sealed record MarkOrderShipped(Guid OrderId);
internal sealed record CancelOrder(Guid OrderId, string Reason);
```

## Fulfillment (`FulfillmentFlow.Fulfillment.Contracts`)

```csharp
public sealed record PickListCreatedIntegrationEvent(
    Guid MessageId, DateTimeOffset OccurredAt, Guid PickListId, Guid OrderId, int PickCount);

public sealed record PickListCompletedIntegrationEvent(
    Guid MessageId, DateTimeOffset OccurredAt, Guid PickListId, Guid OrderId);

public sealed record ShipmentDispatchedIntegrationEvent(
    Guid MessageId, DateTimeOffset OccurredAt,
    Guid PickListId, Guid OrderId, Guid ShipmentId,
    string Carrier, string TrackingNumber, IReadOnlyList<Guid> ReservationIds);

public sealed record PickListCancelledIntegrationEvent(
    Guid MessageId, DateTimeOffset OccurredAt, Guid PickListId, Guid OrderId, string Reason);
```

## Catalog (`FulfillmentFlow.Catalog.Contracts`)

```csharp
public sealed record ProductChangedIntegrationEvent(
    Guid MessageId, DateTimeOffset OccurredAt,
    string Sku, string Name, decimal UnitPrice, decimal WeightKg,
    decimal LengthCm, decimal WidthCm, decimal HeightCm,
    bool IsActive, long Version);
```

## Billing (`FulfillmentFlow.Billing.Contracts`)

```csharp
public sealed record PaymentCapturedIntegrationEvent(
    Guid MessageId, DateTimeOffset OccurredAt, Guid OrderId, Guid PaymentId, decimal Amount);

public sealed record PaymentFailedIntegrationEvent(
    Guid MessageId, DateTimeOffset OccurredAt, Guid OrderId, decimal Amount, string Reason);

public sealed record RefundFlaggedIntegrationEvent(
    Guid MessageId, DateTimeOffset OccurredAt, Guid OrderId, Guid PaymentId, decimal Amount, string Reason);
```
