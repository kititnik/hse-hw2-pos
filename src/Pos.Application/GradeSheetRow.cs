using Pos.Domain.Grading;
using Pos.Domain.Students;

namespace Pos.Application;

public sealed record GradeSheetRow(Student Student, FinalGrade? Current, FinalGrade? Final);
