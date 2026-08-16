using SferaCandidate.Domain.Assessments;
using SferaCandidate.Domain.Common;

namespace SferaCandidate.Application.Assessments;

internal static class AnswerValueParser
{
    public static void Validate(string questionKey, string value)
    {
        if (!QuestionKeys.Supported.Contains(questionKey))
        {
            throw new DomainValidationException($"Unsupported question key '{questionKey}'.");
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException("Answer value is required.");
        }

        switch (questionKey)
        {
            case QuestionKeys.ParticipantType:
                _ = ParseParticipantType(value);
                break;
            case QuestionKeys.PrimaryNeed:
                _ = ParseNeedCategory(value);
                break;
            case QuestionKeys.ManagesWholesaleContracts:
            case QuestionKeys.NeedsTradingOrPlanningSupport:
                _ = ParseBoolean(value);
                break;
            case QuestionKeys.OrganizationName:
                _ = ValidateOrganizationName(value);
                break;
        }
    }

    public static ParticipantType ParseParticipantType(string value) =>
        Normalize(value) switch
        {
            "trader_or_supplier" or "traderorsupplier" => ParticipantType.TraderOrSupplier,
            "distribution_operator" or "distributionoperator" => ParticipantType.DistributionOperator,
            "market_operator" or "marketoperator" => ParticipantType.MarketOperator,
            "industrial_consumer" or "industrialconsumer" => ParticipantType.IndustrialConsumer,
            "other" => ParticipantType.Other,
            _ => throw new DomainValidationException(
                "Participant type must be one of: trader_or_supplier, distribution_operator, market_operator, industrial_consumer, other.")
        };

    public static NeedCategory ParseNeedCategory(string value) =>
        Normalize(value) switch
        {
            "trading_and_supply" or "tradingandsupply" => NeedCategory.TradingAndSupply,
            "distribution" => NeedCategory.Distribution,
            "market_operations" or "marketoperations" => NeedCategory.MarketOperations,
            "compliance_reporting" or "compliancereporting" => NeedCategory.ComplianceReporting,
            "other" => NeedCategory.Other,
            _ => throw new DomainValidationException(
                "Primary need must be one of: trading_and_supply, distribution, market_operations, compliance_reporting, other.")
        };

    public static bool ParseBoolean(string value)
    {
        if (bool.TryParse(value.Trim(), out var parsed))
        {
            return parsed;
        }

        throw new DomainValidationException("Boolean answer must be either 'true' or 'false'.");
    }

    private static string ValidateOrganizationName(string value)
    {
        var trimmed = value.Trim();

        if (trimmed.Length > 200)
        {
            throw new DomainValidationException("Organization name cannot exceed 200 characters.");
        }

        return trimmed;
    }

    private static string Normalize(string value) =>
        value.Trim().Replace("-", "_", StringComparison.Ordinal).ToLowerInvariant();
}
