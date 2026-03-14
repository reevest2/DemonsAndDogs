---
name: ship
description: Ship current feature — final test run, create PR, update memory
disable-model-invocation: true
---

Ship the current feature for DemonsAndDogs.

## Step 1 — Load Context

Read `C:\Users\Trevor\.claude\projects\D--Repos-DemonsAndDogs\memory\current-session.md` to get active feature and spec.

## Step 2 — Quality Gate

Run in sequence. If EITHER fails, stop and report what broke. Do NOT create the PR.

1. `dotnet build DemonsAndDogs.sln --verbosity minimal 2>&1 | tail -15`
2. `dotnet test DemonsAndDogs.sln --verbosity minimal 2>&1 | tail -30`

## Step 3 — Push and PR

```bash
git push -u origin HEAD
gh pr create --title "feat: {feature-name}" --body "$(cat <<'EOF'
## Summary
{1-3 bullet points from spec}

## Test plan
{Key test cases from spec}

🤖 Generated with [Claude Code](https://claude.com/claude-code)
EOF
)"
```

Keep PR body concise (~15 lines max). Summarize the spec, don't paste it verbatim.

## Step 4 — Update Memory

- Update `project_milestone_state.md`: add feature to completed list with date
- Update `current-session.md`: clear active_feature, set next_step to "Run `/session` to pick next feature"

Report: "PR created: {url}. Run `/session` to continue."
