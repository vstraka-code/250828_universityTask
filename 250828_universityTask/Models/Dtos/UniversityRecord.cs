namespace _250828_universityTask.Models.Dtos
{
    // helper DTOs for database (to prevent circles from happening)
    public record UniversityRecord
    (
        int Id,
        string Name,
        string City,
        string Country
    );
}
