using System;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace LostInSpace.WebApp.Shared.Utility
{
	public static class ExceptionExtensions
	{
		private static readonly Regex lineInterpreter = new Regex("^(at .*?) in (.*):line (\\d+)$");

		public static void Format(this Exception exception, StringBuilder stringBuilder)
		{
			// Example: "InvalidOperationException: Message from exception"
			stringBuilder.Append(exception.GetType().Name);
			stringBuilder.Append(": ");

			stringBuilder.Append(exception.Message);

			stringBuilder.Append('\n');

			// Example: "   at Namespace.Type.Method() (at Path/File.cs:130)"
			string stackTrace = exception.StackTrace;
			if (stackTrace != null)
			{
				string[] stackTraceLines = stackTrace.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

				for (int i = 0; i < stackTraceLines.Length; i++)
				{
					string line = stackTraceLines[i];
					line = line.TrimStart(' ');

					if (exception.GetType() == typeof(TargetInvocationException))
					{
						if (line.StartsWith("at System.RuntimeMethodHandle.InvokeMethod", StringComparison.Ordinal) ||
							line.StartsWith("at System.Reflection.RuntimeMethodInfo.Invoke", StringComparison.Ordinal) ||
							line.StartsWith("at System.Reflection.MethodBase.Invoke", StringComparison.Ordinal))
						{
							continue;
						}
					}
					string stackLine = FormatStackLine(line);
					if (stackLine != null)
					{
						stringBuilder.Append("   ");
						stringBuilder.Append(stackLine);
						stringBuilder.Append('\n');
					}
				}
			}

			if (exception.GetType() == typeof(AggregateException))
			{
				// Example: "Wrapping InvalidOperationException: Message from exception"
				var aggregateException = (AggregateException)exception;
				for (int i = 0; i < aggregateException.InnerExceptions.Count; i++)
				{
					var innerException = aggregateException.InnerExceptions[i];

					stringBuilder.Append("Inner Exception #");
					stringBuilder.Append(i);
					stringBuilder.Append(" ");
					innerException.Format(stringBuilder);
				}
			}
			else if (exception.GetType() == typeof(TargetInvocationException))
			{
				var targetInvocationException = (TargetInvocationException)exception;

				targetInvocationException.InnerException.Format(stringBuilder);
			}
			else
			{
				// Example: "Caused by InvalidOperationException: Message from exception"
				var innerException = exception.InnerException;
				if (innerException != null)
				{
					stringBuilder.Append("Caused by ");
					innerException.Format(stringBuilder);
				}
			}
		}

		public static string Format(this Exception exception)
		{
			var stringBuilder = new StringBuilder();

			exception.Format(stringBuilder);

			return stringBuilder.ToString();
		}

		private static string FormatStackLine(string line)
		{
			// Remove Stack Lines cased by async methods
			if (line.StartsWith("at System.Runtime.CompilerServices.ConfiguredTaskAwaitable`1+ConfiguredTaskAwaiter[TResult].GetResult", StringComparison.Ordinal) ||
				line.StartsWith("at System.Runtime.CompilerServices.ConfiguredTaskAwaitable+ConfiguredTaskAwaiter.GetResult", StringComparison.Ordinal) ||
				line.StartsWith("at System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification", StringComparison.Ordinal) ||
				line.StartsWith("at System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess", StringComparison.Ordinal) ||
				line.StartsWith("at System.Runtime.CompilerServices.TaskAwaiter.ValidateEnd", StringComparison.Ordinal) ||
				line.StartsWith("at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw", StringComparison.Ordinal) ||
				line.StartsWith("at System.Threading.Tasks.TaskFactory", StringComparison.Ordinal))
			{
				return null;
			}

			// Remove Stack Lines from the Startup.cs wrapping
			if (line.StartsWith("at Microsoft.AspNetCore.Hosting.", StringComparison.Ordinal)
				|| line.StartsWith("at Microsoft.Extensions.Hosting.", StringComparison.Ordinal))
			{
				return null;
			}

			// Normalise calls in callstack
			var match = lineInterpreter.Match(line);
			if (match.Success)
			{
				string method = match.Groups[1].Value;
				string filePath = match.Groups[2].Value;
				string lineNumber = match.Groups[3].Value;

				int methodSeperator = method.LastIndexOf('.');
				string typeName = method.Substring(0, methodSeperator);
				int typeSeperator = typeName.LastIndexOf('.');
				string shortName = typeSeperator == -1
					? method.Substring(3)
					: method.Substring(typeSeperator + 1);

				return $"at {shortName} in {filePath}:{lineNumber}";
			}

			int removeAfterIndex = line.IndexOf("in <");
			if (removeAfterIndex != -1)
			{
				line = line.Substring(0, removeAfterIndex);
			}

			return line;
		}
	}
}
