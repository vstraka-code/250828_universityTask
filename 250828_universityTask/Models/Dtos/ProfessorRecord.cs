namespace _250828_universityTask.Models.Dtos
{
    // helper DTOs for database (to prevent circles from happening)
    public record ProfessorRecord
    (
        int Id,
        string Email,
        string Name,
        int UniversityId
    );
}
