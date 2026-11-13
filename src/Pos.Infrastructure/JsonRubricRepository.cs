using Pos.Domain.Rubrics;

namespace Pos.Infrastructure;

public sealed class JsonRubricRepository : IRubricRepository
{
    private readonly string _path;

    public JsonRubricRepository(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        _path = path;
    }

    public Rubric Get()
    {
        var criteria = JsonFile.Read<List<CriterionDto>>(_path) ?? [];

        return new Rubric(criteria.Select(ToCriterion));
    }

    public void Save(Rubric rubric)
    {
        ArgumentNullException.ThrowIfNull(rubric);

        JsonFile.Write(_path, rubric.Criteria.Select(ToDto).ToList());
    }

    private static Criterion ToCriterion(CriterionDto dto)
    {
        return new Criterion(dto.Id, dto.Name, new MaxScore(dto.MaxScore), new Weight(dto.Weight));
    }

    private static CriterionDto ToDto(Criterion criterion)
    {
        return new CriterionDto(criterion.Id, criterion.Name, criterion.MaxScore.Value, criterion.Weight.Value);
    }
}
