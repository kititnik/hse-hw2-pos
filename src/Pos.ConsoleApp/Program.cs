using Microsoft.Extensions.DependencyInjection;
using Pos.Application;
using Pos.ConsoleApp;
using Pos.Domain.Evaluations;
using Pos.Domain.Grading;
using Pos.Domain.Rubrics;
using Pos.Domain.Students;
using Pos.Infrastructure;

const string dataDir = "data";

using var provider = new ServiceCollection()
    .AddSingleton<IRubricRepository>(new JsonRubricRepository(Path.Combine(dataDir, "rubric.json")))
    .AddSingleton<IStudentRepository>(serviceProvider => new JsonStudentRepository(
        Path.Combine(dataDir, "students.json"),
        serviceProvider.GetRequiredService<IRubricRepository>(),
        serviceProvider.GetRequiredService<EvaluationFactory>()))
    .AddSingleton<IGradeSheetExporter>(new CsvGradeSheetExporter(Path.Combine(dataDir, "grades.csv")))
    .AddSingleton<EvaluationFactory>()
    .AddSingleton<FinalGradeCalculator>()
    .AddSingleton<IReadOnlyDictionary<GradeKind, IGradingPolicy>>(new Dictionary<GradeKind, IGradingPolicy>
    {
        [GradeKind.Current] = new RoundingPolicy(new GradedOnlyPolicy(), 2),
        [GradeKind.Final] = new RoundingPolicy(new MissingAsZeroPolicy(), 0)
    })
    .AddSingleton<StudentsService>()
    .AddSingleton<RubricService>()
    .AddSingleton<GradingService>()
    .AddSingleton<FinalGradeService>()
    .AddSingleton<ConsoleInput>()
    .AddSingleton<ConsoleMenu>()
    .BuildServiceProvider();

provider.GetRequiredService<ConsoleMenu>().Run();
