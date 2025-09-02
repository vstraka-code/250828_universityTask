namespace _250828_universityTask.Auth
{
    public class JwtSettings
    {
        public string Key { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string[]? Audiences { get; set; } = null!;
        public int ExpiryMinutes { get; set; } = 60;
    }
}
