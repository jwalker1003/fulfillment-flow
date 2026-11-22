---
description: Implement one story from docs/05-delivery/stories test-first
argument-hint: <story id, e.g. P2-02>
---

Implement story **$ARGUMENTS** for Fulfillment Flow.

1. Open `docs/05-delivery/stories/$ARGUMENTS-*.md`. Check `docs/05-delivery/roadmap.md`: if any dependency is not done or the previous phase gate has not passed, stop and say so.
2. Read only the docs listed under *Read first*, plus `docs/02-domain/glossary.md` and the requirement IDs in `docs/01-product/requirements.md`.
3. Write failing tests for every requirement ID in the story, tagged `[Trait("Req", "<ID>")]`. Run them and confirm they fail for the right reason. Show Brad the test list.
4. Implement until the story's tests pass. Stay inside the story's *Scope*.
5. Run `dotnet build -warnaserror` and `dotnet test` for the whole solution.
6. Update `docs/05-delivery/traceability.md` (and glossary / error catalog / contracts if you added anything).
7. Use the `story-reviewer` agent to review the change against the story and `CLAUDE.md`, then fix what it finds.
8. Report: requirement IDs covered, tests added, deviations, open questions. If this is a phase-gate story, stop for Brad's review.
