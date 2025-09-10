using _250828_universityTask.Cache;
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
        // private readonly AppDbContext _db;
        private readonly CacheService _cacheService;
        private readonly JsonDbContext _json;

        // constructor for AddStudentHandler => receives AppDbContext as parameter (db) from Dependency Injection (DI)
        public AddStudentHandler(CacheService cacheService, JsonDbContext json)
        {
            // _db = db;
            _cacheService = cacheService;
            _json = json;
        }

        // CancellationToken is a signal => stop querying the database if the token is triggered
        public Task<StudentDto> Handle(AddStudentCommand req, CancellationToken cancellationToken)
        {
            var professors = _cacheService.AllProfessors();
            // var professors = await _cacheService.AllProfessors();
            var professor = professors.FirstOrDefault(p => p.Id == req.ProfessorId);

            if (professor == null) throw new UnauthorizedAccessException();

            var students = _cacheService.AllStudents();
            // var students = await _cacheService.AllStudents();

            var duplicate = students.Any(s => s.Name == req.Name && s.UniversityId == professor.UniversityId);

            if (duplicate)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Name", new[] { "A student with this name already exists in the university." } }
                });
            }

            var id = (_json.Students.Any() ? _json.Students.Max(s => s.Id) : 0) + 1;

            var student = new Student
            {
                Id = id,
                Name = req.Name,
                UniversityId = professor.UniversityId,
                ProfessorAddedId = professor.Id,
                University = professor.University,
                ProfessorAdded = professor
            };

            // _db.Students.Add(student);
            // await _db.SaveChangesAsync(cancellationToken);

            _json.Students.Add(student);
            _json.Save();

            _cacheService.ClearStudentsCache();

            return Task.FromResult(
                new StudentDto(
                student.Id,
                student.Name,
                professor.University.Name ?? "Unknown",
                professor.Name ?? "Unknown"
                )
            );
        }
    }

}
