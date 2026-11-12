using Pos.Domain.Rubrics;
using Pos.Domain.Students;

namespace Pos.Domain.Grading;

public sealed class FinalGradeCalculator
{
    public FinalGrade? Calculate(Student student, Rubric rubric, IGradingPolicy policy)
    {
        ArgumentNullException.ThrowIfNull(student);
        ArgumentNullException.ThrowIfNull(rubric);
        ArgumentNullException.ThrowIfNull(policy);

        var scores = rubric.Criteria
            .Select(criterion => new CriterionScore(criterion, student.FindEvaluation(criterion.Id)?.Score))
            .ToList();

        var value = policy.Compute(scores);
        if (value is null)
            return null;

        return new FinalGrade(value.Value);
    }
}
