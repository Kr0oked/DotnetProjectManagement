namespace DotnetProjectManagement.ProjectManagement.App.Services;

using Microsoft.EntityFrameworkCore.Storage;
using UseCases.Abstractions;

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
