namespace Pos.Domain.Evaluations;

public sealed record Evaluation
{
    public Guid CriterionId { get; }
    public Score Score { get; }
    public string? Comment { get; }

    internal Evaluation(Guid criterionId, Score score, string? comment)
    {
        ArgumentNullException.ThrowIfNull(score);

        CriterionId = criterionId;
        Score = score;
        Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
    }
}
