# Fulfillment Flow — Documentation Index

| Need | Read |
| --- | --- |
| What we're building and the rules | [01-product/requirements.md](01-product/requirements.md) (authoritative, v1.1) |
| What changed from the original spec, what's still open | [01-product/open-questions.md](01-product/open-questions.md) |
| Names for everything | [02-domain/glossary.md](02-domain/glossary.md) |
| How the contexts relate | [02-domain/context-map.md](02-domain/context-map.md) |
| Every event, who raises it, who handles it | [02-domain/event-catalog.md](02-domain/event-catalog.md) |
| The order saga state machine | [02-domain/saga.md](02-domain/saga.md) |
| Model of each context | [02-domain/contexts/](02-domain/contexts/) — [inventory](02-domain/contexts/inventory.md), [ordering](02-domain/contexts/ordering.md), [fulfillment](02-domain/contexts/fulfillment.md), [catalog](02-domain/contexts/catalog.md), [billing](02-domain/contexts/billing.md) |
| Solution layout, references, messaging, persistence | [03-architecture/architecture.md](03-architecture/architecture.md) |
| Why each decision was made | [03-architecture/adr/](03-architecture/adr/README.md) |
| REST API contract | [04-contracts/openapi.yaml](04-contracts/openapi.yaml) |
| Integration event records | [04-contracts/integration-events.md](04-contracts/integration-events.md) |
| Error codes → HTTP status | [04-contracts/error-catalog.md](04-contracts/error-catalog.md) |
| Build order and phase gates | [05-delivery/roadmap.md](05-delivery/roadmap.md) |
| **Where the build stands** | [05-delivery/progress.md](05-delivery/progress.md) |
| How the factory runs (commands, agents, hooks) | [05-delivery/factory.md](05-delivery/factory.md) |
| Units of work for the factory | [05-delivery/stories/](05-delivery/stories/) |
| Requirement → story → test | [05-delivery/traceability.md](05-delivery/traceability.md) |
| How we test | [06-quality/test-strategy.md](06-quality/test-strategy.md) |
| When a story is done | [06-quality/definition-of-done.md](06-quality/definition-of-done.md) |
| Rules for Claude Code | [../CLAUDE.md](../CLAUDE.md) |

## How the software factory uses these docs

Run `/run-phase <n>`. It works through the phase's stories from `progress.md` and stops at the gate for Brad's review. Full details: [05-delivery/factory.md](05-delivery/factory.md).

## Precedence when docs disagree

requirements.md → ADRs → context docs → contracts → stories. Raise the conflict; don't silently choose.
