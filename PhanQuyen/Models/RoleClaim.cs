using System.ComponentModel.DataAnnotations;

namespace RezUtility.PhanQuyen.Models;

public class RoleClaim
{
	[Key]
	public Guid Id { get; set; }
	[Required(AllowEmptyStrings = false)]
	public string Type { get; set; } = string.Empty;
	[Required(AllowEmptyStrings = false)]
	public string Value { get; set; } = string.Empty;
}