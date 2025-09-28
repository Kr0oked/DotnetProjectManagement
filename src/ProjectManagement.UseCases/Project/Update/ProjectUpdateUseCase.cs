namespace DotnetProjectManagement.ProjectManagement.UseCases.Project.Update;

using System.ComponentModel.DataAnnotations;
using Abstractions;
using Domain.Actions;
using Domain.Entities;
using DTOs;
using Exceptions;
using Extensions;
using Microsoft.Extensions.Logging;

public class ProjectUpdateUseCase(
    IProjectRepository projectRepository,
    IUserRepository userRepository,
    ITransactionManager transactionManager,
    ILogger<ProjectUpdateUseCase> logger)
{
    public async Task<ProjectDto> UpdateProjectAsync(
        Actor actor,
        ProjectUpdateCommand command,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await transactionManager.BeginTransactionAsync(cancellationToken);
        var project = await this.GetProject(command, cancellationToken);

        await this.VerifyUsersExist(command, cancellationToken);
        actor.VerifyIsManager(project);
        project.VerifyProjectIsNotArchived();

        await this.UpdateEntityAsync(actor, command, project, cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        logger.LogProjectUpdated(actor.UserId, project);
        return project.ToDto();
    }

    private async Task<Project> GetProject(ProjectUpdateCommand command, CancellationToken cancellationToken) =>
        await projectRepository.FindOneAsync(command.ProjectId, cancellationToken)
        ?? throw new ProjectNotFoundException(command.ProjectId);

    private async Task VerifyUsersExist(ProjectUpdateCommand command, CancellationToken cancellationToken)
    {
        foreach (var member in command.Members)
        {
            if (!await userRepository.ExistsAsync(member.Key, cancellationToken))
            {
                throw new UserNotFoundException(member.Key);
            }
        }
    }

    private async Task UpdateEntityAsync(
        Actor actor,
        ProjectUpdateCommand command,
        Project project,
        CancellationToken cancellationToken)
    {
        project.DisplayName = command.DisplayName;
        project.Members = command.Members.ToDictionary();

        Validator.ValidateObject(project, new ValidationContext(project), true);

        await projectRepository.SaveAsync(project, ProjectAction.Update, actor.UserId, cancellationToken);
    }
}
