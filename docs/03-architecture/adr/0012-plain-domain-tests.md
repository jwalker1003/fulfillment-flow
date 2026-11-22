# ADR-0012: Plain unit tests, no mocks, for the domain

**Status:** Accepted

**Decision.** Domain tests construct aggregates, call methods, and assert on state, the returned `Result`, and raised domain events. No domain test project references a mocking library (architecture test). Time is passed in. Application tests use hand-written in-memory repositories, not mocks.

**Alternatives.** Mock-heavy tests.

**Consequences.** Fast suites (NFR-6); tests read like the acceptance criteria.

**Defense in one breath.** Rules are pure logic. If testing needs mocks, the rule is probably in the wrong layer.
