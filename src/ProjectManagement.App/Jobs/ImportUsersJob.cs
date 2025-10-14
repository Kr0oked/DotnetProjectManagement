namespace DotnetProjectManagement.ProjectManagement.App.Jobs;

using Quartz;
using Services;
using UseCases.DTOs;
using UseCases.User.Initialization;

public class ImportUsersJob(UserInitializationUseCase useCase, ExternalResourceService externalResourceService) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var users = await externalResourceService.GetUsers(context.CancellationToken);

        foreach (var user in users)
        {
            var actor = new Actor
            {
                UserId = (Guid)user.Id!,
                FirstName = user.FirstName!,
                LastName = user.LastName!,
                IsAdministrator = false
            };
            await useCase.InitializeUserAsync(actor, context.CancellationToken);
        }
    }
}
