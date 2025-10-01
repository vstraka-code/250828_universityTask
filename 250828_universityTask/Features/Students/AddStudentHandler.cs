using _250828_universityTask.Cache;
using _250828_universityTask.Data;
using _250828_universityTask.Exceptions;
using _250828_universityTask.Helpers;
using _250828_universityTask.Models;
using _250828_universityTask.Models.Dtos;
using MediatR;

namespace _250828_universityTask.Features.Students
{
    public class AddStudentHandler : IRequestHandler<AddStudentCommand, StudentDto>
    {
        // private readonly AppDbContext _db;
        private readonly CacheServiceWithoutExtension _cacheService;
        private readonly IJsonDbContext _json;
        private readonly GenerateIdExtension _generateIdExtension;

        // constructor for AddStudentHandler => receives AppDbContext as parameter (db) from Dependency Injection (DI)
        public AddStudentHandler(CacheServiceWithoutExtension cacheService, IJsonDbContext json, GenerateIdExtension generateIdExtension)
        {
            // _db = db;
            _cacheService = cacheService;
            _json = json;
            _generateIdExtension = generateIdExtension;
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

            // checks are there any students and finds the max + 1
            // var id = (_json.Students.Any() ? _json.Students.Max(s => s.Id) : 0) + 1;
            var id = _generateIdExtension.GenerateIdStudent();

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
