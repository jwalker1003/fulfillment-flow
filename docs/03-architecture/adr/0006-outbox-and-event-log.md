# ADR-0006: Outbox, retained as the event log

**Status:** Accepted

**Context.** Publishing after save can lose events on a crash (NFR-4). Read models must be rebuildable from events (DASH-3).

**Decision.** Each module has a `<prefix>_Outbox` table written in the same transaction as the aggregate. A background processor publishes rows afterward. Rows are marked processed and **never deleted**; together they form the event log. Rebuild replays them ordered by `OccurredAt`, then `Sequence`.

Columns: `Sequence` (autoincrement PK), `MessageId` (Guid, unique), `Kind` (Event | Command), `Type`, `Payload` (JSON), `OccurredAt`, `CorrelationId`, `ProcessedAt`, `Attempts`, `NextAttemptAt`, `LastError`, `DeadLetteredAt`.

**Alternatives.** Publish after save (loses events); event sourcing (the log *is* the state: more powerful, but a bigger topic than this demo needs).

**Consequences.** At-least-once delivery, so consumers de-duplicate (ADR-0018). The log grows forever (fine for a demo). This is not event sourcing: aggregates are still stored as state.

**Defense in one breath.** Publishing after save can lose events on a crash. The outbox makes save and publish atomic, and keeping it gives us a replayable log for free.
