using _250828_universityTask.Data;
using _250828_universityTask.Logger;
using _250828_universityTask.Models;

namespace _250828_universityTask.Cache
{
    public class CacheServiceWithoutExtension
    {
        #region Properties
        private string mess = "";

        [Inject] private LoggerTopics _topic = LoggerTopics.Cache;
        [Inject] private readonly IJsonDbContext _json;
        [Inject] private readonly Cache _cache;
        [Inject] private readonly ILogger<CacheServiceWithoutExtension> _logger;
        [Inject] private FileLoggerProvider _fileLoggerProvider;
        #endregion

        #region Constructor
        public CacheServiceWithoutExtension(IJsonDbContext json, Cache cache, ILogger<CacheServiceWithoutExtension> logger, FileLoggerProvider filelogger)
        {
            _json = json;
            _cache = cache;
            _logger = logger;
            _fileLoggerProvider = filelogger;
        }
        #endregion

        #region Cache
        public List<Student> AllStudents()
        {
            // List<Student> students;
            // students = new List<Student>();

            var students = new List<Student>();

            students = _cache.GetCacheStudents();

            if (students.Any())
            {
                mess = "Students found in cache.";
                _fileLoggerProvider.SaveBehaviourLogging(mess, _topic);
            }
            else
            {
                mess = "Students NOT found in cache.";
                _fileLoggerProvider.SaveBehaviourLogging(mess, _topic);

                students = _json.Students;

                _cache.SaveCacheStudents(students);
            }

            mess = "Finished Students.";
            _fileLoggerProvider.SaveBehaviourLogging(mess, _topic);

            return students;
        }

        public List<Professor> AllProfessors()
        {
            List<Professor> professors;
            professors = new List<Professor>();

            professors = _cache.GetCacheProfessors();

            if (professors.Any())
            {
                mess = "Professors found in cache.";
                _fileLoggerProvider.SaveBehaviourLogging(mess, _topic);
            }
            else
            {
                mess = "Professors NOT found in cache.";
                _fileLoggerProvider.SaveBehaviourLogging(mess, _topic);

                professors = _json.Professors;

                _cache.SaveCacheProfessors(professors);
            }

            mess = "Finished Professors.";
            _fileLoggerProvider.SaveBehaviourLogging(mess, _topic);

            return professors;
        }
        #endregion

        #region ClearCache
        public IResult ClearCache()
        {
            _cache.RemoveCacheStudents();
            _cache.RemoveCacheProfessors();

            mess = "Cleared cache.";
            _fileLoggerProvider.SaveBehaviourLogging(mess, _topic);

            return Results.Ok("Cache cleared");
        }

        public IResult ClearStudentsCache()
        {
            _cache.RemoveCacheStudents();

            mess = "Cleared Students cache.";
            _fileLoggerProvider.SaveBehaviourLogging(mess, _topic);

            return Results.Ok("Students Cache cleared");
        }

        public IResult ClearProfessorCache()
        {
            _cache.RemoveCacheProfessors();

            mess = "Cleared Professor cache.";
            _fileLoggerProvider.SaveBehaviourLogging(mess, _topic);

            return Results.Ok("Professor Cache cleared");
        }
        #endregion

    }
}
