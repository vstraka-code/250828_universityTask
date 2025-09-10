using _250828_universityTask.Cache;
using _250828_universityTask.Data;
using _250828_universityTask.Exceptions;
using _250828_universityTask.Models.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace _250828_universityTask.Features.Students
{
    public class UpdateStudentHandler : IRequestHandler<UpdateStudentCommand, StudentDto>
    {
        // private readonly AppDbContext _db;
        private readonly CacheService _cacheService;
        private readonly JsonDbContext _json;

        public UpdateStudentHandler(JsonDbContext json, CacheService cacheService)
        {
            // _db = db;
            _cacheService = cacheService;
            _json = json;
        }

        public Task<StudentDto> Handle(UpdateStudentCommand req, CancellationToken cancellationToken)
        {
            // var professors = await _cacheService.AllProfessors();
            var professors = _cacheService.AllProfessors();

            var professor = professors.FirstOrDefault(p => p.Id == req.ProfessorId);

            if (professor == null)
                throw new UnauthorizedAccessException();

            // var students = await _cacheService.AllStudents();
            var students = _cacheService.AllStudents();

            var duplicate = students.Any(s => s.Id != req.StudentId && s.Name == req.Name && s.UniversityId == professor.UniversityId);

            if (duplicate)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Name", new[] { "A student with this name already exists in the university." } }
                });
            }

            var student = students.FirstOrDefault(p => p.Id == req.StudentId);

            if (student == null) throw new KeyNotFoundException();

            if (student.UniversityId != professor.UniversityId)
                throw new UnauthorizedAccessException();

            student.Name = req.Name;
            //await _db.SaveChangesAsync(cancellationToken);
            _json.Save();

            _cacheService.ClearStudentsCache();

            return Task.FromResult(
                new StudentDto(
                student.Id,
                student.Name,
                student.University?.Name ?? "Unknown",
                student.ProfessorAdded?.Name ?? "Unknown"
                )
            );
        }
    }
}
