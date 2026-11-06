# Contributing to FreelanceFlow

## Branching

1. Start from the latest `develop` branch.
2. Create one branch per task: `feature/<short-description>`, `fix/<short-description>`, or `docs/<short-description>`.
3. Keep each branch focused; avoid unrelated changes.
4. Push your branch and open a Pull Request into `develop`.
5. Do not push directly to `main` or merge your own Pull Request without Team Lead approval.

## Commit messages

Use short, imperative messages with a clear scope, for example:

- `feat: add client creation`
- `fix: validate invoice payment balance`
- `test: cover project progress calculation`
- `docs: update local setup instructions`

## Pull Request checklist

- [ ] The change matches an approved requirement or issue.
- [ ] The solution builds successfully.
- [ ] Relevant tests pass.
- [ ] No secrets, connection strings, passwords, or local files are committed.
- [ ] Database changes and migration impact are explained.
- [ ] The PR has a clear description and screenshots for UI changes.

## Collaboration rules

- Pull the latest `develop` before starting a task.
- Do not edit another member's files without coordination.
- Do not create or apply EF Core migrations independently; coordinate schema changes with the Team Lead.
- Resolve conflicts locally and explain non-trivial resolutions in the PR.
