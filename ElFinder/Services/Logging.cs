using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using System.Text;

namespace RezUtility.ElFinder.Services;

public class LoggingInterceptor : IInterceptor
{
	private readonly ILogger _logger;

	public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
	{
		_logger = logger;
	}

	public void Intercept(IInvocation invocation)
	{
		StringBuilder builder = new();
		builder.AppendLine($"Calling: ${invocation.Method.Name}");
		if (invocation.Arguments?.Length > 0)
			for (int i = 0; i < invocation.Arguments.Length; i++)
			{
				builder.AppendLine($"Arg: ${invocation.Arguments[i]}");
				builder.AppendLine($"Val: ${invocation.GetArgumentValue(i)}");
			}
		invocation.Proceed();
		builder.AppendLine($"Return: {invocation.ReturnValue}");
		_logger.LogDebug(builder.ToString());
	}
}