namespace DotnetProjectManagement.ProjectManagement.Domain.Entities;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
public class User
{
    public Guid Id { get; set; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DisplayNameMaxLength)]
    public required string FirstName { get; set; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(Constants.Text.DisplayNameMaxLength)]
    public required string LastName { get; set; }
}
