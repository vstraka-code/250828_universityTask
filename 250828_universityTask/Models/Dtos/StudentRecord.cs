namespace _250828_universityTask.Models.Dtos
{
    // helper DTOs for database (to prevent circles from happening)
    public record StudentRecord
    (
        int Id,
        string Name,
        int UniversityId,
        int ProfessorAddedId
    );
}
