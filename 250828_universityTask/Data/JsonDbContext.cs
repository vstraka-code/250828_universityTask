using _250828_universityTask.Logger;
using _250828_universityTask.Models;
using _250828_universityTask.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace _250828_universityTask.Data
{
    // custom in-memory DB
    public class JsonDbContext : IJsonDbContext
    {
        #region Properties
        private readonly string _filePath = @"C:\Users\stv\source\repos\250828_universityTask\250828_universityTask\Data\database.json";
        private string mess = "";

        // representing tables
        public List<Professor> Professors { get; set; }
        public List<Student> Students { get; set; }
        public List<University> Universities { get; set; }

        [Inject] private readonly FileLoggerProvider _fileLoggerProvider;
        [Inject] private LoggerTopics _topic = LoggerTopics.JSONDB;
        #endregion

        #region Constructor
        // constructor
        public JsonDbContext(FileLoggerProvider fileLoggerProvider)
        {
            Students = new List<Student>();
            Professors = new List<Professor>();
            Universities = new List<University>();
            _fileLoggerProvider = fileLoggerProvider;
            Load();
        }
        #endregion

        #region LoadAndSave
        public void Load()
        {
            if (!File.Exists(_filePath))
            {
                Save();
                return;
            }

            var writableDoc = File.ReadAllText(_filePath);

            if (!string.IsNullOrWhiteSpace(writableDoc))
            {
                // convert JSON string back into objects
                var data = JsonSerializer.Deserialize<JsonData>(writableDoc);
                if (data != null)
                {
                    Students = data.Students?.Select(MapStudent).ToList() ?? new(); // if deserialization = null => use empty list
                    Professors = data.Professors?.Select(MapProfessor).ToList() ?? new();
                    Universities = data.Universities?.Select(MapUniversity).ToList() ?? new();

                    MapOtherVariables();

                    mess = "JSON DB File read (converted from JSON into Objects)";
                    _fileLoggerProvider.SaveBehaviourLogging(mess, _topic);
                }
            }
        }

        // virtual = can be overridden
        public virtual void Save()
        {
            var data = new
            {
                // select maps objects to records
                Students = Students.Select(s => new StudentRecord
                (
                    s.Id,
                    s.Name,
                    s.UniversityId ?? 0,
                    s.ProfessorAddedId ?? 0
                )).ToList(),

                Professors = Professors.Select(p => new ProfessorRecord
                (
                    p.Id,
                    p.Email,
                    p.Name,
                    p.UniversityId
                )).ToList(),

                Universities = Universities.Select(u => new UniversityRecord
                (
                    u.Id,
                    u.Name,
                    u.City,
                    u.Country
                )).ToList()
            };

            // back to to JSON string
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                // humand-readable - you don't need it above because writing is only here
                WriteIndented = true
            });

            File.WriteAllText(_filePath, json);

            mess = "JSON DB File filled with content.";
            _fileLoggerProvider.SaveBehaviourLogging(mess, _topic);
        }
        #endregion

        #region Mapping
        private Student MapStudent(Student s) => new Student
        {
            Id = s.Id,
            Name = s.Name,
            UniversityId = s.UniversityId,
            ProfessorAddedId = s.ProfessorAddedId
        };

        private Professor MapProfessor(Professor p) => new Professor
        {
            Id = p.Id,
            Email = p.Email,
            Name = p.Name,
            UniversityId = p.UniversityId
        };

        private University MapUniversity(University u) => new University
        {
            Id = u.Id,
            Name = u.Name,
            City = u.City,
            Country = u.Country
        };

        private void MapOtherVariables() // sets navigation properties
        {
            foreach (var student in Students)
            {
                student.University = Universities.FirstOrDefault(u => u.Id == student.UniversityId);
                student.ProfessorAdded = Professors.FirstOrDefault(p => p.Id == student.ProfessorAddedId);
            }

            foreach (var professor in Professors)
            {
                professor.University = Universities.FirstOrDefault(u => u.Id == professor.UniversityId);
                professor.AddedStudents = Students.Where(s => s.ProfessorAddedId == professor.Id).ToList();
            }
        }
        #endregion
    }
}
