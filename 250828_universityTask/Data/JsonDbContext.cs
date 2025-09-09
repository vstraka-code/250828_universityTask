using _250828_universityTask.Models;
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

        // helper for serialization (same shape as your JSON file)
        public class JsonData
        {
            public List<Professor>? Professors { get; set; }
            public List<Student>? Students { get; set; }
            public List<University>? Universities { get; set; }
        }

        public JsonDbContext()
        {
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
                    Students = data.Students ?? new();
                    Professors = data.Professors ?? new();
                    Universities = data.Universities ?? new();

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

        public void Save()
        {
            var data = new JsonData
            {
                Students = Students,
                Professors = Professors,
                Universities = Universities
            };

            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(_filePath, json);
        }
    }
}
