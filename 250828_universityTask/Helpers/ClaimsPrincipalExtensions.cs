using System.Security.Claims;

namespace _250828_universityTask.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetProfessorIdOrThrow(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst("ProfessorId");
            if (claim == null)
                throw new UnauthorizedAccessException("Only professors can perform this action.");

            return int.Parse(claim.Value);
        }

        public static int GetStudentIdOrThrow(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst("StudentId");
            if (claim == null)
                throw new UnauthorizedAccessException("Only students can perform this action.");

            return int.Parse(claim.Value);
        }
    }
}
