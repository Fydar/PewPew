using LostInSpace.WebApp.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostInSpace.WebApp.Shared.Services.Network
{
	public class NetworkLog
	{
		private readonly NetworkLogChannel channel;
		private readonly Dictionary<string, object> internalProperties;

		public string Name { get; }
		public string Result { get; private set; }

		public Exception Exception { get; private set; }
		public LogLevel Level { get; private set; }
		public DateTimeOffset? StartTime { get; private set; }
		public DateTimeOffset? EndTime { get; private set; }

		public IReadOnlyDictionary<string, object> Properties => internalProperties;

		public TimeSpan ElapsedTime
		{
			get
			{
				if (!StartTime.HasValue)
				{
					return TimeSpan.FromTicks(0);
				}

				DateTimeOffset endTime;
				if (!EndTime.HasValue)
				{
					endTime = DateTimeOffset.UtcNow;
				}
				else
				{
					endTime = EndTime.Value;
				}
				return endTime - StartTime.Value;
			}
		}

		internal NetworkLog(NetworkLogChannel channel, string name)
		{
			this.channel = channel;
			Level = LogLevel.None;
			Name = name;

			StartTime = null;
			EndTime = null;

			internalProperties = new Dictionary<string, object>();
		}

		public NetworkLog PushProperty(string key, object value)
		{
			internalProperties.Add(key, value);
			return this;
		}

		public NetworkLog Start()
		{
			if (StartTime != null)
			{
				throw new InvalidOperationException($"Attempting to start a {nameof(NetworkLog)} that has already been started.");
			}
			StartTime = DateTimeOffset.UtcNow;
			return this;
		}

		public void End(LogLevel level, string result = null, Exception exception = null)
		{
			if (EndTime != null)
			{
				throw new InvalidOperationException($"Attempting to end a {nameof(NetworkLog)} that has already been ended.");
			}
			EndTime = DateTimeOffset.UtcNow;

			Level = level;
			Result = result;
			Exception = exception;

			channel.InvokeOnComplete(this);
		}

		public override string ToString()
		{
			var sb = new StringBuilder();

			sb.Append("[");
			sb.Append(Level.ToString());
			sb.Append("] ");

			sb.Append(Name);
			sb.Append("\n - Elapsed: ");
			sb.Append(ElapsedTime.TotalMilliseconds.ToString("###,##0.0"));
			sb.Append("ms\n - ");
			sb.Append(Result);

			foreach (var property in internalProperties)
			{
				sb.Append("\n");
				sb.Append(property.Key);
				sb.Append(": ");
				sb.Append(property.Value);
			}

			if (Exception != null)
			{
				sb.Append("\n");
				sb.Append("Exception: ");
				Exception.Format(sb);
			}

			return sb.ToString();
		}
	}
}
