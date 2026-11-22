# ADR-0018: Inbox idempotency, retry and dead letters

**Status:** Accepted

**Decision.**
- Each consuming module has `<prefix>_Inbox(MessageId, HandlerName, ProcessedAt)` with a unique key on (MessageId, HandlerName). A handler that changes one aggregate inserts its inbox row in the same transaction; if the row exists, it skips (NFR-5).
- Handlers that span several transactions (for example reserving each line) rely on natural-key idempotency in the domain (`Reserve` by order line, `Release`/`Commit` no-ops) and write the inbox row last.
- The saga also keeps `ProcessedMessageIds` so duplicates show in its history (SAGA-6).
- API idempotency: the `Idempotency-Key` header is stored in `api_IdempotencyKeys(Key, RequestHash, StatusCode, ResponseBody, CreatedAt)`. A replay returns the stored response; a mismatched hash returns 422 (API-6).
- Failure: exponential backoff (1, 2, 4, 8, 16 s). After `Messaging:MaxAttempts` (5) the message is dead-lettered and listed at `GET /admin/dead-letters` (NFR-11).

**Defense in one breath.** Delivery is at-least-once, so every handler is safe to run twice; when something keeps failing we stop, park it, and make it visible.
