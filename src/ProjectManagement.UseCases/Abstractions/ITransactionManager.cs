namespace DotnetProjectManagement.ProjectManagement.UseCases.Abstractions;

public interface ITransactionManager
{
    public Task<ITransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
