using _250828_universityTask.Cache;
using _250828_universityTask.Data;
using _250828_universityTask.Exceptions;
using _250828_universityTask.Features.Students;
using _250828_universityTask.Models;
using _250828_universityTask.Models.Dtos;
using MediatR;

namespace _250828_universityTask.Features.Professors
{
    public class AddProfessorHandler : IRequestHandler<AddProfessorCommand, ProfessorDto>
    {
        private readonly CacheServiceWithoutExtension _cacheService;
        private readonly IJsonDbContext _json;

        public AddProfessorHandler(CacheServiceWithoutExtension cacheService, IJsonDbContext json)
        {
            _cacheService = cacheService;
            _json = json;
        }

        public Task<ProfessorDto> Handle(AddProfessorCommand req, CancellationToken cancellationToken)
        {
            var professors = _cacheService.AllProfessors();

            var duplicate = professors.Any(p => p.Name == req.Name && p.UniversityId == req.UniId);

            if (duplicate)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Name", new[] { "A professor with this name already exists in the university." } }
                });
            }

            var id = (_json.Professors.Any() ? _json.Professors.Max(s => s.Id) : 0) + 1;

            var professor = new Professor
            {
                Id = id,
                Name = req.Name,
                UniversityId = req.UniId,
            };

            var university = _json.Universities.FirstOrDefault(u => u.Id == req.UniId);

            if (university == null)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "UniversityId", new[] { "There is no university with this name." } }
                });
            }

            _json.Professors.Add(professor);
            _json.Save();

            _cacheService.ClearProfessorCache();

            return Task.FromResult(
                new ProfessorDto(
                professor.Id,
                professor.Name,
                university.Name ?? "Unkown"
                )
            );
        }
    }
}
