namespace _250828_universityTask.Models.Requests
{
    public record LoginRequest(
        int? Id,
        string Password = "",
        string Role = ""
    );
}
