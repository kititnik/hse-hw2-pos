namespace Pos.Domain.Rubrics;

public sealed record MaxScore
{
    public decimal Value { get; }

    public MaxScore(decimal value)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);

        Value = value;
    }
}
