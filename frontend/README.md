# Candidate demo frontend — Phase 5

Next.js/React frontend for the **SFÉRA Energy Solution & ROI Configurator — Candidate Engineering Demo**.

> UNOFFICIAL CANDIDATE DEMO. This is an independent candidate project. It is not an official SFÉRA product or internal system.

## Stack

- Next.js 16.2.11 (Active LTS security line at implementation time)
- React 19.2.7
- TypeScript 5.9-compatible codebase
- Tailwind CSS 4.3
- direct REST/JSON calls to the ASP.NET Core candidate API

## Local configuration

Copy `.env.example` to `.env.local`:

```text
NEXT_PUBLIC_API_BASE_URL=http://localhost:5158
```

Run the backend first, then:

```bash
npm install
npm run dev
```

Frontend URL:

```text
http://localhost:3000
```

## Phase 5 workflow

1. Landing page explains scope and non-official status.
2. `Start assessment` performs `POST /api/v1/assessments`.
3. Browser routes to `/assessment/{id}`.
4. Wizard restores the persisted assessment with `GET /api/v1/assessments/{id}`.
5. Each confirmed answer uses `PUT /answers/{questionKey}`.
6. Final step calls `POST /evaluate`.
7. The persisted recommendation is rendered with rule reasons, fit-score context and the expert-review boundary.
8. The reference ROI scenario is calculated through `POST /roi`.
9. Conservative / Reference / Growth presets can be selected and recalculated.
10. Assumptions can be edited; displayed metrics remain tied to the last confirmed server response until recalculation.

## Commands

```bash
npm run typecheck
npm run lint
npm run build
```
