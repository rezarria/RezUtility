using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RezUtility.Utilities;
using System.Security.Claims;

namespace RezUtility.Services;

public interface ITokenService
{
	string TaoTokenAsync(ICollection<Claim>? themVao = null);
	bool KiemTraToken(string token);
	ClaimsPrincipal GiaiMaToken(string token);
}

public class TokenServiceOptions
{
	public double ExpiryDurationMinutes { get; set; }
	public string Key { get; set; } = string.Empty;
	public string Issuer { get; set; } = string.Empty;
}

public class TokenService : ITokenService
{
	private readonly double _expiryDurationMinutes;
	private readonly string _issuer;
	private readonly string _key;


	public TokenService(IOptions<TokenServiceOptions> options)
	{
		_expiryDurationMinutes = options.Value.ExpiryDurationMinutes;
		_key = options.Value.Key;
		_issuer = options.Value.Issuer;
	}

	public string TaoTokenAsync(ICollection<Claim>? themVao = null)
	{
		return TokenUtility.TaoTokenAsync(_key, _issuer, _expiryDurationMinutes, themVao);
	}

	public bool KiemTraToken(string token)
	{
		return TokenUtility.KiemTraToken(_key, _issuer, token);
	}

	public ClaimsPrincipal GiaiMaToken(string token)
	{
		return TokenUtility.GiaiMaToken(_key, _issuer, token);
	}
}

public static class TokenServiceExtension
{
	public static IServiceCollection AddTokenService(this IServiceCollection services)
	{
		services.AddTransient<ITokenService, TokenService>();
		return services;
	}
}