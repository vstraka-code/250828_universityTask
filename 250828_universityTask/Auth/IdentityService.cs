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

        // factory for creating + writing tokens
        private static JwtSecurityTokenHandler TokenHandler => new();

        // builds blueprint of token
        public SecurityToken CreateSecurityToken(ClaimsIdentity identity)
        {
            var tokenDesciptor = GetTokenDescriptor(identity);

            return TokenHandler.CreateToken(tokenDesciptor);
        }

        // Sec Token into a JWT string
        public string WriteToken(SecurityToken token)
        {
            return TokenHandler.WriteToken(token);
        }

        // payload recipe
        private SecurityTokenDescriptor GetTokenDescriptor(ClaimsIdentity identity)
        {
            return new SecurityTokenDescriptor()
            {
                Subject = identity,
                Expires = DateTime.Now.AddHours(1),
                Audience = _settings!.Audiences?[0]!,
                Issuer = _settings!.Issuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key),
                SecurityAlgorithms.HmacSha256Signature)
            };
        }
    }
}
