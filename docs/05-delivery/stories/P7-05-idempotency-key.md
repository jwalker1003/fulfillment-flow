# P7-05 `Idempotency-Key`

**Phase:** 7 · **Depends on:** P7-02 · **Requirements:** API-6 — Phase 7 gate

## Goal
Retrying any command with the same `Idempotency-Key` has its effect once.

## Read first
- `docs/03-architecture/adr/0018-idempotency-inbox-retry.md` — API idempotency
- `docs/04-contracts/openapi.yaml` — `IdempotencyKey` parameter

## Scope
`FulfillmentFlow.Api/Idempotency/` (endpoint filter + `api_IdempotencyKeys` table in an `ApiDbContext`), IntegrationTests

## Tasks (test-first)
1. Endpoint filter on all POST/PUT/DELETE groups: no header → pass through; header seen with same request hash (method + path + body SHA-256) → replay stored status/body; different hash → 422 `api.idempotency-key-reused`; new → run, store response (2xx and 4xx; never 5xx).
2. Concurrent duplicate requests: unique key constraint; the loser waits/replays.

## Acceptance
- Retried `POST /orders` with the same key → one order, same ID returned.
- Retried `POST /inventory/receipts` → on-hand increased once.
- Same key, different body → 422.

## Done when
Standard. **Phase 7 gate** reviewed by Brad.
