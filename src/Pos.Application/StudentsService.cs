using Pos.Domain.Students;

namespace Pos.Application;

public sealed class StudentsService
{
    private readonly IStudentRepository _studentRepository;

    public StudentsService(IStudentRepository studentRepository)
    {
        ArgumentNullException.ThrowIfNull(studentRepository);

        _studentRepository = studentRepository;
    }

    public Student AddStudent(string name)
    {
        var student = new Student(Guid.NewGuid(), name);
        _studentRepository.Save(student);

        return student;
    }

    public IReadOnlyList<Student> GetAll()
    {
        return _studentRepository.GetAll();
    }
}
