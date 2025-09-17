using _250828_universityTask.Models;
using _250828_universityTask.Models.Dtos;
using System.Text.Json;

namespace _250828_universityTask.Cache
{
    public class Cache
    {
        public List<Professor> Professors { get; set; }
        public List<Student> Students { get; set; }
        public List<University> Universities { get; set; }

        public Cache()
        {
            Students = new List<Student>();
            Professors = new List<Professor>();
            Universities = new List<University>();
        }

        public void SaveCacheStudents(List<Student> StudentsDb)
        {
            Students = StudentsDb.ToList();
        }

        public void SaveCacheProfessors(List<Professor> ProfessorsDb)
        {
            Professors = ProfessorsDb.ToList();
        }

        public void SaveCacheUniversities(List<University> UniversitiesDb)
        {
            Universities = UniversitiesDb.ToList();
        }

        public List<Student> GetCacheStudents()
        {
            return Students;
        }

        public List<Professor> GetCacheProfessors()
        {
            return Professors;
        }

        public List<University> GetCacheUniversities()
        {
            return Universities;
        }

        public void RemoveCacheStudents()
        {
            Students.Clear();
        }

        public void RemoveCacheProfessors()
        {
            Professors.Clear();
        }
        public void RemoveCacheUniversities()
        {
            Universities.Clear();
        }
    }
}
