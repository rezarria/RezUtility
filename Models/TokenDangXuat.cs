using System.ComponentModel.DataAnnotations;

namespace RezUtility.Models;

public class TokenDangXuat
{
	[Key]
	[Required(AllowEmptyStrings = false)]
	public string Token { get; set; } = string.Empty;
	[Required]
	public TimeSpan Exp { get; set; }
}