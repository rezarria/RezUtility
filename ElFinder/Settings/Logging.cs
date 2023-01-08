using Castle.DynamicProxy;
using elFinder.Net.Core;
using elFinder.Net.Core.Plugins;
using elFinder.Net.Drivers.FileSystem;
using Microsoft.Extensions.DependencyInjection;
using RezUtility.ElFinder.Services;

namespace RezUtility.ElFinder.Settings;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddElFinderLogging(this IServiceCollection services, PluginCollection collection,
														Type? connectorType = null,
														Type? driverType = null)
	{
		services.AddScoped<LoggingInterceptor>();

		collection.Captures.Add(new PluginCapture
							    {
								    ImplType = connectorType ?? typeof(Connector),
								    Type = typeof(IConnector),
								    CaptureFunc = (provider, service) =>
											      {
												      LoggingInterceptor interceptor = provider.GetRequiredService<LoggingInterceptor>();
												      IConnector? proxy = new ProxyGenerator().CreateInterfaceProxyWithTarget(service as IConnector, interceptor);
												      return proxy;
											      }
							    });

		collection.Captures.Add(new PluginCapture
							    {
								    ImplType = driverType ?? typeof(FileSystemDriver),
								    Type = typeof(IDriver),
								    CaptureFunc = (provider, service) =>
											      {
												      LoggingInterceptor interceptor = provider.GetRequiredService<LoggingInterceptor>();
												      IDriver? proxy = new ProxyGenerator().CreateInterfaceProxyWithTarget(service as IDriver, interceptor);
												      return proxy;
											      }
							    });

		return services;
	}
}