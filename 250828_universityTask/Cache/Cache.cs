using _250828_universityTask.Logger;
using _250828_universityTask.Models;
using _250828_universityTask.Models.Dtos;
using System.Text.Json;

namespace _250828_universityTask.Cache
{
    public class Cache
    {
        #region Properties
        private string mess = "";

        public List<Professor> Professors { get; set; }
        public List<Student> Students { get; set; }
        public List<University> Universities { get; set; }

        [Inject] private FileLoggerProvider _fileLoggerProvider;
        [Inject] private LoggerTopics _topic = LoggerTopics.Cache;
        #endregion 

        #region Constructor
        public Cache(FileLoggerProvider fileLoggerProvider)
        {
            Students = new List<Student>();
            Professors = new List<Professor>();
            Universities = new List<University>();
            _fileLoggerProvider = fileLoggerProvider;
        }
        #endregion

        #region SaveCache
        public void SaveCacheStudents(List<Student> StudentsDb)
        {
            Students = StudentsDb.ToList();
            mess = "Students Cache saved.";
            _fileLoggerProvider.SaveBehaviourLogging(mess, _topic);
        }

        public void SaveCacheProfessors(List<Professor> ProfessorsDb)
        {
            Professors = ProfessorsDb.ToList();
            mess = "Professors Cache saved.";
            _fileLoggerProvider.SaveBehaviourLogging(mess, _topic);
        }

        public void SaveCacheUniversities(List<University> UniversitiesDb)
        {
            Universities = UniversitiesDb.ToList();
            mess = "Univiersities Cache saved.";
            _fileLoggerProvider.SaveBehaviourLogging(mess, _topic);
        }
        #endregion

        #region GetCache
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
        #endregion

        #region RemoveCache
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
        #endregion
    }
}
