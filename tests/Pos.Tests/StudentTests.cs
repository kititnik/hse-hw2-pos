using Pos.Domain.Evaluations;
using Pos.Domain.Rubrics;
using Pos.Domain.Students;

namespace Pos.Tests;

public class StudentTests
{
    private readonly Rubric _rubric = new();
    private readonly EvaluationFactory _evaluationFactory = new();
    private readonly Student _student = new(Guid.NewGuid(), "Test");
    private readonly Criterion _criterion;

    public StudentTests()
    {
        _criterion = _rubric.AddCriterion("Test", new MaxScore(10), new Weight(3));
    }

    [Fact]
    public void Grade_AlreadyGraded_ShouldThrow()
    {
        _student.Grade(_evaluationFactory.Create(_rubric, _criterion.Id, 8, null));

        Assert.Throws<InvalidOperationException>(
            () => _student.Grade(_evaluationFactory.Create(_rubric, _criterion.Id, 9, null)));

        Assert.Equal(8, _student.FindEvaluation(_criterion.Id)?.Score.Value);
    }

    [Fact]
    public void Correct_NotGraded_ShouldThrow()
    {
        Assert.Throws<InvalidOperationException>(
            () => _student.Correct(_evaluationFactory.Create(_rubric, _criterion.Id, 9, null)));
    }

    [Fact]
    public void Cancel_NotGraded_ShouldThrow()
    {
        Assert.Throws<InvalidOperationException>(() => _student.Cancel(_criterion.Id));
    }
}
