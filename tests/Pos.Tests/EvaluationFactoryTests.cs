using Pos.Domain.Evaluations;
using Pos.Domain.Rubrics;

namespace Pos.Tests;

public class EvaluationFactoryTests
{
    private readonly Rubric _rubric = new();
    private readonly EvaluationFactory _evaluationFactory = new();
    private readonly Criterion _criterion;

    public EvaluationFactoryTests()
    {
        _criterion = _rubric.AddCriterion("Test", new MaxScore(10), new Weight(3));
    }

    [Fact]
    public void Create_ScoreAboveMax_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => _evaluationFactory.Create(_rubric, _criterion.Id, 12, null));
    }

    [Fact]
    public void Create_UnknownCriterion_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => _evaluationFactory.Create(_rubric, Guid.NewGuid(), 5, null));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public void Create_ScoreInRange_ShouldReturnEvaluation(int score)
    {
        var result = _evaluationFactory.Create(_rubric, _criterion.Id, score, null);

        Assert.Equal(_criterion.Id, result.CriterionId);
        Assert.Equal(score, result.Score.Value);
    }
}
