namespace _250828_universityTask.Models
{
    public class University
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;

        // Navigation property
        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<Professor> Professors { get; set; } = new List<Professor>();
    }
}
