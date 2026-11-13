using Pos.Domain.Evaluations;
using Pos.Domain.Rubrics;
using Pos.Domain.Students;

namespace Pos.Infrastructure;

public sealed class JsonStudentRepository : IStudentRepository
{
    private readonly string _path;
    private readonly IRubricRepository _rubricRepository;
    private readonly EvaluationFactory _evaluationFactory;

    public JsonStudentRepository(string path, IRubricRepository rubricRepository, EvaluationFactory evaluationFactory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(rubricRepository);
        ArgumentNullException.ThrowIfNull(evaluationFactory);

        _path = path;
        _rubricRepository = rubricRepository;
        _evaluationFactory = evaluationFactory;
    }

    public IReadOnlyList<Student> GetAll()
    {
        var rubric = _rubricRepository.Get();

        return ReadAll()
            .Select(dto => ToStudent(dto, rubric))
            .ToList();
    }

    public Student? Find(Guid id)
    {
        var dto = ReadAll().Find(student => student.Id == id);
        if (dto is null)
            return null;

        return ToStudent(dto, _rubricRepository.Get());
    }

    public void Save(Student student)
    {
        ArgumentNullException.ThrowIfNull(student);

        var students = ReadAll();
        var index = students.FindIndex(existing => existing.Id == student.Id);
        if (index >= 0)
            students[index] = ToDto(student);
        else
            students.Add(ToDto(student));

        JsonFile.Write(_path, students);
    }

    private List<StudentDto> ReadAll()
    {
        return JsonFile.Read<List<StudentDto>>(_path) ?? [];
    }

    private Student ToStudent(StudentDto dto, Rubric rubric)
    {
        var student = new Student(dto.Id, dto.Name);
        foreach (var evaluation in dto.Evaluations)
            student.Grade(_evaluationFactory.Create(rubric, evaluation.CriterionId, evaluation.Score,
                evaluation.Comment));

        return student;
    }

    private static StudentDto ToDto(Student student)
    {
        var evaluations = student.Evaluations
            .Select(evaluation => new EvaluationDto(evaluation.CriterionId, evaluation.Score.Value, evaluation.Comment))
            .ToList();

        return new StudentDto(student.Id, student.Name, evaluations);
    }
}
