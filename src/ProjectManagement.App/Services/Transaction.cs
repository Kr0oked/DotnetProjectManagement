namespace DotnetProjectManagement.ProjectManagement.App.Services;

using UseCases.Abstractions;
using Microsoft.EntityFrameworkCore.Storage;

public class Transaction(IDbContextTransaction dbContext) : ITransaction
{
    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return dbContext.DisposeAsync();
    }

    public Task CommitAsync(CancellationToken cancellationToken = default) =>
        dbContext.CommitAsync(cancellationToken);
}
