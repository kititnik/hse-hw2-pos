using Pos.Domain.Grading;
using Pos.Domain.Rubrics;
using Pos.Domain.Students;

namespace Pos.Application;

public sealed class FinalGradeService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IRubricRepository _rubricRepository;
    private readonly FinalGradeCalculator _calculator;
    private readonly IReadOnlyDictionary<GradeKind, IGradingPolicy> _policies;
    private readonly IGradeSheetExporter _exporter;

    public FinalGradeService(
        IStudentRepository studentRepository,
        IRubricRepository rubricRepository,
        FinalGradeCalculator calculator,
        IReadOnlyDictionary<GradeKind, IGradingPolicy> policies,
        IGradeSheetExporter exporter)
    {
        ArgumentNullException.ThrowIfNull(studentRepository);
        ArgumentNullException.ThrowIfNull(rubricRepository);
        ArgumentNullException.ThrowIfNull(calculator);
        ArgumentNullException.ThrowIfNull(policies);
        ArgumentNullException.ThrowIfNull(exporter);
        foreach (var kind in Enum.GetValues<GradeKind>())
        {
            if (!policies.ContainsKey(kind))
                throw new ArgumentException($"No grading policy for {kind}.", nameof(policies));
        }

        _studentRepository = studentRepository;
        _rubricRepository = rubricRepository;
        _calculator = calculator;
        _policies = policies;
        _exporter = exporter;
    }

    public FinalGrade? GetFinalGrade(Guid studentId, GradeKind kind)
    {
        var student = _studentRepository.Find(studentId)
                      ?? throw new ArgumentException($"Student {studentId} is not found.", nameof(studentId));

        return _calculator.Calculate(student, _rubricRepository.Get(), _policies[kind]);
    }

    public IReadOnlyList<GradeSheetRow> GetGradeSheet()
    {
        var rubric = _rubricRepository.Get();

        return _studentRepository.GetAll()
            .Select(student => new GradeSheetRow(
                student,
                _calculator.Calculate(student, rubric, _policies[GradeKind.Current]),
                _calculator.Calculate(student, rubric, _policies[GradeKind.Final])))
            .ToList();
    }

    public void Export()
    {
        _exporter.Export(GetGradeSheet());
    }
}
