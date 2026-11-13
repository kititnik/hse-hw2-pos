namespace Pos.Application;

public interface IGradeSheetExporter
{
    void Export(IReadOnlyList<GradeSheetRow> rows);
}
