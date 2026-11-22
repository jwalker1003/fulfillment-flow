# Definition of Done

A story is done only when **every** box is true.

## Tests
- [ ] Failing tests were written first from the story's requirement IDs (visible in commit history: tests commit before implementation commit).
- [ ] Every requirement sub-ID in the story has at least one passing test tagged `[Trait("Req", …)]`.
- [ ] `dotnet test` passes for the **whole** solution, including architecture tests.
- [ ] No skipped or commented-out tests.

## Code
- [ ] `dotnet build -warnaserror` passes with zero warnings.
- [ ] Names match `docs/02-domain/glossary.md`; any new term was added to the glossary in the same change.
- [ ] No public setters on entities/aggregates; no domain reference to infrastructure.
- [ ] Rule violations return `Result` with a code that exists in `docs/04-contracts/error-catalog.md` (new codes added there in the same change).
- [ ] New integration events match `docs/04-contracts/integration-events.md` (or the doc was updated in the same change, with a note in the PR).
- [ ] No TODOs without a linked story ID.

## Docs
- [ ] `docs/05-delivery/traceability.md` Tests column updated.
- [ ] If a design decision was made or changed, an ADR was added/updated.
- [ ] If the implementation revealed a requirement gap, it is in `docs/01-product/open-questions.md` and Brad is told in the PR description.

## Review
- [ ] PR description lists: requirement IDs covered, tests added, any deviation from the story and why.
- [ ] Phase-gate stories: Brad has reviewed the tests and signed off before the next phase starts.
