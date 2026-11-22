# P6-03 Billing stub and anti-corruption layer

**Phase:** 6 · **Depends on:** P4-03 · **Requirements:** BIL-1.1, BIL-1.2, BIL-1.3, BIL-2.1, BIL-3.1, BIL-3.2 — Phase 6 gate (with P6-01, P6-02)

## Goal
Billing charges a fake provider through an ACL and reports outcomes; refund requests flag payments.

## Read first
- `docs/02-domain/contexts/billing.md`

## Scope
`Billing.Domain`, `Billing.Application`, `Billing.Infrastructure` (incl. `FakePay/`), `tests/FulfillmentFlow.Billing.Tests/`, IntegrationTests

## Tasks (test-first)
1. Domain: `Payment` (`Captured`/`Failed` factories, `FlagForRefund`), IDs, events.
2. Application: `IPaymentGateway`, `PaymentOutcome`, `IPaymentRepository`, handlers `ChargeForOrder` (PaymentRequested) and `FlagRefund` (RefundRequested), mapper.
3. Infrastructure: `FakePayClient`, `FakePayChargeResponse` (snake_case JSON shape), `FakePayTranslator`, `FakePayPaymentGateway`, `BillingDbContext` (unique `OrderId`), migration, `AddBillingModule`, option `Billing:FailWhenAmountEquals`.
4. Translator unit tests: APPROVED → success with reference; DECLINED → failure with decline code as reason; unknown status → failure "unrecognised provider status".
5. Remove the `FakeBilling` test helper from P5-06; saga end-to-end tests now use real Billing (amount 13.13 triggers failure).

## Acceptance
Listed IDs pass; BIL-2.1 architecture test green; saga end-to-end suite green with real Billing.

## Done when
Standard. **Phase 6 gate** reviewed by Brad.
