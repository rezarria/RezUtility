using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RezUtility.Contexts;
using RezUtility.Models;
using System.Collections.Concurrent;

namespace RezUtility.Services;

public interface ITokenDangXuatService
{
	int SoLuongToiDa { get; set; }
	bool SyncKhiGoi { get; set; }
	void ThemToken(TokenDangXuat token);
	bool KiemTra(string token);
	Task XoaTuDongAsync(CancellationToken cancellationToken = default);
	void LuuDatabase();
}

public class TokenDangXuatService : ITokenDangXuatService
{
	private readonly ConcurrentStack<TokenDangXuat> _danhSachBoNhoTrong;
	private readonly object _danhSachBoNhoTrongLock = new();
	private readonly ConcurrentStack<TokenDangXuat> _danhSachTokenTrongDatabase;
	private readonly ILogger _logger;
	private readonly IServiceProvider _serviceProvider;

	public TokenDangXuatService(IServiceProvider serviceProvider, ILogger<TokenDangXuatService> logger)
	{
		_serviceProvider = serviceProvider;
		using IServiceScope scope = _serviceProvider.CreateScope();
		XacThucDbContext dbContext = scope.ServiceProvider.GetRequiredService<XacThucDbContext>();
		_danhSachBoNhoTrong = new ConcurrentStack<TokenDangXuat>();
		_danhSachTokenTrongDatabase = new ConcurrentStack<TokenDangXuat>(dbContext.TokenDangXuat.AsNoTracking());
		_logger = logger;
	}
	public void LuuDatabase()
	{
		_logger.LogInformation("Lưu token từ ram vào database");
		using IServiceScope scope = _serviceProvider.CreateScope();
		XacThucDbContext dbContext = scope.ServiceProvider.GetRequiredService<XacThucDbContext>();
		lock (_danhSachBoNhoTrongLock)
		{
			dbContext.AddRange(_danhSachBoNhoTrong);
			dbContext.SaveChanges();
		}
	}
	public int SoLuongToiDa { get; set; } = 1000;

	public bool KiemTra(string token)
	{
		_logger.LogInformation("Kiểm tra token");
		lock (_danhSachBoNhoTrongLock)
		{
			return _danhSachBoNhoTrong.Any(x => x.Token.Equals(token)) ||
				   _danhSachTokenTrongDatabase.Any(x => x.Token.Equals(token));
		}
	}

	public bool SyncKhiGoi { get; set; } = false;

	public void ThemToken(TokenDangXuat token)
	{
		lock (_danhSachBoNhoTrongLock)
		{
			if (_danhSachBoNhoTrong.Any(x => x.Token.SequenceEqual(token.Token)))
				return;
			_logger.LogInformation("Thêm token vào hàng chờ | {Token}", token.Token);
			_danhSachBoNhoTrong.Push(token);
		}
	}

	public async Task XoaTuDongAsync(CancellationToken cancellationToken = default)
	{
		TokenDangXuat? token;
		lock (_danhSachBoNhoTrongLock)
		{
			while (_danhSachBoNhoTrong.TryPeek(out token))
			{
				if (token.Exp > DateTime.UtcNow.Subtract(DateTime.UnixEpoch))
					break;
				_danhSachBoNhoTrong.TryPop(out _);
				_logger.LogInformation("Xóa token đăng xuất trong bộ nhớ");
			}
		}

		List<TokenDangXuat> danhSachXoa = new();
		while (_danhSachTokenTrongDatabase.TryPeek(out token))
		{
			if (token.Exp > DateTime.UtcNow.Subtract(DateTime.UnixEpoch))
				break;
			danhSachXoa.Add(token);
			while (!_danhSachTokenTrongDatabase.TryPop(out _)) {}
		}

		if (danhSachXoa.Any())
		{
			_logger.LogInformation("Xóa token đăng xuất trong database");
			using IServiceScope scope = _serviceProvider.CreateScope();
			XacThucDbContext dbContext = scope.ServiceProvider.GetRequiredService<XacThucDbContext>();
			dbContext.RemoveRange(danhSachXoa);
			await dbContext.SaveChangesAsync(cancellationToken);
		}
	}
}

public static class ToKenDangXuatServiceExtension
{
	public static IServiceCollection AddTokenDangXuat(this IServiceCollection service)
	{
		service.AddSingleton<ITokenDangXuatService, TokenDangXuatService>();
		return service;
	}
}