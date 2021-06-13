using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Json;
using System;

namespace PewPew.WebApp.Server.Utility
{
	public class ColoredConsoleSink : ILogEventSink
	{
		private static readonly string[] blacklistedProperties = new string[]
		{
			"TraceId",
			"SpanId",
			"ParentId",
			"ConnectionId",
			"ActionId",
			"ActionName",

			"RequestPath",
			"RequestMethod",
			"RequestId",
			"SourceContext"
		};

		private readonly JsonValueFormatter valueFormatter;

		/// <summary>
		/// Construct a <see cref="ColoredConsoleSink"/>, optionally supplying a formatter for
		/// <see cref="LogEventPropertyValue"/> on the event.
		/// </summary>
		/// <param name="valueFormatter">A value formatter, or null.</param>
		public ColoredConsoleSink(JsonValueFormatter? valueFormatter = null)
		{
			this.valueFormatter = valueFormatter ?? new JsonValueFormatter(typeTagName: "$type");
		}

		public void Emit(LogEvent logEvent)
		{
			if (logEvent == null)
			{
				throw new ArgumentNullException(nameof(logEvent));
			}

			var output = Console.Out;

			lock (output)
			{
				Console.ForegroundColor = ConsoleColor.DarkGray;
				output.Write("[");
				output.Write(logEvent.Timestamp.ToString("HH:mm:ss"));
				output.Write("] ");

				output.Write("[");
				Console.ForegroundColor = LogLevelToColor(logEvent.Level);
				output.Write(AbbreviateLogLevel(logEvent.Level));
				Console.ForegroundColor = ConsoleColor.DarkGray;
				output.Write("] ");

				Console.ForegroundColor = ConsoleColor.Gray;
				string message = logEvent.MessageTemplate.Render(logEvent.Properties);
				output.Write(message);

				if (logEvent.Exception != null)
				{
					Console.ForegroundColor = ConsoleColor.DarkRed;
					output.Write("\n");
					output.Write(logEvent.Exception);
				}

				if (logEvent.Properties.TryGetValue("RequestPath", out var requestPathValue))
				{
					logEvent.Properties.TryGetValue("RequestMethod", out var requestMethod);

					Console.ForegroundColor = ConsoleColor.DarkGray;
					output.Write("\n - ");
					Console.ForegroundColor = ConsoleColor.Cyan;
					output.Write("Request");
					Console.ForegroundColor = ConsoleColor.DarkGray;
					output.Write(": ");
					Console.ForegroundColor = ConsoleColor.Yellow;
					output.Write(requestMethod?.ToString()?.Trim('\"') ?? "---");
					output.Write(" ");
					Console.ForegroundColor = ConsoleColor.Gray;
					output.Write(requestPathValue.ToString().Trim('\"'));
				}

				foreach (var property in logEvent.Properties)
				{
					string name = property.Key;
					if (name.Length > 0 && name[0] == '@')
					{
						// Escape first '@' by doubling
						name = '@' + name;
					}

					// Filter out blacklisted properties
					bool shouldInclude = true;
					foreach (string blacklistedProperty in blacklistedProperties)
					{
						if (name == blacklistedProperty)
						{
							shouldInclude = false;
						}
					}

					if (!shouldInclude)
					{
						continue;
					}

					Console.ForegroundColor = ConsoleColor.DarkGray;
					output.Write("\n - ");
					Console.ForegroundColor = ConsoleColor.Cyan;
					output.Write(name);
					Console.ForegroundColor = ConsoleColor.DarkGray;
					output.Write(": ");

					if (property.Value is ScalarValue scalarValue)
					{
						if (scalarValue.Value is int
							|| scalarValue.Value is uint
							|| scalarValue.Value is byte
							|| scalarValue.Value is sbyte
							|| scalarValue.Value is short
							|| scalarValue.Value is ushort
							|| scalarValue.Value is long
							|| scalarValue.Value is ulong
							|| scalarValue.Value is double
							|| scalarValue.Value is decimal
							|| scalarValue.Value is float)
						{
							Console.ForegroundColor = ConsoleColor.Blue;
							valueFormatter.Format(property.Value, output);
						}
						else if (scalarValue.Value is TimeSpan timeSpan)
						{
							Console.ForegroundColor = ConsoleColor.Blue;
							output.Write($"{timeSpan.TotalMilliseconds:###,##0.0}ms");
						}
						else
						{
							Console.ForegroundColor = ConsoleColor.Yellow;
							valueFormatter.Format(property.Value, output);
						}
					}
					else if (property.Value is null)
					{
						Console.ForegroundColor = ConsoleColor.Blue;
						valueFormatter.Format(property.Value, output);
					}
					else
					{
						Console.ForegroundColor = ConsoleColor.Gray;
						valueFormatter.Format(property.Value, output);
					}
				}
				Console.ResetColor();
				output.Write("\n\n");
			}
		}

		private static ConsoleColor LogLevelToColor(LogEventLevel logLevel)
		{
			return logLevel switch
			{
				LogEventLevel.Debug => ConsoleColor.Gray,
				LogEventLevel.Information => ConsoleColor.White,
				LogEventLevel.Warning => ConsoleColor.Yellow,
				LogEventLevel.Error => ConsoleColor.DarkRed,
				LogEventLevel.Fatal => ConsoleColor.Red,
				_ => ConsoleColor.DarkGray,
			};
		}

		private static string AbbreviateLogLevel(LogEventLevel logLevel)
		{
			return logLevel switch
			{
				LogEventLevel.Verbose => "VRB",
				LogEventLevel.Debug => "DBG",
				LogEventLevel.Information => "INF",
				LogEventLevel.Warning => "WRN",
				LogEventLevel.Error => "ERR",
				LogEventLevel.Fatal => "FTL",
				_ => logLevel.ToString(),
			};
		}
	}
}
