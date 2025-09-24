using _250828_universityTask.Models.Requests;
using Azure.Core;
using System.Net.Http.Headers;
using System.Text;

namespace _250828_universityTask.Helpers
{
    public static class GetHeaderValueExtension
    {
        private static readonly string name = "name";
        private static readonly string password = "password";

        public static LoginRequest GetHeaderValueLogin(HttpContext context)
        {
            var req = new LoginRequest(
                Id: ParseId(GetHeaderValue(context, "USER-ID")),
                Email: EncodingAuthenticationHeader(context, name),
                Password: EncodingAuthenticationHeader(context, password),
                Role: GetHeaderValue(context, "USER-ROLE")
            );

            return req;
        }

        public static RegistrationRequest GetHeaderValueRegister(HttpContext context)
        {
            var req = new RegistrationRequest(
                Name: GetHeaderValue(context, "PROF-NAME"),
                Email: EncodingAuthenticationHeader(context, name),
                UniId: ParseId(GetHeaderValue(context, "UNI-ID"))
            );

            return req;
        }

        public static AddStudentRequest GetHeaderValueAdd(HttpContext context)
        {
            var req = new AddStudentRequest(
                Name: GetHeaderValue(context, "NAME")
            );

            return req;
        }

        public static UpdateStudentRequest GetHeaderValueUpdate(HttpContext context)
        {
            var req = new UpdateStudentRequest(
                Name: GetHeaderValue(context, "NAME")
            );

            return req;
        }

        public static string GetHeaderValue(HttpContext context, string headerValue)
        {
            context.Request.Headers.TryGetValue(headerValue, out var value);
            return value.ToString();
        }

        public static string EncodingAuthenticationHeader(HttpContext context, string authField)
        {
            var authHeader = context.Request.Headers["Authorization"];
            var authHeaderVal = AuthenticationHeaderValue.Parse(authHeader);

            var encoding = Encoding.GetEncoding("iso-8859-1");
            var credentialBytes = Convert.FromBase64String(authHeaderVal.Parameter ?? string.Empty);
            var credentials = encoding.GetString(credentialBytes);

            int separator = credentials.IndexOf(':');

            if (authField == name)
            {
                return credentials.Substring(0, separator);
            } else if (authField == password)
            {
                return credentials.Substring(separator + 1);
            }

            return "";
        }

        public static int? ParseId(string unparsedId)
        {
            int? id = int.TryParse(unparsedId, out var parsedId) ? parsedId : null;
            return id;
        }
    }
}
