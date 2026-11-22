# P<phase>-<nn> <Title>

**Phase:** n · **Depends on:** … · **Requirements:** IDs from `docs/01-product/requirements.md`

## Goal
One or two sentences: what exists after this story that didn't before.

## Read first
Links to the exact docs/sections the implementer needs. Nothing else is required reading.

## Scope
Projects and folders this story may create or change. Anything outside is out of scope.

## Tasks (in order, test-first)
1. Write failing tests for every listed requirement ID (one or more tests per sub-ID, `[Trait("Req", "…")]`).
2. Implement until green.
3. …

## Acceptance
- Every listed requirement sub-ID has a passing test.
- Story-specific technical checks.

## Out of scope
Things a factory might be tempted to do now but must not.

## Done when
- `dotnet build -warnaserror` and `dotnet test` pass for the whole solution.
- `docs/05-delivery/traceability.md` updated with the test names.
- Definition of done (`docs/06-quality/definition-of-done.md`) satisfied.
