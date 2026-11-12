using Pos.Domain.Rubrics;

namespace Pos.Domain.Evaluations;

public sealed class EvaluationFactory
{
    public Evaluation Create(Rubric rubric, Guid criterionId, decimal score, string? comment)
    {
        ArgumentNullException.ThrowIfNull(rubric);

        var criterion = rubric.GetCriterion(criterionId);
        var value = new Score(score);
        if (value.Value > criterion.MaxScore.Value)
            throw new ArgumentException(
                $"Score {score} exceeds the maximum {criterion.MaxScore.Value} of criterion '{criterion.Name}'.",
                nameof(score));

        return new Evaluation(criterion.Id, value, comment);
    }
}
