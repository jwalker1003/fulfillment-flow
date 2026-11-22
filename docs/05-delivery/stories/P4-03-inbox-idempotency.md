# P4-03 Inbox idempotency, retry and dead letters

**Phase:** 4 · **Depends on:** P4-02 · **Requirements:** NFR-5, NFR-11, ADR-0018

## Goal
Every handler can safely receive a message twice, failures retry with backoff, and poison messages are parked.

## Read first
- `docs/03-architecture/adr/0018-idempotency-inbox-retry.md`
- `docs/03-architecture/architecture.md` — Messaging

## Scope
BuildingBlocks.Infrastructure, module Infrastructure projects, IntegrationTests

## Tasks (test-first)
1. `InboxMessage(MessageId, HandlerName, ProcessedAt)` table per consuming module; unique key.
2. `IdempotentHandler<TEvent>` decorator (or base class) for single-aggregate handlers: skip if inbox row exists; otherwise run the handler and add the inbox row in the same `SaveChanges`.
3. Multi-transaction handlers (e.g. `ReserveStockForOrder`) write the inbox row after their last step; they are already idempotent by natural keys.
4. Per-handler delivery: the processor invokes each handler separately; a failure in one handler records the error on the outbox row and schedules a retry for that message, while handlers that already succeeded skip via their inbox rows on retry.
5. Backoff 1, 2, 4, 8, 16 s; after `Messaging:MaxAttempts` → `DeadLetteredAt` set.
6. `IDeadLetterQuery` returning dead letters across modules (exposed by API in P7).

## Acceptance
- Test: delivering the same `OrderPlaced` message twice through the processor results in one set of reservations and one inbox row per handler.
- Test: a handler that throws 5 times dead-letters the message; other handlers for that message still processed once.
- Test: a handler that throws once then succeeds is processed exactly once.

## Out of scope
API idempotency keys (P7-05).

## Done when
Standard.
