namespace _250828_universityTask.Models.Dtos
{
    public record StudentDto
    (
        int Id,
        string Name,
        string? UniversityName,
        string? ProfessorName
    );
}
