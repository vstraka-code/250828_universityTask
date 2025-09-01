using _250828_universityTask.Data;
using _250828_universityTask.Models.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace _250828_universityTask.Features.Students
{
    public class GetAllStudentsHandler : IRequestHandler<GetAllStudentsQuery, List<StudentDto>>
    {
        private readonly AppDbContext _db;

        public GetAllStudentsHandler(AppDbContext db) => _db = db;

        public async Task<List<StudentDto>> Handle(GetAllStudentsQuery req, CancellationToken cancellationToken)
        {
            var professor = await _db.Professors.FindAsync(req.ProfessorId);
            if (professor == null) throw new UnauthorizedAccessException();

            var students = await _db.Students
                .Where(s => s.UniversityId == professor.UniversityId)
                .Include(s => s.University)
                .Include(s => s.ProfessorAdded)
                .Select(s => new StudentDto(
                    s.Id,
                    s.Name,
                    s.University != null ? s.University.Name : null,
                    s.ProfessorAdded != null ? s.ProfessorAdded.Name : null
                ))
                .ToListAsync(cancellationToken);

            return students;
        }
    }
}
