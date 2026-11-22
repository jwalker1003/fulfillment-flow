# P4-02 Outbox write and processor

**Phase:** 4 · **Depends on:** P4-01 · **Requirements:** NFR-4, DASH-3 (log retained), ADR-0006

## Goal
Aggregate changes and their integration events are saved atomically; a background processor publishes them afterward, in order.

## Read first
- `docs/03-architecture/architecture.md` — Messaging
- `docs/03-architecture/adr/0006-outbox-and-event-log.md`

## Scope
BuildingBlocks.Infrastructure (`Outbox/`, `Messaging/`), Inventory.Infrastructure, Ordering.Infrastructure, IntegrationTests

## Tasks (test-first)
1. `OutboxMessage` entity and table per module (columns per ADR-0006). JSON serialisation with type name.
2. Repository `Save`: map domain events with the module's `IIntegrationEventMapper`, add outbox rows, `SaveChanges` once, `ClearDomainEvents()`.
3. `IOutboxWriter` for application code that publishes without an aggregate (e.g. `ReservationFailed`) — writes in the current unit of work.
4. `InProcessMessageBus`: resolves all `IIntegrationEventHandler<T>` for a message type.
5. `OutboxProcessor<TDbContext>` BackgroundService: loads unprocessed, non-dead-lettered rows with `NextAttemptAt <= now` ordered by `Sequence`, dispatches, sets `ProcessedAt`. Disabled when `Messaging:Enabled=false`.
6. `IOutboxDrainer.DrainAsync()` for tests: loops all module outboxes until none have pending rows (max 100 rounds, then throws).
7. Replace the P3 in-memory publish path in integration tests with outbox + drainer.

## Acceptance
- Test: saving an aggregate writes state and outbox rows in one transaction (inspect both tables).
- Test: if `SaveChanges` throws, neither state nor outbox rows exist.
- Test: the place-order → reserve flow from P3-05 passes on SQLite with the drainer.
- Processed rows are kept.

## Out of scope
Inbox and retries (P4-03).

## Done when
Standard.
