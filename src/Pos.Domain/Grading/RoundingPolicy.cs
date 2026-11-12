namespace Pos.Domain.Grading;

public sealed class RoundingPolicy : IGradingPolicy
{
    private readonly IGradingPolicy _inner;
    private readonly int _digits;

    public RoundingPolicy(IGradingPolicy inner, int digits)
    {
        ArgumentNullException.ThrowIfNull(inner);

        _inner = inner;
        _digits = digits;
    }

    public decimal? Compute(IReadOnlyList<CriterionScore> scores)
    {
        var value = _inner.Compute(scores);
        if (value is null)
            return null;

        return Math.Round(value.Value, _digits, MidpointRounding.AwayFromZero);
    }
}
