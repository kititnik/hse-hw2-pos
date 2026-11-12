namespace Pos.Domain.Grading;

public interface IGradingPolicy
{
    decimal? Compute(IReadOnlyList<CriterionScore> scores);
}
