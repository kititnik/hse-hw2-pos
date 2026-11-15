# ПОС

## Запуск
```
dotnet run --project src/Pos.ConsoleApp
```
Данные хранятся в `data/` в текущем каталоге: `students.json`, `rubric.json`, ведомость выгружается в `grades.csv`.
### Тесты
```
dotnet test
```

## Архитектура

Ports and Adapters

```
src/
    Pos.Domain          сущности, VO, фабрика, расчёт итога, порты-репозитории
    Pos.Application     сценарии, порт IGradeSheetExporter. Зависит от Domain
    Pos.Infrastructure  JSON-репозитории, CSV-выгрузка. Зависит от Domain и Application
    Pos.ConsoleApp      меню и сборка DI. Зависит от всего
tests/
    Pos.Tests           тесты. Зависит от Domain
```

- Ядро - `Pos.Domain` и `Pos.Application`. Domain не знает про файлы, консоль и сериализацию.
- Порты - `IStudentRepository`, `IRubricRepository` (Domain), `IGradeSheetExporter` (Application).
- Адаптеры - `JsonStudentRepository`, `JsonRubricRepository`, `CsvGradeSheetExporter` и консольное меню.
- `CsvGradeSheetExporter` упоминается только в `Program.cs`. Меню вызывает `FinalGradeService.Export()`, а сервис зависит от порта.

## Карта DDD

```
Rubric (aggregate root)
    Criterion (entity): Id, Name, MaxScore, Weight

Student (aggregate root, entity): Id, Name
    Evaluation (VO): CriterionId, Score, Comment
```

| Термин | Тип |
|---|---|
| студент | `Student` |
| рубрикатор | `Rubric` |
| критерий | `Criterion` |
| максимум критерия, вес, балл | `MaxScore`, `Weight`, `Score` |
| оценка | `Evaluation` |
| итог | `FinalGrade` |
| текущий итог / итоговая оценка | `GradeKind.Current` / `GradeKind.Final` |
| ведомость | `GradeSheetRow` |

- Entity - `Student` и `Criterion`. Два студента с одинаковым ФИО - разные студенты, а на критерий оценки ссылаются по `Id`.
- Value Object - `Score`, `MaxScore`, `Weight`, `FinalGrade`, `Evaluation`. `Evaluation` - VO, потому что у оценки нет жизни вне студента.
- Агрегаты - `Rubric` и `Student`. Связаны только через `Guid` критерия.
- Domain Service - `FinalGradeCalculator`. Итог зависит от двух агрегатов (оценки студента и веса рубрикатора), поэтому не принадлежит ни одному из них.
- Repository - порты в Domain, реализации на JSON в Infrastructure.

### Инварианты

| Правило | Где |
|---|---|
| балл >= 0, max > 0, вес > 0, итог от 0 до 10 | конструкторы VO |
| балл не больше max критерия | `EvaluationFactory` |
| оценка только по критерию из рубрикатора | `EvaluationFactory` через `Rubric.GetCriterion` |
| одна оценка на критерий | `Student.Grade`, для замены есть `Correct` |
| уникальные названия критериев | `Rubric.AddCriterion` |
| итог согласован с оценками | итог не хранится, считается при каждом запросе |

## Паттерны GoF

| Группа | Паттерн | Где | Какую боль закрывает |
|---|---|---|---|
| Порождающий | Factory | `EvaluationFactory` | для создания оценки нужны данные двух агрегатов. Единственная точка создания, которую нельзя обойти |
| Структурный | Decorator | `RoundingPolicy` | округление не зависит от способа счёта, внутри может быть любая политика без отдельных классов на каждую комбинацию |
| Поведенческий | Strategy | `IGradingPolicy`: `GradedOnlyPolicy`, `MissingAsZeroPolicy` | два правила итога. Выбираются по `GradeKind`, связь задаётся в `Program.cs` |

## Тесты

21 тест на Domain: инварианты VO, `EvaluationFactory`, `Student` и расчёт итога с обеими политиками и округлением.

ИИ не использовался
