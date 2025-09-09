using _250828_universityTask.Data;
using _250828_universityTask.Models;
using _250828_universityTask.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace _250828_universityTask.Cache
{

    public class CacheService
    {
        // private readonly AppDbContext _db;
        private readonly JsonDbContext _json;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CacheService> _logger;
        private readonly string studentscacheKey = "studentsCacheKey";
        private readonly string professorscacheKey = "professorsCacheKey";

        public CacheService(JsonDbContext json, IMemoryCache cache, ILogger<CacheService> logger)
        {
            _json = json;
            _cache = cache;
            _logger = logger;
        }

        //public async Task<List<Student>> AllStudents()
        public List<Student> AllStudents()
        {
            // var stopwatch = new Stopwatch();
            // stopwatch.Start();

            if (_cache.TryGetValue(studentscacheKey, out List<Student> students) && students != null)
            {
                _logger.Log(LogLevel.Information, "Students found in cache.");
            } else
            {
                _logger.Log(LogLevel.Information, "Students NOT found in cache.");

                //students = await _db.Students
                //    .Include(s => s.University)
                //    .Include(s => s.ProfessorAdded)
                //    .ToListAsync();

                students = _json.Students;

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(45))
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                    .SetPriority(CacheItemPriority.Normal);

                _cache.Set(studentscacheKey, students, cacheEntryOptions);
            }

            // stopwatch.Stop();

            // _logger.Log(LogLevel.Information, "Passed time " + stopwatch.ElapsedMilliseconds);
            _logger.Log(LogLevel.Information, "Finished Students");

            return students;
        }

        //public async Task<List<Professor>> AllProfessors()
        public List<Professor> AllProfessors()
        {
            // var stopwatch = new Stopwatch();
            // stopwatch.Start();

            if (_cache.TryGetValue(professorscacheKey, out List<Professor> professors) && professors != null)
            {
                _logger.Log(LogLevel.Information, "Professors found in cache.");
            }
            else
            {
                _logger.Log(LogLevel.Information, "Professors NOT found in cache.");

                //professors = await _db.Professors
                //    .Include(p => p.University)
                //    .Include(p => p.AddedStudents)
                //    .ToListAsync();

                professors = _json.Professors;

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(45))
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
                    .SetPriority(CacheItemPriority.Normal);

                _cache.Set(professorscacheKey, professors, cacheEntryOptions);
            }

            // stopwatch.Stop();

            // _logger.Log(LogLevel.Information, "Passed time " + stopwatch.ElapsedMilliseconds);
            _logger.Log(LogLevel.Information, "Finished Professor");

            return professors;
        }

        public IResult ClearCache()
        {
            _cache.Remove(studentscacheKey);
            _cache.Remove(professorscacheKey);
            _logger.Log(LogLevel.Information, "Cleared cache");

            return Results.Ok("Cache cleared");
        }

        public IResult ClearStudentsCache()
        {
            _cache.Remove(studentscacheKey);
            _logger.Log(LogLevel.Information, "Cleared Students cache");

            return Results.Ok("Students Cache cleared");
        }

        public IResult ClearProfessorCache()
        {
            _cache.Remove(professorscacheKey);
            _logger.Log(LogLevel.Information, "Cleared Professor cache");

            return Results.Ok("Professor Cache cleared");
        }

    }

}
