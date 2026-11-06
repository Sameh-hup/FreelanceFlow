# FreelanceFlow

A web application that helps freelancers manage clients, projects, milestones, tasks, invoices, and payments from one place. Clients can securely view their own project progress and invoices through a read-only portal.

## Project scope

This repository contains the FreelanceFlow MVP. The approved requirements document is the source of truth for features, business rules, data model, acceptance criteria, and the four-week delivery plan.

### Technology

- ASP.NET Core MVC
- Razor Views (no JavaScript framework)
- Entity Framework Core — Code First
- SQL Server
- xUnit for pure business-logic tests

## Repository workflow

- `main`: stable, reviewed delivery branch. Do not push directly.
- `develop`: shared integration branch.
- `feature/<short-description>`: one focused task per branch, created from `develop`.
- Every feature is submitted as a Pull Request into `develop`.
- The Team Lead reviews and merges Pull Requests.
- Releases are merged from `develop` into `main` through a Pull Request.

## Getting started

The solution structure and local setup instructions will be added in the initial foundation task.

## Team

Five members. Responsibilities are defined in the approved project requirements and will be tracked through GitHub Issues.

## Scope discipline

Do not add Phase 2 features or new frameworks without an explicit team decision and requirements update.
