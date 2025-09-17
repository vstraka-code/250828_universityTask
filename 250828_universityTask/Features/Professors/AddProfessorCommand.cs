using _250828_universityTask.Models.Dtos;
using MediatR;

namespace _250828_universityTask.Features.Professors
{
    public record AddProfessorCommand(string Name, int UniId) : IRequest<ProfessorDto>;
}
