using _250828_universityTask.Data;
using _250828_universityTask.Models;
using _250828_universityTask.Models.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace _250828_universityTask.Features.Students
{
    public class GetStudentHandler : IRequestHandler<GetStudentQuery, StudentDto>
    {
        private readonly AppDbContext _db;

        public GetStudentHandler(AppDbContext db) => _db = db;

        public async Task<StudentDto> Handle(GetStudentQuery req, CancellationToken cancellationToken)
        {
            var student = await _db.Students
                .Include(s => s.University)
                .Include(s => s.ProfessorAdded)
                .FirstOrDefaultAsync(s => s.Id == req.StudentId, cancellationToken);

            if (student == null) throw new KeyNotFoundException();

            if (req.ProfessorId != null)
            {
                var professor = await _db.Professors.FindAsync(req.ProfessorId);
                if (professor == null) throw new UnauthorizedAccessException();
                if (student.UniversityId != professor.UniversityId)
                    throw new UnauthorizedAccessException();
            }
            else if (req.CurrentStudentId != null)
            {
                if (student.Id != req.CurrentStudentId)
                    throw new UnauthorizedAccessException();
            }
            else
            {
                throw new UnauthorizedAccessException();
            }

            return new StudentDto(
                student.Id,
                student.Name,
                student.University?.Name,
                student.ProfessorAdded?.Name
            );
        }
    }
}
