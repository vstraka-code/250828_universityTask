using _250828_universityTask.Cache;
using _250828_universityTask.Data;
using _250828_universityTask.Logger;
using _250828_universityTask.Models;
using _250828_universityTask.Models.Dtos;
using System.Collections.Immutable;

namespace _250828_universityTask.Helpers
{
    public class GenerateIdExtension
    {
        private readonly CacheServiceWithoutExtension _cacheService;

        private readonly FileLoggerProvider _fileLoggerProvider;
        private static string mess = "";
        private static LoggerTopics topic = LoggerTopics.GenerateID;
        public List<int> IdsProfessors { get; set; }
        public List<int> IdsStudents { get; set; }

        public GenerateIdExtension(CacheServiceWithoutExtension cacheService, FileLoggerProvider fileLoggerProvider)
        {
            _cacheService = cacheService;
            _fileLoggerProvider = fileLoggerProvider;
            IdsProfessors = new List<int>();
            IdsStudents = new List<int>();
        }
        public int GenerateIdProfessor()
        {
            var professors = _cacheService.AllProfessors();

            IdsProfessors = professors
                .Select(p => p.Id)
                .ToList();

            return FindMissingNumber(IdsProfessors);
        }

        public int GenerateIdStudent()
        {
            var students = _cacheService.AllStudents();

            IdsStudents = students
                .Select(s => s.Id)
                .ToList();

            return FindMissingNumber(IdsStudents);
        }

        private int FindMissingNumber(List<int> ids)
        {
            if (ids == null || ids.Count == 0) return 1;

            ids.Sort();
            int max = ids.Max();
            bool foundId = false;
            int id = 0;

            for (int min = 1; min <= max; min++)
            {
                bool isThere = ids.Contains(min);

                if (!isThere)
                {
                    id = min;
                    foundId = true;
                    break;
                }
            }

            if (foundId == false)
            {
                id = max + 1;
            }
            
            if (ids.Contains(id))
            {
                mess = "Something went wrong generating an Id.";
                _fileLoggerProvider.SaveBehaviourLogging(mess, topic);
                throw new ArgumentNullException();
            }

            mess = "Available ID: " + id;
            _fileLoggerProvider.SaveBehaviourLogging(mess, topic);

            return id;
        }
    }
}
