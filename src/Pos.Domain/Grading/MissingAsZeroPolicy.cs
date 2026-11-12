namespace Pos.Domain.Grading;

public sealed class MissingAsZeroPolicy : IGradingPolicy
{
    public decimal? Compute(IReadOnlyList<CriterionScore> scores)
    {
        ArgumentNullException.ThrowIfNull(scores);
        if (scores.Count == 0)
            return null;

        return WeightedAverage.Compute(scores);
    }
}
