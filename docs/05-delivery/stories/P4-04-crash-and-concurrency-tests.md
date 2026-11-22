# P4-04 Crash and concurrency tests

**Phase:** 4 · **Depends on:** P4-03 · **Requirements:** NFR-4, NFR-5, NFR-10 — Phase 4 gate

## Goal
Prove, with tests, that events survive a crash and concurrent reservations never oversell.

## Read first
- `docs/06-quality/test-strategy.md` — Integration tests

## Scope
IntegrationTests only (plus test hooks in BuildingBlocks.Infrastructure if needed, behind an interface).

## Tasks
1. **Crash after save, before publish:** save an aggregate, dispose the processor without draining (simulated crash), start a new host on the same database file, drain → the event is delivered exactly once.
2. **Crash during handling:** handler throws after making its change but before commit (inject a fault via a test-only `IFaultInjector`) → retry delivers once; state correct.
3. **Crash after handling, before marking processed:** commit handler transaction, then throw before `ProcessedAt` is set → redelivery is skipped by the inbox.
4. **Concurrency:** 20 parallel reservations of 1 unit against a stock item with 10 available (separate DbContexts, file-based SQLite) with retry-on-conflict → exactly 10 succeed, available = 0, never negative.

## Acceptance
All four scenarios pass reliably (run 10× locally without flakiness).

## Out of scope
New production features.

## Done when
Standard. **Phase 4 gate** reviewed by Brad.
