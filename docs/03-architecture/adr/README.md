# Architecture Decision Records

Format: Context → Decision → Alternatives → Consequences → Defense in one breath. `Proposed` means the ADR resolves a gap in the v1.0 spec and awaits Brad's review (see `docs/01-product/open-questions.md`). Build against Proposed ADRs as if accepted.

| ADR | Title | Status |
| --- | --- | --- |
| [0001](0001-modular-monolith.md) | Modular monolith | Accepted |
| [0002](0002-core-subdomains.md) | Inventory and Fulfillment are core | Accepted |
| [0003](0003-stockitem-holds-reservations.md) | `StockItem` holds its reservations | Accepted |
| [0004](0004-sku-reference-and-local-copies.md) | SKU reference only; local copies; minimal shared kernel | Accepted |
| [0005](0005-events-between-contexts.md) | Events with eventual consistency between contexts | Accepted |
| [0006](0006-outbox-and-event-log.md) | Outbox, retained as the event log | Accepted |
| [0007](0007-saga-process-manager.md) | Saga as a process manager in Ordering | Proposed |
| [0008](0008-allocation-strategy.md) | Allocation strategy behind an interface; nearest by coordinates | Proposed |
| [0009](0009-read-models-reporting-module.md) | Separate read models in a Reporting module | Accepted |
| [0010](0010-catalog-crud-billing-stub.md) | Catalog as CRUD, Billing as a stub with ACL | Accepted |
| [0011](0011-hand-written-dispatcher.md) | Hand-written dispatcher, no MediatR | Accepted |
| [0012](0012-plain-domain-tests.md) | Plain unit tests, no mocks, for the domain | Accepted |
| [0013](0013-technology-stack.md) | Technology stack | Accepted |
| [0014](0014-result-type-for-rule-violations.md) | `Result` type and error codes for rule violations | Proposed |
| [0015](0015-reservation-and-payment-sequence.md) | Per-line reservation on `OrderPlaced`; payment only after all lines reserved | Proposed |
| [0016](0016-pick-list-after-payment.md) | Pick list created from `FulfillmentRequested` after payment | Proposed |
| [0017](0017-reservation-expiry.md) | Reservation lifetime, expiry worker, confirmation | Proposed |
| [0018](0018-idempotency-inbox-retry.md) | Inbox idempotency, retry and dead letters | Accepted |
| [0019](0019-dashboard-hosting.md) | Dashboard hosted in-process, calls API over HTTP | Proposed |
| [0020](0020-sqlite-module-tables.md) | One SQLite file, table prefix per module, version concurrency | Accepted |
