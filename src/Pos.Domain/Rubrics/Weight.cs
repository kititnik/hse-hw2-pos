namespace Pos.Domain.Rubrics;

public sealed record Weight
{
    public decimal Value { get; }

    public Weight(decimal value)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);

        Value = value;
    }
}
