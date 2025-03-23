namespace DotnetProjectManagement.ProjectManagement.UseCases.Mappers;

using Domain.Entities;
using DTOs;

public static class UserExtensions
{
    public static UserDto ToDto(this User user) => new()
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName
    };
}
