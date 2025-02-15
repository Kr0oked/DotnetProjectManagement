namespace DotnetProjectManagement.ProjectManagement.UseCases.DTOs;

using System.Collections.Immutable;

public record Page<TElement>
{
    public int Size => this.PageRequest.Size;

    public long TotalElements { get; }

    public int TotalPages => this.Size == 0 ? 1 : (int)Math.Ceiling((double)this.TotalElements / this.Size);

    public int Number => this.PageRequest.Number;

    public ImmutableList<TElement> Content { get; }

    private PageRequest PageRequest { get; }

    public Page(ImmutableList<TElement> content, PageRequest pageRequest, long totalElements)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(totalElements);
        this.Content = content;
        this.PageRequest = pageRequest;
        this.TotalElements = totalElements;
    }

    public Page<TMapped> Map<TMapped>(Func<TElement, TMapped> converter)
    {
        var convertedContent = this.Content.Select(converter).ToImmutableList();
        return new Page<TMapped>(convertedContent, this.PageRequest, this.TotalElements);
    }
}
