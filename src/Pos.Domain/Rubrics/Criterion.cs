namespace Pos.Domain.Rubrics;

public sealed class Criterion
{
    public Guid Id { get; }
    public string Name { get; }
    public MaxScore MaxScore { get; }
    public Weight Weight { get; }

    public Criterion(Guid id, string name, MaxScore maxScore, Weight weight)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Criterion id must not be empty.", nameof(id));
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(maxScore);
        ArgumentNullException.ThrowIfNull(weight);

        Id = id;
        Name = name.Trim();
        MaxScore = maxScore;
        Weight = weight;
    }
}
