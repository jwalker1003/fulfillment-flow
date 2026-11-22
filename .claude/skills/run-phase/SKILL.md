---
name: run-phase
description: Run one Fulfillment Flow build phase end to end — implement each story in dependency order with fresh subagents, review, track progress, and stop at the phase gate for Brad's review.
argument-hint: <phase number, e.g. 2>
disable-model-invocation: true
---

# Run phase $ARGUMENTS

You are the **orchestrator**. You do not write production or test code yourself. You pick stories, delegate each to a fresh `story-implementer` subagent, verify, get it reviewed, and keep `docs/05-delivery/progress.md` accurate. You stop at the phase gate.

## 0. Preconditions (stop and report if any fail)
1. `git status --porcelain` is empty (clean working tree).
2. Phase $ARGUMENTS exists in `docs/05-delivery/progress.md`.
3. If $ARGUMENTS > 1, the previous phase's **Gate** line in `progress.md` reads `✅ signed off`, and its branch is merged into `main` (`git branch --merged main` lists it, or no such branch exists).
4. No story in phase $ARGUMENTS is `blocked`. If one is, report it and stop. Brad must clear it first.

## 1. Set up
1. Branch name: `phase/$ARGUMENTS-<slug>`, where slug is the phase title from `progress.md` in kebab case (e.g. `phase/2-inventory-domain`).
   - If it exists: `git switch` to it (resuming a run).
   - Else: `git switch main` then `git switch -c <branch>`.
2. Create the run lock so the doc-protection hook is active: `touch .claude/factory-run.lock`.
3. Read `docs/05-delivery/roadmap.md` for the phase's story order and dependencies.

## 2. Story loop
Repeat until every story in the phase is `done`:

1. **Pick** the first story in the phase table that is `todo` or `in-progress` (resume) and whose dependencies in `roadmap.md` are all `done` in `progress.md`. If none qualifies but some are not done, mark nothing, report the dependency deadlock, and go to step 4 (stop).
2. **Mark** it `in-progress` in `progress.md`. Do not commit yet.
3. **Implement:** launch the `story-implementer` agent with the prompt:
   > Implement story `<ID>`. Follow your instructions exactly. Report back in the required format.
4. **Read its report.** If it reports `BLOCKED`, write the reason in the story's Notes, set `blocked`, commit `progress.md` (`progress: <ID> blocked`), and go to §3 Stop.
5. **Verify independently** (don't trust the report): run `dotnet build -warnaserror` and `dotnet test`. Confirm `git log` shows the story's commits and `git status --porcelain` is clean. If anything fails, send the failure output to the same implementer agent (SendMessage) and ask it to fix it. That counts as a fix round.
6. **Review:** set `review` in `progress.md`, launch the `story-reviewer` agent with:
   > Review story `<ID>`: the commits since `<commit before this story>`.
   If it reports findings, send them to the same implementer agent and ask it to fix them and commit (`<ID>: address review`). Then verify again (step 5) and re-review.
   **Maximum 2 fix rounds in total** (steps 5 and 6 combined). If it still isn't clean, mark `blocked` with a one-line summary of what's outstanding, commit `progress.md`, and stop (§3).
7. **Done:** set `done`, Commit = short hash of the story's last commit, Notes = anything Brad should know (deviations, open questions, or blank). Commit `progress.md` alone: `progress: <ID> done`.
8. Tell Brad in one line: `✔ <ID> <title> — <n> tests, <commit>`. Then continue the loop.

## 3. Stop (blocked or deadlocked)
Remove the lock (`rm -f .claude/factory-run.lock`), and report: which story stopped, why, what you tried, and what Brad needs to decide. Do not attempt workarounds outside the story's scope.

## 4. Gate (all stories done)
1. Run `dotnet build -warnaserror` and `dotnet test` one final time. Collect test count and duration.
2. Check the phase's gate criteria in `roadmap.md` one by one (for example, "INV-1 … INV-8 pass": run `dotnet test --filter "Req~INV-"` and confirm every listed sub-ID has a passing test; cross-check `traceability.md`).
3. Replace the phase's Gate line in `progress.md` with:
   ```
   **Gate:** 🟡 awaiting Brad's review — <date>
   - Build: ✅ zero warnings · Tests: <passed>/<total> in <duration>
   - Gate criteria: <each criterion → ✅ / ❌ with evidence>
   - Deviations and open questions: <list, or "none">
   - Review: `git diff main...<branch>`
   ```
4. Commit `progress: phase $ARGUMENTS gate report`.
5. Remove the lock: `rm -f .claude/factory-run.lock`.
6. Report to Brad: the gate summary, the branch name, and that the next step is his review. **Do not merge, and do not start the next phase.** When Brad signs off he changes the Gate line to `✅ signed off` and merges the branch (or asks you to).

## Rules
- Never edit `docs/01-product/requirements.md` or ADRs. A hook blocks it during runs anyway. Conflicts go in Notes and stop the story.
- Never push, never force anything, never `git reset --hard`.
- One story at a time. No parallel implementers.
- Keep your own messages short; the subagents do the heavy reading.
