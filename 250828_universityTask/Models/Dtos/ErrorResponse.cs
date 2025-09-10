namespace _250828_universityTask.Models.Dtos
{
    public record ErrorResponse(int Status, string Message, object? Errors = null);

}
