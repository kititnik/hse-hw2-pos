using System.Globalization;
using Pos.Application;
using Pos.Domain.Grading;

namespace Pos.Infrastructure;

public sealed class CsvGradeSheetExporter : IGradeSheetExporter
{
    private const string Header = "Name,Current,Final";

    private readonly string _path;

    public CsvGradeSheetExporter(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        _path = path;
    }

    public void Export(IReadOnlyList<GradeSheetRow> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        var directory = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        File.WriteAllLines(_path, [Header, ..rows.Select(ToLine)]);
    }

    private static string ToLine(GradeSheetRow row)
    {
        return $"{row.Student.Name},{Format(row.Current)},{Format(row.Final)}";
    }

    private static string Format(FinalGrade? grade)
    {
        return grade?.Value.ToString(CultureInfo.InvariantCulture) ?? "";
    }
}
