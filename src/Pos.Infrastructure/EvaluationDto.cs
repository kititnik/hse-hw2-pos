namespace Pos.Infrastructure;

public sealed record EvaluationDto(Guid CriterionId, decimal Score, string? Comment);
