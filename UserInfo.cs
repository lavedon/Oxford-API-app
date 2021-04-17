using System;
using System.Diagnostics;
using System.IO;

namespace OxfordV2
{
	public static class UserInfo
	{
		public static string appId { get; set; }
		public static string appKey { get; set; }

		public static void GetKeys()
		{

			StreamReader reader = new StreamReader(".\\keys.txt");
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
				Console.WriteLine("keys.txt file is empty");
			}
			finally
			{
				reader.Close();
			}
		}
	}
}
