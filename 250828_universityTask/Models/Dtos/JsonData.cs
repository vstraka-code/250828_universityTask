namespace _250828_universityTask.Models.Dtos
{
    // helper for serialization (same shape as your JSON file)
    public record JsonData
    (
        List<Professor>? Professors,
        List<Student>? Students,
        List<University>? Universities
    );
}
