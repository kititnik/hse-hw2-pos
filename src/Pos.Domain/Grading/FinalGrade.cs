namespace Pos.Domain.Grading;

public sealed record FinalGrade
{
    public const decimal MinValue = 0m;
    public const decimal MaxValue = 10m;

    public decimal Value { get; }

    public FinalGrade(decimal value)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(value, MinValue);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(value, MaxValue);

        Value = value;
    }
}
