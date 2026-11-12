namespace Pos.Domain.Grading;

internal static class WeightedAverage
{
    public static decimal Compute(IReadOnlyList<CriterionScore> scores)
    {
        if (scores.Count == 0)
            throw new ArgumentException("At least one criterion score is required.", nameof(scores));

        var totalWeight = 0m;
        var weightedSum = 0m;
        foreach (var score in scores)
        {
            var weight = score.Criterion.Weight.Value;
            var value = score.Score?.Value ?? 0m;
            totalWeight += weight;
            weightedSum += weight * value / score.Criterion.MaxScore.Value;
        }

        return FinalGrade.MaxValue * weightedSum / totalWeight;
    }
}
