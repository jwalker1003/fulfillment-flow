# P9-01 Defense pass

**Phase:** 9 · **Depends on:** all · **Requirements:** NFR-9, all gates

## Goal
Everything is explainable and provable.

## Tasks
1. Every ADR has all sections filled; Proposed ADRs are Accepted or revised by Brad.
2. `traceability.md`: every requirement sub-ID maps to at least one passing test; generate it with a small script that reads `[Trait("Req", …)]` from test assemblies and fails if any sub-ID in `requirements.md` is missing.
3. Architecture tests green; domain suite runs < 10 s (NFR-6).
4. Root `README.md`: what this is, how to run, how to test, link to docs index and demo script.
5. "Likely follow-up questions" from the original spec answered in `docs/03-architecture/defense-notes.md` with links to code.
6. Brad rehearses each ADR's "defense in one breath" aloud.

## Done when
All of the above, plus the definition of done.
