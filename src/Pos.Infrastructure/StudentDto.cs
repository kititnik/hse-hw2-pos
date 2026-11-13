namespace Pos.Infrastructure;

public sealed record StudentDto(Guid Id, string Name, IReadOnlyList<EvaluationDto> Evaluations);
