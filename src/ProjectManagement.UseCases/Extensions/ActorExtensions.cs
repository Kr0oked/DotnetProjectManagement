namespace DotnetProjectManagement.ProjectManagement.UseCases.Extensions;

using Domain.Entities;
using DTOs;
using Exceptions;

public static class ActorExtensions
{
    public static void VerifyIsAdministrator(this Actor actor)
    {
        if (!actor.IsAdministrator)
        {
            throw new AdministratorRequiredException(actor);
        }
    }

    public static void VerifyIsManager(this Actor actor, Project project)
    {
        if (actor.IsAdministrator)
        {
            return;
        }

        if (project.GetRoleOfUser(actor.UserId) != ProjectMemberRole.Manager)
        {
            throw new ManagerRequiredException(actor, project.Id);
        }
    }

    public static void VerifyIsProjectMember(this Actor actor, Project project)
    {
        if (actor.IsAdministrator)
        {
            return;
        }

        if (project.GetRoleOfUser(actor.UserId) is null)
        {
            throw new ProjectMemberException(actor.UserId, project.Id);
        }
    }
}
