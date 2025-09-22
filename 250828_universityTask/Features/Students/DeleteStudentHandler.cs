using _250828_universityTask.Cache;
using _250828_universityTask.Data;
using MediatR;

namespace _250828_universityTask.Features.Students
{
    public class DeleteStudentHandler : IRequestHandler<DeleteStudentCommand, bool>
    {

        // private readonly AppDbContext _db;
        private readonly CacheServiceWithoutExtension _cacheService;
        private readonly IJsonDbContext _json;

        public DeleteStudentHandler(IJsonDbContext json, CacheServiceWithoutExtension cacheService)
        {
            // _db = db;
            _cacheService = cacheService;
            _json = json;
        }

        public Task<bool> Handle(DeleteStudentCommand req, CancellationToken cancellationToken)
        {
            var students = _cacheService.AllStudents();
            // var students = await _cacheService.AllStudents();
            var student = students.FirstOrDefault(p => p.Id == req.StudentId);

            if (student == null) throw new UnauthorizedAccessException();

            // var professors = await _cacheService.AllProfessors();
            var professors = _cacheService.AllProfessors();

            var professor = professors.FirstOrDefault(p => p.Id == req.ProfessorId);

            if (professor == null || student.UniversityId != professor.UniversityId)
                throw new UnauthorizedAccessException();

            // _db.Students.Remove(student);
            _json.Students.Remove(student);
            _json.Save();

            // await _db.SaveChangesAsync(cancellationToken);

            _cacheService.ClearStudentsCache();

            return Task.FromResult(true);
        }
    }
}
