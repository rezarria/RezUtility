using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using RezUtility.Services;

namespace RezUtility.Middlewares;

public class XacThucMiddleware
{
	private readonly ILogger _logger;
	private readonly RequestDelegate _next;
	private readonly ITokenDangXuatService _tokenDangXuat;

	public XacThucMiddleware(RequestDelegate next, ILogger<XacThucMiddleware> logger, ITokenDangXuatService tokenDangXuat)
	{
		_next = next;
		_logger = logger;
		_tokenDangXuat = tokenDangXuat;
	}

	public async Task InvokeAsync(HttpContext context)
	{
		if (context.Request.Headers.ContainsKey("Authorization"))
			if (context.Request.Headers.TryGetValue("Authorization", out StringValues values))
				if (values.Any())
				{
					string token = values.First()!["Bearer ".Length..];
					if (_tokenDangXuat.KiemTra(token))
					{
						_logger.LogWarning("Token đăng xuất đang cố truy cập");
						context.Response.StatusCode = StatusCodes.Status423Locked;
						return;
					}
				}
		await _next(context);
	}
}

public static class XacThucMiddlewareExtensions
{
	public static IApplicationBuilder UseXacThuc(
		this IApplicationBuilder builder)
	{
		return builder.UseMiddleware<XacThucMiddleware>();
	}
}