using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RezUtility.PhanQuyen.Models;

public class User
{
	[Key]
	public Guid Id { get; set; }
	public DateTime LastLogin { get; set; } = DateTime.UtcNow;
	[InverseProperty(nameof(UserClaim.User))]

	public virtual List<UserClaim> Claims { get; set; } = new();
	[InverseProperty(nameof(UserRole.User))]
	public virtual List<UserRole> UserRoles { get; set; } = new();
	[InverseProperty(nameof(ToKen.User))]

	public virtual List<ToKen> Tokens { get; set; } = new();
}