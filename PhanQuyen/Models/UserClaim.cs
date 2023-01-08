using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RezUtility.PhanQuyen.Models;

public class UserClaim
{
	[Key]
	public Guid Id { get; set; }
	public Guid UserId { get; set; }
	[Required(AllowEmptyStrings = false)]
	public string Type { get; set; } = string.Empty;
	[Required(AllowEmptyStrings = false)]
	public string Value { get; set; } = string.Empty;
	[ForeignKey(nameof(UserClaim.Id))]
	public User? User { get; set; }
}