using Pos.Domain.Evaluations;
using Pos.Domain.Grading;
using Pos.Domain.Rubrics;
using Pos.Domain.Students;

namespace Pos.Tests;

public class FinalGradeCalculatorTests
{
    private readonly Rubric _rubric = new();
    private readonly EvaluationFactory _evaluationFactory = new();
    private readonly FinalGradeCalculator _calculator = new();
    private readonly Student _student = new(Guid.NewGuid(), "Test");
    private readonly Criterion _homework;
    private readonly Criterion _exam;

    public FinalGradeCalculatorTests()
    {
        _homework = _rubric.AddCriterion("Test", new MaxScore(10), new Weight(3));
        _exam = _rubric.AddCriterion("Test2", new MaxScore(20), new Weight(1));
    }

    [Fact]
    public void Calculate_AllGraded_ShouldUseWeightsAndMaxScores()
    {
        _student.Grade(_evaluationFactory.Create(_rubric, _homework.Id, 5, null));
        _student.Grade(_evaluationFactory.Create(_rubric, _exam.Id, 20, null));

        var result = _calculator.Calculate(_student, _rubric, new MissingAsZeroPolicy());

        Assert.Equal(6.25m, result?.Value);
    }

    [Fact]
    public void Calculate_GradedOnlyPolicy_ShouldIgnoreMissing()
    {
        _student.Grade(_evaluationFactory.Create(_rubric, _homework.Id, 5, null));

        var result = _calculator.Calculate(_student, _rubric, new GradedOnlyPolicy());

        Assert.Equal(5m, result?.Value);
    }

    [Fact]
    public void Calculate_MissingAsZeroPolicy_ShouldCountMissingAsZero()
    {
        _student.Grade(_evaluationFactory.Create(_rubric, _homework.Id, 5, null));

        var result = _calculator.Calculate(_student, _rubric, new MissingAsZeroPolicy());

        Assert.Equal(3.75m, result?.Value);
    }

    [Fact]
    public void Calculate_RoundingPolicy_ShouldRoundMidpointAwayFromZero()
    {
        _student.Grade(_evaluationFactory.Create(_rubric, _homework.Id, 6, null));

        var result = _calculator.Calculate(_student, _rubric, new RoundingPolicy(new MissingAsZeroPolicy(), 0));

        Assert.Equal(5m, result?.Value);
    }

    [Fact]
    public void Calculate_GradedOnlyPolicyWithoutEvaluations_ShouldReturnNull()
    {
        var result = _calculator.Calculate(_student, _rubric, new GradedOnlyPolicy());

        Assert.Null(result);
    }
}
