using System.Security.Claims;

namespace _250828_universityTask.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetProfessorId(this ClaimsPrincipal user)
        {
            // claim.value is always string
            var claim = user.FindFirst("ProfessorId");
            if (claim != null)
                return int.Parse(claim.Value);

            return 0;
        }

        public static int GetStudentId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst("StudentId");
            if (claim != null)
                return int.Parse(claim.Value);

            return 0;
        }
    }
}
