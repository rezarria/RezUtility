using System.Security.Claims;

namespace RezUtility.Utilities;

public static class ClaimUtility
{
	public static string Get(this ClaimsPrincipal claimsPricipal, string claimType)
	{
		string value = string.Empty;
		if (claimsPricipal.Claims.Any(x => x.Type == claimType))
		{
			Claim claim = claimsPricipal.Claims.First(x => x.Type == claimType);
			value = claim.Value;
		}
		return value;
	}
}