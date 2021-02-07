namespace PewPew.WebApp.Shared.Model
{
	public struct ChatMessage
	{
		public string User { get; }
		public string Body { get; }

		public ChatMessage(string user, string body)
		{
			User = user;
			Body = body;
		}

		public override string ToString()
		{
			return $"[{User}]: {Body}";
		}
	}
}
