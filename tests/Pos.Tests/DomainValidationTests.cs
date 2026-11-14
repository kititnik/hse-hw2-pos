using Pos.Domain.Evaluations;
using Pos.Domain.Rubrics;
using Pos.Domain.Students;

namespace Pos.Tests;

public class DomainValidationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_NonPositiveMaxScore_ShouldThrow(int maxScore)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MaxScore(maxScore));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_NonPositiveWeight_ShouldThrow(int weight)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Weight(weight));
    }

    [Fact]
    public void Constructor_NegativeScore_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Score(-1));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_BlankName_ShouldThrow(string name)
    {
        Assert.Throws<ArgumentException>(() => new Student(Guid.NewGuid(), name));
    }

    [Fact]
    public void Constructor_EmptyId_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => new Student(Guid.Empty, "Test"));
    }

    [Fact]
    public void Constructor_ValidArguments_ShouldSetProperties()
    {
        var id = Guid.NewGuid();

        var criterion = new Criterion(id, " Test ", new MaxScore(10), new Weight(3));

        Assert.Equal(id, criterion.Id);
        Assert.Equal("Test", criterion.Name);
        Assert.Equal(10, criterion.MaxScore.Value);
        Assert.Equal(3, criterion.Weight.Value);
    }
}
