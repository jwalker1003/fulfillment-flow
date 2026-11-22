---
description: Implement one story from docs/05-delivery/stories by hand (one iteration of /run-phase)
argument-hint: <story id, e.g. P2-02>
---

Implement story **$ARGUMENTS** for Fulfillment Flow. This is one iteration of the `/run-phase` loop, for when you want to drive a single story yourself.

1. Check `docs/05-delivery/progress.md` and `docs/05-delivery/roadmap.md`: the story must be `todo` or `in-progress`, its dependencies `done`, and the previous phase gate `✅ signed off` (unless it is a phase 1 story). If not, stop and say why.
2. Make sure you're on the phase branch (`phase/<n>-<slug>`); create it from `main` if missing. The working tree must be clean.
3. `touch .claude/factory-run.lock` (turns on doc protection).
4. Mark the story `in-progress` in `progress.md`.
5. Launch the `story-implementer` agent: "Implement story `$ARGUMENTS`. Follow your instructions exactly. Report back in the required format."
6. Verify yourself: `dotnet build -warnaserror`, `dotnet test`, clean tree, commits present.
7. Set `review`; launch the `story-reviewer` agent on the story's commits. Send findings back to the implementer, at most 2 fix rounds in total.
8. Set `done` (or `blocked` with a reason), fill in the commit hash and notes, and commit `progress.md` (`progress: $ARGUMENTS done`).
9. `rm -f .claude/factory-run.lock`.
10. Report the implementer's summary to Brad in a few lines. If this story completes its phase, write the gate report as described in `/run-phase` §4 and stop.
