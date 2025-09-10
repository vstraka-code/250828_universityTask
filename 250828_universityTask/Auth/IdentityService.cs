using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace _250828_universityTask.Auth
{
    // JWT Token creation
    public class IdentityService
    {
        private readonly JwtSettings? _settings;
        private readonly byte[] _key;

        // factory for creating + writing tokens
        private readonly JwtSecurityTokenHandler _tokenHandler = new();


        // Constructor
        public IdentityService(IOptions<JwtSettings> jwtSettings)
        {
            _settings = jwtSettings.Value;
            ArgumentNullException.ThrowIfNull(_settings);
            ArgumentNullException.ThrowIfNull(_settings.Key);
            ArgumentNullException.ThrowIfNull(_settings.Audiences);
            ArgumentNullException.ThrowIfNull(_settings.Audiences[0]);
            ArgumentNullException.ThrowIfNull(_settings.Issuer);
            _key = Encoding.ASCII.GetBytes(_settings?.Key!);
        }

        // builds blueprint of token
        public SecurityToken CreateSecurityToken(ClaimsIdentity identity)
        {
            var tokenDesciptor = GetTokenDescriptor(identity);
            return _tokenHandler.CreateToken(tokenDesciptor);
        }

        // Sec Token into a JWT string
        public string WriteToken(SecurityToken token)
        {
            return _tokenHandler.WriteToken(token);
        }

        // payload recipe
        private SecurityTokenDescriptor GetTokenDescriptor(ClaimsIdentity identity)
        {
            return new SecurityTokenDescriptor()
            {
                Subject = identity,
                Expires = DateTime.UtcNow.AddHours(_settings.ExpiryHours),
                Audience = _settings!.Audiences?[0]!,
                Issuer = _settings!.Issuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key),
                SecurityAlgorithms.HmacSha256Signature)
            };
        }
    }
}
