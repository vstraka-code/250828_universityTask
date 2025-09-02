namespace _250828_universityTask.Models.Dtos
{
    // clients see that when requesting student data
    public record StudentDto
    (
        int Id,
        string Name,
        string? UniversityName,
        string? ProfessorName
    );
}
