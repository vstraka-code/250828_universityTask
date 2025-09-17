using _250828_universityTask.Data;
using _250828_universityTask.Models;

namespace _250828_universityTask.Cache
{
    public class CacheServiceWithoutExtension
    {
        private readonly IJsonDbContext _json;
        private readonly Cache _cache;
        private readonly ILogger<CacheServiceWithoutExtension> _logger;

        public CacheServiceWithoutExtension(IJsonDbContext json, Cache cache, ILogger<CacheServiceWithoutExtension> logger)
        {
            _json = json;
            _cache = cache;
            _logger = logger;
        }

        public List<Student> AllStudents()
        {
            List<Student> students;
            students = new List<Student>();

            students = _cache.GetCacheStudents();

            if (students.Any())
            {
                _logger.Log(LogLevel.Information, "Students found in cache.");
            }
            else
            {
                _logger.Log(LogLevel.Information, "Students NOT found in cache.");

                students = _json.Students;

                _cache.SaveCacheStudents(students);
            }

            _logger.Log(LogLevel.Information, "Finished Students");

            return students;
        }

        public List<Professor> AllProfessors()
        {
            List<Professor> professors;
            professors = new List<Professor>();

            professors = _cache.GetCacheProfessors();

            if (professors.Any())
            {
                _logger.Log(LogLevel.Information, "Professors found in cache.");
            }
            else
            {
                _logger.Log(LogLevel.Information, "Professors NOT found in cache.");

                professors = _json.Professors;

                _cache.SaveCacheProfessors(professors);
            }

            _logger.Log(LogLevel.Information, "Finished Professor");

            return professors;
        }

        public IResult ClearCache()
        {
            _cache.RemoveCacheStudents();
            _cache.RemoveCacheProfessors();
            _logger.Log(LogLevel.Information, "Cleared cache");

            return Results.Ok("Cache cleared");
        }

        public IResult ClearStudentsCache()
        {
            _cache.RemoveCacheStudents();
            _logger.Log(LogLevel.Information, "Cleared Students cache");

            return Results.Ok("Students Cache cleared");
        }

        public IResult ClearProfessorCache()
        {
            _cache.RemoveCacheProfessors();
            _logger.Log(LogLevel.Information, "Cleared Professor cache");

            return Results.Ok("Professor Cache cleared");
        }

    }
}
