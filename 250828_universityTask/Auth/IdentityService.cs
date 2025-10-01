using _250828_universityTask.Logger;
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
        private const string PROFESSOR = "professor";
        private const string STUDENT = "student";

        private readonly JwtSettings? _settings;
        private readonly byte[] _key; //signing key as raw bytes

        // factory for creating + writing tokens (from Microsoft)
        private readonly JwtSecurityTokenHandler _tokenHandler = new();

        private readonly FileLoggerProvider _fileLoggerProvider;
        private string mess = "";
        private LoggerTopics topic = LoggerTopics.JWTToken;

        // Constructor
        public IdentityService(IOptions<JwtSettings> jwtSettings, FileLoggerProvider fileLoggerProvider)
        {
            _settings = jwtSettings.Value;
            // ensure everything required is present
            ArgumentNullException.ThrowIfNull(_settings);
            ArgumentNullException.ThrowIfNull(_settings.Key);
            ArgumentNullException.ThrowIfNull(_settings.Audiences);
            ArgumentNullException.ThrowIfNull(_settings.Audiences[0]);
            ArgumentNullException.ThrowIfNull(_settings.Issuer);
            _key = Encoding.ASCII.GetBytes(_settings.Key);
            _fileLoggerProvider = fileLoggerProvider;
        }

        // builds blueprint of token - just token object
        // ClaimsIdentity = built-in .NET class representing user identity + claims (from authendpoints)
        public SecurityToken CreateSecurityToken(ClaimsIdentity identity)
        {
            var tokenDesciptor = GetTokenDescriptor(identity);
            return _tokenHandler.CreateToken(tokenDesciptor);
        }

        // Sec Token into a JWT string, this will be send to the client
        public string WriteToken(SecurityToken token)
        {
            mess = "Token converted to JWT string.";
            _fileLoggerProvider.SaveBehaviourLogging(mess, topic);

            return _tokenHandler.WriteToken(token);
        }

        // payload recipe
        private SecurityTokenDescriptor GetTokenDescriptor(ClaimsIdentity identity)
        {
            mess = "Created Token.";
            _fileLoggerProvider.SaveBehaviourLogging(mess, topic);

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

        // adding claims
        public string CreateToken(int id, string role = "", int? uniId = null)
        {
            var claims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, id.ToString()), //subject
                        new Claim(ClaimTypes.Role, role),
                    };
            if (role == PROFESSOR)
            {
                claims.Add(new("ProfessorId", id.ToString()));
                claims.Add(new("UniversityId", uniId?.ToString() ?? "")); // if id !null convert to string, otherwise empty string
            }
            else if (role == STUDENT)
            {
                claims.Add(new("StudentId", id.ToString()));
            }

            var token = WriteToken(CreateSecurityToken(new ClaimsIdentity(claims)));

            mess = "Finished Token for id " + id;
            _fileLoggerProvider.SaveBehaviourLogging(mess, topic);

            return token;
        }
    }
}

// 1. create ClaimsIdentity in authendpoints
// 2.CreateSecurityToken
// 3.which calls GetTokenDescriptor
// 4. WriteToken

// identityService.WriteToken(identityService.CreateSecurityToken(new ClaimsIdentity(claims)));