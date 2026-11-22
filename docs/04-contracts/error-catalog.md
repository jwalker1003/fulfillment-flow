# Error Catalog

Every rule violation has a stable code. Domain and application code return `DomainError(Code, Message)` (ADR-0014). The API maps the code to an HTTP status with this table (implemented as `src/FulfillmentFlow.Api/Errors/ErrorCatalog.cs`) and returns RFC 9457 problem details:

```json
{
  "type": "urn:fulfillment-flow:error:inventory.insufficient-stock",
  "title": "Insufficient stock",
  "status": 409,
  "detail": "Requested 3 but only 2 available for SKU ABC-1 at SEA-1.",
  "instance": "/inventory/...",
  "code": "inventory.insufficient-stock",
  "requirement": "INV-2.2",
  "traceId": "00-..."
}
```

**Status rule of thumb:** `422` = the request itself is invalid regardless of state; `409` = the request is valid but conflicts with current state; `404` = the target does not exist.

## Shared kernel

| Code | Status | Requirement | Meaning |
| --- | --- | --- | --- |
| `shared.invalid-sku` | 422 | NFR-7 | SKU format invalid |
| `shared.invalid-quantity` | 422 | NFR-7 | Negative quantity |
| `shared.invalid-money` | 422 | NFR-7 | Negative amount or more than 2 decimals |
| `shared.invalid-geo-point` | 422 | NFR-7 | Latitude/longitude out of range |

## Inventory

| Code | Status | Requirement |
| --- | --- | --- |
| `inventory.quantity-must-be-positive` | 422 | INV-1.2, INV-2 |
| `inventory.unknown-location` | 422 | INV-1.4 |
| `inventory.location-already-exists` | 409 | — |
| `inventory.stock-item-not-found` | 404 | — |
| `inventory.insufficient-stock` | 409 | INV-2.2 |
| `inventory.no-location-can-supply` | 409 | INV-6.2 |
| `inventory.reservation-not-found` | 404 | INV-3 |
| `inventory.reservation-not-active` | 409 | INV-4.2, INV-3.4 |
| `inventory.adjustment-reason-required` | 422 | INV-5.1 |
| `inventory.adjustment-must-change-quantity` | 422 | INV-5.4 |
| `inventory.on-hand-below-reserved` | 409 | INV-5.2 |
| `inventory.stock-count-not-found` | 404 | — |
| `inventory.stock-count-closed` | 409 | INV-8.2 |
| `inventory.stock-count-empty` | 409 | INV-8.5 |

## Ordering

| Code | Status | Requirement |
| --- | --- | --- |
| `ordering.order-not-found` | 404 | — |
| `ordering.invalid-customer-reference` | 422 | ORD-1.1 |
| `ordering.quantity-must-be-positive` | 422 | ORD-1.3 |
| `ordering.unknown-sku` | 422 | ORD-1.5, ORD-5.4 |
| `ordering.line-not-found` | 404 | ORD-1.6 |
| `ordering.invalid-shipping-address` | 422 | ORD-1.7 |
| `ordering.order-not-draft` | 409 | ORD-2.3 |
| `ordering.order-has-no-lines` | 409 | ORD-2.2 |
| `ordering.shipping-address-required` | 409 | ORD-2.4 |
| `ordering.invalid-status-transition` | 409 | ORD-3.2, ORD-3.3, ORD-4.2 |
| `ordering.cancellation-reason-required` | 422 | ORD-4.3 |

## Fulfillment

| Code | Status | Requirement |
| --- | --- | --- |
| `fulfillment.pick-list-not-found` | 404 | — |
| `fulfillment.pick-not-found` | 404 | FUL-2 |
| `fulfillment.pick-list-needs-picks` | 422 | FUL-1.1 (internal only) |
| `fulfillment.pick-list-not-open` | 409 | FUL-2 |
| `fulfillment.picks-incomplete` | 409 | FUL-2.2 |
| `fulfillment.pick-list-not-completed` | 409 | FUL-3.2 |
| `fulfillment.carrier-required` | 422 | FUL-3.3 |
| `fulfillment.tracking-number-required` | 422 | FUL-3.3 |

## Catalog

| Code | Status | Requirement |
| --- | --- | --- |
| `catalog.product-not-found` | 404 | — |
| `catalog.duplicate-sku` | 409 | CAT-1.2 |
| `catalog.invalid-product` | 422 | CAT-1.3 |

## API

| Code | Status | Requirement |
| --- | --- | --- |
| `api.validation-failed` | 422 | API-3 (malformed body, missing required field) |
| `api.idempotency-key-reused` | 422 | API-6 |
| `api.concurrency-conflict` | 409 | NFR-10 |
| `api.unexpected-error` | 500 | Must never occur in tests (API-3) |

Billing raises no API errors (it has no command endpoints). Codes not in this table default to `422` and log a warning; an architecture/unit test asserts every `DomainError` code constant defined in the code base appears in `ErrorCatalog`.
