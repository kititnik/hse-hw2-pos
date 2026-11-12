namespace Pos.Domain.Rubrics;

public interface IRubricRepository
{
    Rubric Get();
    void Save(Rubric rubric);
}
