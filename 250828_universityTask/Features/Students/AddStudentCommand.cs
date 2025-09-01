using _250828_universityTask.Models.Dtos;
using MediatR;

namespace _250828_universityTask.Features.Students
{
    public record AddStudentCommand(string Name, int ProfessorId) : IRequest<StudentDto>;

}
