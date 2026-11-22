# The Software Factory

How Claude Code builds Fulfillment Flow from these docs, and where Brad stays in control.

## Pieces

| Piece | File | Role |
| --- | --- | --- |
| Orchestrator | `.claude/skills/run-phase/SKILL.md` (`/run-phase <n>`) | Picks stories in order, delegates, verifies, tracks progress, stops at the gate |
| Single story | `.claude/commands/implement-story.md` (`/implement-story <ID>`) | One iteration of the loop, run by hand |
| Implementer | `.claude/agents/story-implementer.md` | Fresh context per story: plan → failing tests → code → green → commit |
| Reviewer | `.claude/agents/story-reviewer.md` | Checks the story against requirements, `CLAUDE.md`, glossary, scope; read-only |
| Progress note | `docs/05-delivery/progress.md` | The only status record; owned by the orchestrator |
| Permissions | `.claude/settings.json` | Pre-approves `dotnet`, local `git` and edits under `src/`, `tests/`, `docs/`; denies push, hard reset, clean |
| Build gate hook | `.claude/hooks/verify-build.ps1` (Stop, SubagentStop) | Claude can't finish while `dotnet build -warnaserror` or `dotnet test` fails (gives up after 3 tries and warns you) |
| Doc guard hook | `.claude/hooks/protect-docs.ps1` (PreToolUse) | During a run, blocks edits to `requirements.md`, the original spec and ADRs |
| Run lock | `.claude/factory-run.lock` (gitignored) | Exists only while a run is active; switches the doc guard on |

## One phase, start to finish

```
You:           /run-phase 2
Orchestrator:  preconditions (clean tree, phase 1 gate ✅, branch merged)
               create/switch branch phase/2-inventory-domain, create run lock
  loop ─────── pick next todo story whose deps are done → mark in-progress
  │            story-implementer (fresh context):
  │              read story + its Read-first docs → checklist
  │              failing tests → commit "P2-01: tests — …"
  │              code until whole suite green → traceability → commit "P2-01: implementation — …"
  │              [verify-build hook blocks finishing while red]
  │            orchestrator re-runs build + tests itself
  │            story-reviewer → findings back to implementer (≤ 2 fix rounds)
  │            mark done + commit hash → commit "progress: P2-01 done"
  └─────────── next story
               all done → final build/test → gate report in progress.md → remove lock → STOP
You:           review `git diff main...phase/2-inventory-domain`, the gate report and the tests
               change the Gate line to "✅ signed off", merge the branch into main
               /run-phase 3
```

Each story produces 2–3 commits on the phase branch (tests, implementation, sometimes review fixes) plus a one-line progress commit, so any story can be inspected or reverted alone.

## When it stops early

- **Blocked story:** can't reach green after 2 fix rounds, or the docs contradict each other. It's marked `blocked` with a reason in Notes. You decide (change a doc, split the story, or give guidance), set it back to `todo`, and run `/run-phase <n>` again; it resumes where it stopped.
- **Interrupted:** stop Claude at any time. Re-running `/run-phase <n>` resumes from `progress.md`. If a story was left half-done, check `git status` first and either commit or discard that work.
- **Stale lock:** if a run crashed, delete `.claude/factory-run.lock` yourself to edit requirements or ADRs again.

## Changing the rules

- Requirements or decisions: edit `requirements.md` or the ADRs outside a run (no lock), then update the affected stories.
- Factory behaviour: edit the skill, the agent files or `CLAUDE.md`. Changes to `.claude/settings.json` hooks take effect in a new Claude Code session.
- Permissions only for you: put them in `.claude/settings.local.json` (gitignored).

## Not yet automated (by choice)

- **Parallel stories** via worktrees: possible later for independent stories; off until the loop has proven reliable.
- **Unattended headless runs** (`claude -p "/run-phase n"`): works with the same files once you trust a phase run interactively.
- **Merging and pushing:** always you.
