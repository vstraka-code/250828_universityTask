namespace _250828_universityTask.Models
{
    public class Professor
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;


        // Each professor works at ONE university
        public int UniversityId { get; set; }
        public University University { get; set; } = null!;


        // Professors can add students
        public ICollection<Student> AddedStudents { get; set; } = new List<Student>();
    }
}