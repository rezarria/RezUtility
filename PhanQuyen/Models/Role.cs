using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RezUtility.PhanQuyen.Models;

public class Role
{
	[Key]
	public Guid Id { get; set; }
	[Required(AllowEmptyStrings = false)]
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }
	public virtual List<RoleClaim> Claims { get; set; } = new();
	[InverseProperty(nameof(UserRole.Role))]
	public virtual List<UserRole> UserRoles { get; set; } = new();
}