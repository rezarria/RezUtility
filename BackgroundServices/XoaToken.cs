using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RezUtility.Services;

namespace RezUtility.BackgroundServices;

public sealed class XoaTokenBackgroundService : BackgroundService
{
	private readonly ILogger<XoaTokenBackgroundService> _logger;
	private readonly ITokenDangXuatService _tokenDangXuat;

	public XoaTokenBackgroundService(
		ILogger<XoaTokenBackgroundService> logger,
		ITokenDangXuatService tokenDangXuat)
	{
		(_logger, _tokenDangXuat) = (logger, tokenDangXuat);
	}

	protected override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation($"{nameof(XoaTokenBackgroundService)} is running.");
		_tokenDangXuat.SyncKhiGoi = true;
		await DoWorkAsync(cancellationToken);
	}

	private async Task DoWorkAsync(CancellationToken cancellationToken)
	{
		Thread.CurrentThread.Name = "Xóa token";
		_logger.LogInformation($"{nameof(XoaTokenBackgroundService)} is working.");
		while (!cancellationToken.IsCancellationRequested)
		{
			_logger.LogInformation("Tiến hành tìm và xóa token cũ");
			await _tokenDangXuat.XoaTuDongAsync(cancellationToken);
			await Task.Delay(5_000, cancellationToken);
		}
	}

	public override async Task StopAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation($"{nameof(XoaTokenBackgroundService)} is stopping.");
		_tokenDangXuat.LuuDatabase();
		await base.StopAsync(cancellationToken);
	}
}