using System;

namespace LostInSpace.WebApp.Server.Controllers
{
	public static class LobbyIdGenerator
	{
		private static readonly Random random;

		static LobbyIdGenerator()
		{
			random = new Random();
		}

		public static string GenerateLobbyKey()
		{
			char[] characters = new char[6];
			for (int i = 0; i < characters.Length; i++)
			{
				characters[i] = IntToChar(random.Next(0, 10));
			}
			return new string(characters);
		}

		public static bool IsValidLobbyKey(string lobbyKey)
		{
			if (string.IsNullOrWhiteSpace(lobbyKey))
			{
				return false;
			}
			return true;
		}

		private static char IntToChar(int value)
		{
			return value switch
			{
				1 => '1',
				2 => '2',
				3 => '3',
				4 => '4',
				5 => '5',
				6 => '6',
				7 => '7',
				8 => '8',
				9 => '9',
				_ => '0',
			};
		}
	}
}
