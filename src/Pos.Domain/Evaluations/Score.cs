namespace Pos.Domain.Evaluations;

public sealed record Score
{
    public decimal Value { get; }

    public Score(decimal value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);

        Value = value;
    }
}
