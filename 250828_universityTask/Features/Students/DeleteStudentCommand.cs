using MediatR;

namespace _250828_universityTask.Features.Students
{
    public record DeleteStudentCommand(int StudentId, int ProfessorId) : IRequest<bool>;

}
