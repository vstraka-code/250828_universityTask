using _250828_universityTask.Models.Dtos;
using MediatR;

namespace _250828_universityTask.Features.Students
{
    public record GetStudentQuery(int StudentId, int? ProfessorId = null, int? CurrentStudentId = null) : IRequest<StudentDto>;

}
