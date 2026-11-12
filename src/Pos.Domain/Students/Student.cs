using Pos.Domain.Evaluations;

namespace Pos.Domain.Students;

public sealed class Student
{
    private readonly Dictionary<Guid, Evaluation> _evaluations = [];

    public Guid Id { get; }
    public string Name { get; }

    public IReadOnlyCollection<Evaluation> Evaluations
    {
        get { return _evaluations.Values; }
    }

    public Student(Guid id, string name)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Student id must not be empty.", nameof(id));
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Id = id;
        Name = name.Trim();
    }

    public void Grade(Evaluation evaluation)
    {
        ArgumentNullException.ThrowIfNull(evaluation);
        if (!_evaluations.TryAdd(evaluation.CriterionId, evaluation))
            throw new InvalidOperationException(
                $"Student '{Name}' already has an evaluation for criterion {evaluation.CriterionId}. Use Correct to replace it.");
    }

    public void Correct(Evaluation evaluation)
    {
        ArgumentNullException.ThrowIfNull(evaluation);
        if (!_evaluations.ContainsKey(evaluation.CriterionId))
            throw new InvalidOperationException(
                $"Student '{Name}' has no evaluation for criterion {evaluation.CriterionId} to correct.");

        _evaluations[evaluation.CriterionId] = evaluation;
    }

    public void Cancel(Guid criterionId)
    {
        if (!_evaluations.Remove(criterionId))
            throw new InvalidOperationException(
                $"Student '{Name}' has no evaluation for criterion {criterionId} to cancel.");
    }

    public Evaluation? FindEvaluation(Guid criterionId)
    {
        return _evaluations.GetValueOrDefault(criterionId);
    }
}
