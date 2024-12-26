namespace DotnetProjectManagement.ProjectManagement.App.Services;

using Data.Contexts;
using UseCases.Abstractions;

public class TransactionManager(ProjectManagementDbContext dbContext) : ITransactionManager
{
    public async Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var dbContextTransaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        return new Transaction(dbContextTransaction);
    }
}
