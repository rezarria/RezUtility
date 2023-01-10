using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RezUtility.PhanQuyen.Models;

public class UserRole
{
	[Key]
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	public Guid RoleId { get; set; }
	public bool Active { get; set; }
	[ForeignKey(nameof(UserId))]
	public User? User { get; set; }
	[ForeignKey(nameof(RoleId))]
	public Role? Role { get; set; }
}