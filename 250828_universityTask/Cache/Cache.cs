using _250828_universityTask.Logger;
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

        private FileLoggerProvider _fileLoggerProvider;
        private string mess = "";
        private LoggerTopics topic = LoggerTopics.Cache;

        public Cache(FileLoggerProvider fileLoggerProvider)
        {
            Students = new List<Student>();
            Professors = new List<Professor>();
            Universities = new List<University>();
            _fileLoggerProvider = fileLoggerProvider;
        }

        public void SaveCacheStudents(List<Student> StudentsDb)
        {
            Students = StudentsDb.ToList();
            mess = "Students Cache saved.";
            _fileLoggerProvider.SaveBehaviourLogging(mess, topic);
        }

        public void SaveCacheProfessors(List<Professor> ProfessorsDb)
        {
            Professors = ProfessorsDb.ToList();
            mess = "Professors Cache saved.";
            _fileLoggerProvider.SaveBehaviourLogging(mess, topic);
        }

        public void SaveCacheUniversities(List<University> UniversitiesDb)
        {
            Universities = UniversitiesDb.ToList();
            mess = "Univiersities Cache saved.";
            _fileLoggerProvider.SaveBehaviourLogging(mess, topic);
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
