---
name: ship
description: Ship current feature — final test run, create PR to develop, update memory
disable-model-invocation: true
---

Ship the current feature for DemonsAndDogs.

## Step 1 — Load Context

Read `C:\Users\Trevor\.claude\projects\D--Repos-DemonsAndDogs\memory\current-session.md` to get active feature.

## Step 2 — Quality Gate

Run in sequence. If EITHER fails, stop and report what broke. Do NOT create the PR.

1. `dotnet build DemonsAndDogs.sln --verbosity minimal 2>&1 | tail -15`
2. `dotnet test DemonsAndDogs.sln --verbosity minimal 2>&1 | tail -30`

## Step 3 — Push and PR

```bash
git push -u origin HEAD
gh pr create --base develop --title "feat: {feature-name}" --body "$(cat <<'EOF'
## Summary
{1-3 bullet points summarizing the changes from commits and conversation}

## Test plan
{Key test cases and how to verify}

🤖 Generated with [Claude Code](https://claude.com/claude-code)
EOF
)"
```

Keep PR body concise (~15 lines max).

## Step 4 — Update Memory

- Update `project_milestone_state.md`: add feature to completed list with date
- Update `current-session.md`: clear active_feature, set next_step to "Run `/session` to pick next feature"

Report: "PR created: {url}. Run `/session` to continue."
