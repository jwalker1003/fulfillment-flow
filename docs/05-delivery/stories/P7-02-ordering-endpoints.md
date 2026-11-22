# P7-02 Ordering endpoints

**Phase:** 7 · **Depends on:** P7-01 · **Requirements:** API-1, API-2, API-3 (Ordering codes)

## Goal
All `/orders` endpoints in the OpenAPI document work over HTTP.

## Read first
- `docs/04-contracts/openapi.yaml` — Orders tag

## Scope
`FulfillmentFlow.Api/Endpoints/OrderEndpoints.cs`, Ordering.Application (query `GetOrder` returning `OrderDto`-shaped read result), IntegrationTests (`Api/Orders`)

## Tasks (test-first)
1. Endpoint group `/orders` mapping each operation to its command/query; endpoints contain no logic beyond binding and `ToHttpResult()`.
2. `GetOrder` query in Ordering.Application (reads the aggregate via repository and maps to a DTO; that is allowed for single-aggregate reads).
3. HTTP tests: create (201 + Location), add/merge lines, change/remove, set address, place, cancel, get; each error code from the Ordering section returns its documented status.

## Acceptance
API-1, API-2 tests pass; responses match OpenAPI schemas (property names and types).

## Done when
Standard.
