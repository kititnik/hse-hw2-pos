using Pos.Domain.Evaluations;
using Pos.Domain.Rubrics;

namespace Pos.Domain.Grading;

public sealed record CriterionScore(Criterion Criterion, Score? Score);
