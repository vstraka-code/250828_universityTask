using _250828_universityTask.Cache;
using _250828_universityTask.Data;
using _250828_universityTask.Exceptions;
using _250828_universityTask.Helpers;
using _250828_universityTask.Models;
using _250828_universityTask.Models.Dtos;
using MediatR;

namespace _250828_universityTask.Features.Professors
{
    public class AddProfessorHandler : IRequestHandler<AddProfessorCommand, ProfessorDto>
    {
        private readonly CacheServiceWithoutExtension _cacheService;
        private readonly IJsonDbContext _json;
        private readonly GenerateIdExtension _generateIdExtension;

        public AddProfessorHandler(CacheServiceWithoutExtension cacheService, IJsonDbContext json, GenerateIdExtension generateIdExtension)
        {
            _cacheService = cacheService;
            _json = json;
            _generateIdExtension = generateIdExtension;
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

            // var id = (_json.Professors.Any() ? _json.Professors.Max(s => s.Id) : 0) + 1;
            var id = _generateIdExtension.GenerateIdProfessor();

            var professor = new Professor
            {
                Id = id,
                Email = "example@example.com",
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
                professor.Email,
                professor.Name,
                university.Name ?? "Unkown"
                )
            );
        }
    }
}
