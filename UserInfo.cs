using System;
using System.Diagnostics;
using System.IO;

namespace oed
{
	public static class UserInfo
	{
		public static string appId { get; set; }
		public static string appKey { get; set; }

		public static void GetKeys(string userID, string key)
        {
			try
            {
				Trace.WriteLine("GUI passed user entered userID and user entered key.");
				appId = userID;
				appKey = key;
            }
			catch
            {
				Trace.WriteLine("Error setting user id and key from GUI.");
            }
        }
		public static void GetKeys()
		{
			xConsole.WriteLine("Loading Keys...");
            var keysFilePath = Path.Combine(Environment.CurrentDirectory, "keys.txt");
			xConsole.WriteLine("From...");
			xConsole.WriteLine(keysFilePath);
//			StreamReader reader = new StreamReader(".\\keys.txt");
			StreamReader reader = new StreamReader(keysFilePath);
			try
			{
				{
					Trace.WriteLine("Getting request keys");
					appId = reader.ReadLine();
					appKey = reader.ReadLine();
				}
			}
			catch
			{
				xConsole.WriteLine("keys.txt file is empty");
			}
			finally
			{
				reader.Close();
			}
		}
	}
}
