using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RezUtility.Contexts;
using System.Collections;
using System.Reflection;

namespace RezUtility.Tasks;

public class CheckTask
{
	private readonly IHostApplicationLifetime _hostApplicationLifetimelifeTime;
	private readonly ILogger _logger;
	private readonly IServiceProvider _serviceProvider;

	private CheckTask(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
		_logger = serviceProvider.GetRequiredService<ILogger<CheckTask>>();
		_hostApplicationLifetimelifeTime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();
	}

	public static void Check(IServiceProvider serviceProvider)
	{
		CheckTask checkTask = new(serviceProvider);
		checkTask.ExecuteAsync().Wait();
	}

	public Task ExecuteAsync(CancellationToken cancellationToken = default)
	{
		using IServiceScope scope = _serviceProvider.CreateScope();
		_logger.LogInformation("Bắt đầu kiểm tra...");
		Task<bool>[] tasks = KiemTraToanBoDbContext(scope, cancellationToken);
		bool needShutDown = false;
		if (tasks.Any())
		{
			Task.WhenAll(tasks).Wait(cancellationToken);
			needShutDown = tasks.All(x => x.Result);
		}
		if (!needShutDown)
			return Task.CompletedTask;
		_logger.LogError("Đóng chương trình....");
		_hostApplicationLifetimelifeTime.StopApplication();
		return Task.CompletedTask;
	}

	/// <summary>
	/// </summary>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
	private Task<bool>[] KiemTraToanBoDbContext(IServiceScope scope, CancellationToken cancellationToken = default)
	{
		object? rootProvider = LayGiaTriProperty(scope, "RootProvider");
		object? callSiteValidator = LayGiaTriField(rootProvider, "_callSiteValidator");
		IDictionary? data = LayGiaTriField(callSiteValidator, "_scopedServices") as IDictionary;
		ICollection<Type>? types = data?.Keys as ICollection<Type>;
		List<Type> tasks = (types ?? throw new InvalidOperationException())
						   .Where(x => x.BaseType is not null && (typeof(DbContext) == x.BaseType || typeof(IdentityDbContext) == x.BaseType) || x.GetInterfaces().Contains(typeof(IXacThucDbContext)))
						   .ToList();
		Task<bool>[] job = tasks.Select(x => KiemTraDatabase((DbContext)scope.ServiceProvider.GetRequiredService(x), cancellationToken)).ToArray();
		return job;
	}

	private static object? LayGiaTriField(object? obj, string name)
	{
		return obj?.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(obj);
	}

	private static object? LayGiaTriProperty(object obj, string name)
	{
		return obj.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance)?.GetGetMethod(nonPublic: true)?.Invoke(obj, null);
	}

	private async Task<bool> KiemTraDatabase(DbContext context, CancellationToken cancellationToken = default)
	{
		_logger.LogInformation("Kiểm tra database {Database}", context.Database.GetDbConnection().Database);
		bool needShutDown = false;

		if (await context.Database.CanConnectAsync(cancellationToken))
		{
			_logger.LogInformation("Kết nối ổn ✔");
			_logger.LogInformation("Kiểm tra sự tồn tại của database");
			if (
				!(context.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator)!.Exists()
			)
			{
				_logger.LogError("Database không tồn tại ❌");
				_logger.LogError("Bắt đầu khởi tạo database");

				try
				{
					await context.Database.EnsureCreatedAsync(cancellationToken);
				}
				catch (Exception)
				{
					_logger.LogError("Tạo database thất bại ❌");
					needShutDown = true;
				}

				_logger.LogInformation("Tạo database thành công ✔");
			}
		}
		else
		{
			_logger.LogError("Không thể kết nối ❌{NewLine}Bắt đầu khởi tạo database...", Environment.NewLine);
			try
			{
				await context.Database.EnsureCreatedAsync(cancellationToken);
				_logger.LogInformation("Tạo database thành công ✔");
			}
			catch (Exception e)
			{
				_logger.LogError("Tạo database thất bại ❌");
				_logger.LogError(e.Message);
				needShutDown = true;
			}
		}

		return needShutDown;
	}
}