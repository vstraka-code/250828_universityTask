using _250828_universityTask.Data;
using _250828_universityTask.Exceptions;
using _250828_universityTask.Models;
using _250828_universityTask.Models.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace _250828_universityTask.Features.Students
{
    public class AddStudentHandler : IRequestHandler<AddStudentCommand, StudentDto>
    {
        private readonly AppDbContext _db;

        // constructor for AddStudentHandler => receives AppDbContext as parameter (db) from Dependency Injection (DI)
        public AddStudentHandler(AppDbContext db) => _db = db;

        public async Task<StudentDto> Handle(AddStudentCommand req, CancellationToken cancellationToken)
        {
            var professor = await _db.Professors
                .Include(p => p.University)
                .FirstOrDefaultAsync(p => p.Id == req.ProfessorId, cancellationToken);

            if (professor == null) throw new UnauthorizedAccessException();

            var duplicate = await _db.Students
                .AnyAsync(s => s.Name == req.Name && s.UniversityId == professor.UniversityId);

            if (duplicate)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Name", new[] { "A student with this name already exists in the university." } }
                });
            }

            if (string.IsNullOrWhiteSpace(req.Name))
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Name", new[] { "Student name is required." } }
                });
            }

            if (req.Name.Length < 2 || req.Name.Length > 50)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Name", new[] { "Student name must be between 2 and 50 characters." } }
                });
            }

            var student = new Student
            {
                Name = req.Name,
                UniversityId = professor.UniversityId,
                ProfessorAddedId = professor.Id
            };

            _db.Students.Add(student);
            await _db.SaveChangesAsync(cancellationToken);

            return new StudentDto(
                student.Id,
                student.Name,
                professor.University.Name,
                professor.Name
            );
        }
    }

}
