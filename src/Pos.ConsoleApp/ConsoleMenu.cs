using Pos.Application;
using Pos.Domain.Grading;
using Pos.Domain.Rubrics;
using Pos.Domain.Students;

namespace Pos.ConsoleApp;

public sealed class ConsoleMenu
{
    private readonly StudentsService _studentsService;
    private readonly RubricService _rubricService;
    private readonly GradingService _gradingService;
    private readonly FinalGradeService _finalGradeService;
    private readonly ConsoleInput _input;
    private readonly IReadOnlyList<(string Title, Action Action)> _items;

    public ConsoleMenu(StudentsService studentsService, RubricService rubricService, GradingService gradingService,
        FinalGradeService finalGradeService, ConsoleInput input)
    {
        ArgumentNullException.ThrowIfNull(studentsService);
        ArgumentNullException.ThrowIfNull(rubricService);
        ArgumentNullException.ThrowIfNull(gradingService);
        ArgumentNullException.ThrowIfNull(finalGradeService);
        ArgumentNullException.ThrowIfNull(input);

        _studentsService = studentsService;
        _rubricService = rubricService;
        _gradingService = gradingService;
        _finalGradeService = finalGradeService;
        _input = input;
        _items =
        [
            ("Добавить студента", AddStudent),
            ("Список студентов", PrintStudents),
            ("Добавить критерий", AddCriterion),
            ("Список критериев", PrintCriteria),
            ("Выставить оценку", Grade),
            ("Исправить оценку", Correct),
            ("Отменить оценку", Cancel),
            ("Оценки студента", PrintEvaluations),
            ("Итог студента", PrintFinalGrade),
            ("Ведомость", PrintGradeSheet),
            ("Экспорт ведомости в CSV", Export)
        ];
    }

    public void Run()
    {
        try
        {
            while (true)
            {
                PrintMenu();
                var choice = _input.ReadInt("Выберите пункт", 0, _items.Count);
                if (choice == 0)
                    return;

                Console.WriteLine();
                Execute(_items[choice - 1].Action);
                Console.WriteLine();
            }
        }
        catch (EndOfStreamException)
        {
            Console.WriteLine();
            Console.WriteLine("Ввод завершён.");
        }
    }

    private void PrintMenu()
    {
        Console.WriteLine("=== ПОС: оценка студентов ===");
        for (var i = 0; i < _items.Count; i++)
            Console.WriteLine($"{i + 1}. {_items[i].Title}");
        Console.WriteLine("0. Выход");
    }

    private static void Execute(Action action)
    {
        try
        {
            action();
        }
        catch (ArgumentException ex) when (ex is not ArgumentNullException)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    private void AddStudent()
    {
        var student = _studentsService.AddStudent(_input.ReadText("ФИО"));
        Console.WriteLine($"Студент {student.Name} добавлен.");
    }

    private void PrintStudents()
    {
        var students = _studentsService.GetAll();
        if (students.Count == 0)
        {
            Console.WriteLine("Студентов нет.");
            return;
        }

        foreach (var student in students)
            Console.WriteLine(student.Name);
    }

    private void AddCriterion()
    {
        var name = _input.ReadText("Название");
        var maxScore = _input.ReadDecimal("Максимальный балл");
        var weight = _input.ReadDecimal("Вес");

        var criterion = _rubricService.AddCriterion(name, maxScore, weight);
        Console.WriteLine($"Критерий {criterion.Name} добавлен.");
    }

    private void PrintCriteria()
    {
        var criteria = _rubricService.GetCriteria();
        if (criteria.Count == 0)
        {
            Console.WriteLine("Критериев нет.");
            return;
        }

        foreach (var criterion in criteria)
            Console.WriteLine($"{criterion.Name}: максимум {criterion.MaxScore.Value}, вес {criterion.Weight.Value}");
    }

    private void Grade()
    {
        var student = SelectStudent();
        var criterion = SelectCriterion();
        var score = _input.ReadDecimal("Балл");
        var comment = _input.ReadOptionalText("Комментарий (можно пустой)");

        _gradingService.Grade(student.Id, criterion.Id, score, comment);
        Console.WriteLine("Оценка выставлена.");
    }

    private void Correct()
    {
        var student = SelectStudent();
        var criterion = SelectCriterion();
        var score = _input.ReadDecimal("Новый балл");
        var comment = _input.ReadOptionalText("Комментарий (можно пустой)");

        _gradingService.Correct(student.Id, criterion.Id, score, comment);
        Console.WriteLine("Оценка исправлена.");
    }

    private void Cancel()
    {
        var student = SelectStudent();
        var criterion = SelectCriterion();

        _gradingService.Cancel(student.Id, criterion.Id);
        Console.WriteLine("Оценка отменена.");
    }

    private void PrintEvaluations()
    {
        var student = SelectStudent();
        var evaluations = _gradingService.GetEvaluations(student.Id);
        if (evaluations.Count == 0)
        {
            Console.WriteLine("Оценок нет.");
            return;
        }

        var criteria = _rubricService.GetCriteria();
        foreach (var evaluation in evaluations)
        {
            var criterion = criteria.First(criterion => criterion.Id == evaluation.CriterionId);
            var comment = evaluation.Comment is null ? "" : $" - {evaluation.Comment}";
            Console.WriteLine($"{criterion.Name}: {evaluation.Score.Value} из {criterion.MaxScore.Value}{comment}");
        }
    }

    private void PrintFinalGrade()
    {
        var student = SelectStudent();
        var kind = _input.ReadInt("Итог (1 - текущий, 2 - итоговый)", 1, 2) == 1
            ? GradeKind.Current
            : GradeKind.Final;

        Console.WriteLine($"{student.Name}: {Format(_finalGradeService.GetFinalGrade(student.Id, kind))}");
    }

    private void PrintGradeSheet()
    {
        var rows = _finalGradeService.GetGradeSheet();
        if (rows.Count == 0)
        {
            Console.WriteLine("Студентов нет.");
            return;
        }

        foreach (var row in rows)
            Console.WriteLine($"{row.Student.Name}: текущий {Format(row.Current)}, итоговый {Format(row.Final)}");
    }

    private void Export()
    {
        _finalGradeService.Export();
        Console.WriteLine("Ведомость выгружена.");
    }

    private Student SelectStudent()
    {
        var students = _studentsService.GetAll();
        if (students.Count == 0)
            throw new InvalidOperationException("Сначала добавьте студента.");

        for (var i = 0; i < students.Count; i++)
            Console.WriteLine($"{i + 1}. {students[i].Name}");

        return students[_input.ReadInt("Студент", 1, students.Count) - 1];
    }

    private Criterion SelectCriterion()
    {
        var criteria = _rubricService.GetCriteria();
        if (criteria.Count == 0)
            throw new InvalidOperationException("Сначала добавьте критерий.");

        for (var i = 0; i < criteria.Count; i++)
            Console.WriteLine($"{i + 1}. {criteria[i].Name}");

        return criteria[_input.ReadInt("Критерий", 1, criteria.Count) - 1];
    }

    private static string Format(FinalGrade? grade)
    {
        return grade?.Value.ToString() ?? "нет оценок";
    }
}
