using _250828_universityTask.Models;

namespace _250828_universityTask.Data
{
    // test purpose
    public interface IJsonDbContext
    {
        List<Professor> Professors { get; set; }
        List<Student> Students { get; set; }
        List<University> Universities { get; set; }
        void Save();
    }
}
