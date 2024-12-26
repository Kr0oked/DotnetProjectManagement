namespace DotnetProjectManagement.ProjectManagement.UseCases.Abstractions;

public interface ITransaction : IAsyncDisposable
{
    public Task CommitAsync(CancellationToken cancellationToken = default);
}
