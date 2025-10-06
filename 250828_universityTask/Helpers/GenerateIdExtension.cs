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
        #region Properties
        private static string mess = "";

        public List<int> IdsProfessors { get; set; }
        public List<int> IdsStudents { get; set; }

        [Inject] private static LoggerTopics _topic = LoggerTopics.GenerateID;
        [Inject] private readonly CacheServiceWithoutExtension _cacheService;
        [Inject] private readonly FileLoggerProvider _fileLoggerProvider;
        #endregion

        #region Constructor
        public GenerateIdExtension(CacheServiceWithoutExtension cacheService, FileLoggerProvider fileLoggerProvider)
        {
            _cacheService = cacheService;
            _fileLoggerProvider = fileLoggerProvider;
            IdsProfessors = new List<int>();
            IdsStudents = new List<int>();
        }
        #endregion

        #region GenerateId
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
        #endregion

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
                _fileLoggerProvider.SaveBehaviourLogging(mess, _topic);
                throw new ArgumentNullException();
            }

            mess = "Available ID: " + id;
            _fileLoggerProvider.SaveBehaviourLogging(mess, _topic);

            return id;
        }
    }
}
