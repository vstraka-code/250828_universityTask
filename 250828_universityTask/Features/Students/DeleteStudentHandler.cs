using _250828_universityTask.Data;
using MediatR;

namespace _250828_universityTask.Features.Students
{
    public class DeleteStudentHandler : IRequestHandler<DeleteStudentCommand, bool>
    {
        private readonly AppDbContext _db;

        public DeleteStudentHandler(AppDbContext db) => _db = db;
        public async Task<bool> Handle(DeleteStudentCommand req, CancellationToken cancellationToken)
        {
            var student = await _db.Students.FindAsync(req.StudentId);
            if (student == null) return false;

            var professor = await _db.Professors.FindAsync(req.ProfessorId);
            if (professor == null || student.UniversityId != professor.UniversityId)
                throw new UnauthorizedAccessException();

            _db.Students.Remove(student);
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
