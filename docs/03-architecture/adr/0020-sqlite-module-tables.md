# ADR-0020: One SQLite file, table prefix per module, version concurrency

**Status:** Accepted

**Decision.**
- One SQLite database file, in WAL mode with `busy_timeout`, so background workers can run concurrently.
- Each module has its own `DbContext`, migrations and table prefix (`inventory_`, `ordering_`, `fulfillment_`, `catalog_`, `billing_`, `reporting_`, `api_`). No module reads another module's tables.
- Optimistic concurrency uses an `int Version` concurrency token (SQLite has no `rowversion`).
- `decimal` and `DateTimeOffset` are stored as text via value converters.
- Integration tests use SQLite in-memory with one shared open connection per test.

**Alternatives.** One DbContext for all modules (couples modules); PostgreSQL schemas (more realistic, needs a server).

**Consequences.** Zero-install demo. Moving to PostgreSQL later means changing the provider and converters only.

**Defense in one breath.** Modules own their data even in one file; the prefix is our schema and the separate DbContexts are the wall.
