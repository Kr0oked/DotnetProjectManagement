namespace DotnetProjectManagement.ProjectManagement.Web.Models;

using System.Collections.Immutable;
using System.ComponentModel;

public record PageRepresentation<T>
{
    [Description("Size of the page.")]
    public required int Size { get; init; }

    [Description("Total number of elements.")]
    public required long TotalElements { get; init; }

    [Description("Total number of pages.")]
    public required int TotalPages { get; init; }

    [Description("Numfer of the current page. Is always non-negative.")]
    public required int Number { get; init; }

    [Description("Page content as a list.")]
    public required ImmutableList<T> Content { get; init; }
}
