---
name: session
description: Resume development session — reads project state, health-checks build/tests, reports what to do next
disable-model-invocation: true
---

You are resuming a Claude-managed development session for DemonsAndDogs.

## Step 1 — Read State

Read these files directly (do NOT use subagents):
- `C:\Users\Trevor\.claude\projects\D--Repos-DemonsAndDogs\memory\current-session.md`
- `C:\Users\Trevor\.claude\projects\D--Repos-DemonsAndDogs\memory\project_milestone_state.md`

## Step 2 — Health Check

Run sequentially:
1. `dotnet build DemonsAndDogs.sln --verbosity minimal 2>&1 | tail -15`
2. `dotnet test DemonsAndDogs.sln --no-build --verbosity minimal 2>&1 | tail -25`
3. `git status --short`
4. `git log --oneline -5`

## Step 3 — Report

Output a concise status block:
- **Build:** PASS or FAIL (error summary if fail)
- **Tests:** X/Y passing (list failures if any)
- **Branch:** current branch
- **Active feature:** name or "none"
- **Next action:** one sentence — what to do next

Do NOT begin implementing anything. Wait for user direction.
