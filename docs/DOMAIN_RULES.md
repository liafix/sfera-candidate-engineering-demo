# Domain rules — Candidate Demo V1

This document describes only the deterministic rules implemented in the candidate demo.
It is **not** a representation of SFÉRA's internal product-selection logic.

## Safety boundary

- The demo uses synthetic inputs.
- The demo implements only one named product pathway: `XMtrade / ETRM`.
- The pathway is used only as a candidate-project example based on publicly described product capabilities.
- Every named product result has `RequiresExpertReview = true`.
- Unsupported combinations return `EXPERT_REVIEW` instead of inventing a product match.
- No generative AI participates in product selection or ROI calculation.

## Ruleset

Current version:

`candidate-demo-2026.08-v1`

### ETRM demo rule

A suggested ETRM pathway is returned when:

1. participant type is `TraderOrSupplier`, and
2. either wholesale-contract management or trading/planning support is selected.

The score is an explainable **demo fit score**, not a probability, confidence level, quote, or forecast.

### Score weights

- Trader/supplier: +35
- Trading and supply as primary need: +10
- Wholesale contract management: +40
- Trading/planning support: +10
- Maximum score: 95

The score intentionally never reaches 100 because the candidate demo cannot replace specialist validation.

## Unsupported combinations

Any combination outside the explicitly implemented demo rule returns:

- product code: `EXPERT_REVIEW`
- display name: `Expert consultation required`
- status: `ExpertReviewRequired`
- `RequiresExpertReview = true`

This is a fail-safe behavior: the system prefers no recommendation over an invented recommendation.

## ROI model

Inputs:

- cases per month
- minutes saved per case
- loaded hourly cost
- annual operating cost
- implementation cost

Formulas:

```text
casesPerYear = casesPerMonth × 12
annualHoursSaved = casesPerYear × minutesSavedPerCase / 60
annualTimeValue = annualHoursSaved × loadedHourlyCost
annualNetBenefit = annualTimeValue - annualOperatingCost
simplePaybackMonths = implementationCost / annualNetBenefit × 12
```

If `annualNetBenefit <= 0`, payback is `null` / not reached.

All returned values are rounded to two decimal places using midpoint-away-from-zero rounding.
