using _250828_universityTask.Models.Dtos;
using MediatR;

namespace _250828_universityTask.Features.Students
{
    public record GetAllStudentsQuery(int ProfessorId) : IRequest<List<StudentDto>>;

}
