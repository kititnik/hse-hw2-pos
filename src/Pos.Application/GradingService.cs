using Pos.Domain.Evaluations;
using Pos.Domain.Rubrics;
using Pos.Domain.Students;

namespace Pos.Application;

public sealed class GradingService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IRubricRepository _rubricRepository;
    private readonly EvaluationFactory _evaluationFactory;

    public GradingService(IStudentRepository studentRepository, IRubricRepository rubricRepository,
        EvaluationFactory evaluationFactory)
    {
        ArgumentNullException.ThrowIfNull(studentRepository);
        ArgumentNullException.ThrowIfNull(rubricRepository);
        ArgumentNullException.ThrowIfNull(evaluationFactory);

        _studentRepository = studentRepository;
        _rubricRepository = rubricRepository;
        _evaluationFactory = evaluationFactory;
    }

    public Evaluation Grade(Guid studentId, Guid criterionId, decimal score, string? comment)
    {
        var student = GetStudent(studentId);
        var evaluation = _evaluationFactory.Create(_rubricRepository.Get(), criterionId, score, comment);
        student.Grade(evaluation);
        _studentRepository.Save(student);

        return evaluation;
    }

    public Evaluation Correct(Guid studentId, Guid criterionId, decimal score, string? comment)
    {
        var student = GetStudent(studentId);
        var evaluation = _evaluationFactory.Create(_rubricRepository.Get(), criterionId, score, comment);
        student.Correct(evaluation);
        _studentRepository.Save(student);

        return evaluation;
    }

    public void Cancel(Guid studentId, Guid criterionId)
    {
        var student = GetStudent(studentId);
        student.Cancel(criterionId);
        _studentRepository.Save(student);
    }

    public IReadOnlyCollection<Evaluation> GetEvaluations(Guid studentId)
    {
        return GetStudent(studentId).Evaluations;
    }

    private Student GetStudent(Guid id)
    {
        return _studentRepository.Find(id)
               ?? throw new ArgumentException($"Student {id} is not found.", nameof(id));
    }
}
