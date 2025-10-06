namespace _250828_universityTask.Models
{
    public class Student2
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        // foreign key to University
        public int? UniversityId { get; set; }
        public University? University { get; set; }


        // Professor who added this student
        public int? ProfessorAddedId { get; set; }
        public Professor? ProfessorAdded { get; set; }

        public ICollection<Professor> BelongingProfessors { get; set; } = new List<Professor>();
    }
}
