using _250828_universityTask.Data;
using _250828_universityTask.Exceptions;
using _250828_universityTask.Models.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace _250828_universityTask.Features.Students
{
    public class UpdateStudentHandler : IRequestHandler<UpdateStudentCommand, StudentDto>
    {
        private readonly AppDbContext _db;

        public UpdateStudentHandler(AppDbContext db) => _db = db;

        public async Task<StudentDto> Handle(UpdateStudentCommand req, CancellationToken cancellationToken)
        {
            var professor = await _db.Professors
                .Include(p => p.University)
                .FirstOrDefaultAsync(p => p.Id == req.ProfessorId);

            if (professor == null)
                throw new UnauthorizedAccessException();

            var duplicate = await _db.Students
                .AnyAsync(s => s.Name == req.Name && s.UniversityId == professor.UniversityId);

            if (duplicate)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Name", new[] { "A student with this name already exists in the university." } }
                });
            }

            var student = await _db.Students
                .Include(s => s.University)
                .Include(s => s.ProfessorAdded)
                .FirstOrDefaultAsync(s => s.Id == req.StudentId, cancellationToken);

            if (student == null) throw new KeyNotFoundException();

            if (student.UniversityId != (await _db.Professors.FindAsync(req.ProfessorId))?.UniversityId)
                throw new UnauthorizedAccessException();

            student.Name = req.Name;
            await _db.SaveChangesAsync(cancellationToken);

            return new StudentDto(
                student.Id,
                student.Name,
                student.University?.Name,
                student.ProfessorAdded?.Name
            );
        }
    }
}
