namespace Pos.Domain.Rubrics;

public sealed class Rubric
{
    private readonly List<Criterion> _criteria = [];

    public IReadOnlyList<Criterion> Criteria
    {
        get { return _criteria.AsReadOnly(); }
    }

    public Rubric() : this([])
    {
    }

    public Rubric(IEnumerable<Criterion> criteria)
    {
        ArgumentNullException.ThrowIfNull(criteria);

        foreach (var criterion in criteria)
            Add(criterion);
    }

    public Criterion AddCriterion(string name, MaxScore maxScore, Weight weight)
    {
        var criterion = new Criterion(Guid.NewGuid(), name, maxScore, weight);
        Add(criterion);

        return criterion;
    }

    public Criterion GetCriterion(Guid id)
    {
        return _criteria.Find(criterion => criterion.Id == id)
               ?? throw new ArgumentException($"Criterion {id} is not in the rubric.", nameof(id));
    }

    private void Add(Criterion criterion)
    {
        ArgumentNullException.ThrowIfNull(criterion);
        if (_criteria.Exists(existing => existing.Id == criterion.Id))
            throw new InvalidOperationException($"Criterion {criterion.Id} is already in the rubric.");
        if (_criteria.Exists(existing =>
                string.Equals(existing.Name, criterion.Name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Criterion '{criterion.Name}' is already in the rubric.");

        _criteria.Add(criterion);
    }
}
