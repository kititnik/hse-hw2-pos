using Pos.Domain.Rubrics;

namespace Pos.Application;

public sealed class RubricService
{
    private readonly IRubricRepository _rubricRepository;

    public RubricService(IRubricRepository rubricRepository)
    {
        ArgumentNullException.ThrowIfNull(rubricRepository);

        _rubricRepository = rubricRepository;
    }

    public Criterion AddCriterion(string name, decimal maxScore, decimal weight)
    {
        var rubric = _rubricRepository.Get();
        var criterion = rubric.AddCriterion(name, new MaxScore(maxScore), new Weight(weight));
        _rubricRepository.Save(rubric);

        return criterion;
    }

    public IReadOnlyList<Criterion> GetCriteria()
    {
        return _rubricRepository.Get().Criteria;
    }
}
