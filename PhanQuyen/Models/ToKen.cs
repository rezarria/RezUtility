using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RezUtility.PhanQuyen.Models;

public class ToKen
{
	[Key]
	[Required(AllowEmptyStrings = false)]
	public string Token { get; set; } = string.Empty;
	[Required]
	public Guid UserId { get; set; }
	[ForeignKey(nameof(UserId))]
	public User? User { get; set; }
}