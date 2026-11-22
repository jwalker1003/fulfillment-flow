# ADR-0014: `Result` type and error codes for rule violations

**Status:** Proposed

**Context.** Rule violations are expected outcomes ("rejected and nothing changes"), and the API must name the rule (API-3).

**Decision.** Domain and application methods return `Result` / `Result<T>` from SharedKernel. A failure carries `DomainError(string Code, string Message)`. Codes look like `<context>.<kebab-rule>` (for example `inventory.insufficient-stock`) and are listed in `docs/04-contracts/error-catalog.md` with HTTP status and requirement ID. Exceptions are reserved for programming errors and infrastructure faults. Value-object factories return `Result<T>` (`Sku.Create`), with a throwing `Sku.From` for tests and trusted data.

**Alternatives.** Domain exceptions mapped by middleware (works, but uses exceptions for control flow and hides failure paths from signatures).

**Consequences.** Callers must check results. A failed method must leave the aggregate unchanged and raise no events; tests assert this.

**Defense in one breath.** Rejection is a normal outcome of a business rule, so it's in the return type, and the code ties every API error back to a requirement.
