# P3-04 Application building blocks and in-memory bus

**Phase:** 3 · **Depends on:** P1-02 · **Requirements:** NFR-2, NFR-3 (mechanism), ADR-0011, ORD-1.5 (application check)

## Goal
Commands, handlers, integration-event handlers, a clock, and an in-memory message bus exist, so modules can talk through events before there is a database. Ordering's draft/place commands work against in-memory repositories.

## Read first
- `docs/03-architecture/architecture.md` — Messaging, Time
- `docs/03-architecture/adr/0011-hand-written-dispatcher.md`
- `docs/04-contracts/integration-events.md` — Ordering section

## Scope
`FulfillmentFlow.BuildingBlocks.Application`, `FulfillmentFlow.Ordering.Contracts`, `FulfillmentFlow.Ordering.Application`, `tests/FulfillmentFlow.Application.Tests/`

## Tasks (test-first)
1. BuildingBlocks.Application: `ICommand`, `ICommandHandler<TCommand>` (`Task<Result> Handle(TCommand, CancellationToken)`; plus `ICommandHandler<TCommand,TResult>`), `ICommandDispatcher`, `IIntegrationEventHandler<TEvent>` (`Task Handle(TEvent, CancellationToken)`), `IMessageBus.Publish(IIntegrationEvent)`, `IClock`, `IIntegrationEventMapper` (domain event → integration event or null).
2. `CommandDispatcher` resolving from `IServiceProvider`.
3. `InMemoryMessageBus` for tests: queues published events; `DrainAsync()` delivers to every registered handler until the queue is empty.
4. Ordering.Contracts: all Ordering integration events from the contracts doc.
5. Ordering.Application: `IOrderRepository`, `IPriceListReader` (implements domain `IPriceList` lookup + "is SKU valid"), commands `CreateOrder`, `AddOrderLine` (ORD-1.5 → `ordering.unknown-sku`), `ChangeOrderLineQuantity`, `RemoveOrderLine`, `SetShippingAddress`, `PlaceOrder`, `CancelOrder`. `OrderingIntegrationEventMapper`.
6. In-memory repository implementations in the test project; their `Save` maps domain events and publishes them to `InMemoryMessageBus` (stand-in for the outbox).

## Acceptance
- Application tests: create → add line → set address → place publishes exactly one `OrderPlacedIntegrationEvent` with correct data.
- ORD-1.5 test passes at application level.
- Ordering.Application has no reference to EF Core or ASP.NET (architecture test still green).

## Out of scope
Persistence, outbox, saga.

## Done when
Standard.
