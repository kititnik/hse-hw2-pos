namespace Pos.Domain.Students;

public interface IStudentRepository
{
    IReadOnlyList<Student> GetAll();
    Student? Find(Guid id);
    void Save(Student student);
}
