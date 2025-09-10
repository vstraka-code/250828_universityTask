using _250828_universityTask.Models;
using _250828_universityTask.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace _250828_universityTask.Data
{
    public class JsonDbContext
    {
        private readonly string _filePath = Path.Combine(AppContext.BaseDirectory, "data", "database.json");

        public List<Professor> Professors { get; set; }
        public List<Student> Students { get; set; }
        public List<University> Universities { get; set; }

        public JsonDbContext()
        {
            Students = new List<Student>();
            Professors = new List<Professor>();
            Universities = new List<University>();
            Load();
        }

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
                var data = JsonSerializer.Deserialize<JsonData>(writableDoc);
                if (data != null)
                {
                    Students = data.Students?.Select(MapStudent).ToList() ?? new();
                    Professors = data.Professors?.Select(MapProfessor).ToList() ?? new();
                    Universities = data.Universities?.Select(MapUniversity).ToList() ?? new();

                    MapVariables();
                }
            }
        }

        public void Save()
        {
            var data = new
            {
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

            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(_filePath, json);
        }

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

        private void MapVariables()
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
    }
}
