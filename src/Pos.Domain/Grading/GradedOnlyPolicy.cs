namespace Pos.Domain.Grading;

public sealed class GradedOnlyPolicy : IGradingPolicy
{
    public decimal? Compute(IReadOnlyList<CriterionScore> scores)
    {
        ArgumentNullException.ThrowIfNull(scores);

        var graded = scores
            .Where(score => score.Score is not null)
            .ToList();
        if (graded.Count == 0)
            return null;

        return WeightedAverage.Compute(graded);
    }
}
