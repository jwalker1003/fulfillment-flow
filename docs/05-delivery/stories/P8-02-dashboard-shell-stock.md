# P8-02 Dashboard shell and stock views

**Phase:** 8 · **Depends on:** P8-01 · **Requirements:** DASH-1, DASH-2

## Goal
A Blazor Server dashboard hosted in the API process shows stock levels and low stock, and can receive and adjust stock through the API.

## Read first
- `docs/03-architecture/adr/0019-dashboard-hosting.md`
- `docs/03-architecture/architecture.md` — Dashboard

## Scope
`FulfillmentFlow.Dashboard` (RCL), Api host wiring (`AddRazorComponents().AddInteractiveServerComponents()`, map components at `/`), a bUnit-free approach: test the typed client against the API in IntegrationTests.

## Tasks
1. Layout with nav: Stock, Low stock, Orders, Shipments, Activity, Admin.
2. `FulfillmentFlowApiClient` (typed HttpClient, own DTOs, problem-details parsing into an `ApiError(code, title, detail)`).
3. Stock page: table (SKU, location, on-hand, reserved, available, threshold), filter by SKU/location, polls every 2 s. Forms: Receive stock, Adjust stock, Set threshold — each calls the API and shows success or the problem-details error.
4. Low-stock page.
5. No business logic in components (no comparisons against thresholds, no computing available — show what the API returns).

## Acceptance
- Integration test: the typed client can receive stock and read the view.
- Manual check documented in the PR: run the app, receive stock, see it within 2 s; submit an adjustment without a reason and see the 422 message.

## Done when
Standard.
