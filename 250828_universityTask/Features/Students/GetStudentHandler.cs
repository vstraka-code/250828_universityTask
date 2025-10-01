using _250828_universityTask.Cache;
using _250828_universityTask.Models.Dtos;
using MediatR;

namespace _250828_universityTask.Features.Students
{
    public class GetStudentHandler : IRequestHandler<GetStudentQuery, StudentDto>
    {
        //private readonly AppDbContext _db;
        private readonly CacheServiceWithoutExtension _cacheService;

        public GetStudentHandler(CacheServiceWithoutExtension cacheService)
        {
            //_db = db;
            _cacheService = cacheService;
        }

        public Task<StudentDto> Handle(GetStudentQuery req, CancellationToken cancellationToken)
        {
            // var students = await _cacheService.AllStudents();
            var students = _cacheService.AllStudents();

            var student = students.FirstOrDefault(p => p.Id == req.StudentId);

            if (student == null) throw new KeyNotFoundException();

            if (req.ProfessorId != null)
            {
                // var professors = await _cacheService.AllProfessors();
                var professors = _cacheService.AllProfessors();

                var professor = professors.FirstOrDefault(p => p.Id == req.ProfessorId);

                if (professor == null || student.UniversityId != professor.UniversityId) throw new UnauthorizedAccessException();

            }
            else if (req.CurrentStudentId != null)
            {
                if (student.Id != req.CurrentStudentId) throw new UnauthorizedAccessException();
            }
            else
            {
                throw new UnauthorizedAccessException();
            }

            return Task.FromResult(
                new StudentDto(
                student.Id,
                student.Name,
                student.University?.Name,
                student.ProfessorAdded?.Name
                )
            );
        }
    }
}
