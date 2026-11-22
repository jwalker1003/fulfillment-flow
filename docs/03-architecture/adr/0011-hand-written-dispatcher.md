# ADR-0011: Hand-written dispatcher, no MediatR

**Status:** Accepted

**Decision.** `ICommandDispatcher` and `InProcessMessageBus` are small hand-written classes that resolve handlers from DI (`ICommandHandler<TCommand>`, `IIntegrationEventHandler<TEvent>`). No MediatR, no MassTransit.

**Alternatives.** MediatR (commercially licensed from v13); MassTransit's in-memory transport.

**Consequences.** About 150 lines of plumbing we own and can explain; no hidden pipeline behaviour.

**Defense in one breath.** Few moving parts, no licensing questions, and it shows the mechanism rather than hiding it.
