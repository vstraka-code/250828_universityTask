using _250828_universityTask.Cache;
using _250828_universityTask.Data;
using _250828_universityTask.Models.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace _250828_universityTask.Features.Students
{
    public class GetAllStudentsHandler : IRequestHandler<GetAllStudentsQuery, List<StudentDto>>
    {
        //private readonly AppDbContext _db;
        private readonly CacheServiceWithoutExtension _cacheService;

        public GetAllStudentsHandler(CacheServiceWithoutExtension cacheService)
        {
            //_db = db;
            _cacheService = cacheService;
        }

        public Task<List<StudentDto>> Handle(GetAllStudentsQuery req, CancellationToken cancellationToken)
        {
            // var professors = await _cacheService.AllProfessors();
            var professors = _cacheService.AllProfessors();
            var professor = professors.FirstOrDefault(p => p.Id == req.ProfessorId);

            if (professor == null) throw new UnauthorizedAccessException();

            var studentsList = _cacheService.AllStudents() ?? throw new KeyNotFoundException();
            // var studentsList = await _cacheService.AllStudents();

            var students = studentsList
                .Where(s => s.UniversityId == professor.UniversityId)
                .Select(s => new StudentDto(
                    s.Id,
                    s.Name,
                    s.University != null ? s.University.Name : null,
                    s.ProfessorAdded != null ? s.ProfessorAdded.Name : null
                ))
                .ToList();

            return Task.FromResult(students);
        }
    }
}
