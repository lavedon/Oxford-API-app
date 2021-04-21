using System;
using System.Diagnostics;
using System.IO;

namespace OxfordV2
{
    class Program
    {
        static void Main(string[] args)
        {
		// Show Trace on console
		// TextWriterTraceListener tr1 = new TextWriterTraceListener(System.Console.Out);
		// Trace.Listeners.Add(tr1);

		// Try to create the logs directory

		string directoryPath = string.Concat(Environment.CurrentDirectory, "\\logs");

		try
		{
			// Determine whether the directory exists
		if (Directory.Exists(directoryPath))
		{
			Trace.WriteLine("The logs path already exists.");

		}
		else 
		{
		// Try to create the directory
		DirectoryInfo di = Directory.CreateDirectory(directoryPath);
		}
		string fullPath = string.Concat(Environment.CurrentDirectory, $"\\logs\\Log_OxfordApplication_{DateTime.Now.ToString("yyyyMMdd-HHmm")}.txt");
		Trace.WriteLine("Path is {0}", fullPath);
		TextWriterTraceListener tr2 = new TextWriterTraceListener(System.IO.File.CreateText(fullPath));

		Trace.Listeners.Add(tr2);
		}

		catch (Exception e)
		{
			Console.WriteLine("The process failed: {0}", e.ToString());
		}
		finally {}

		Trace.WriteLine("Leaving Main method.");
		ConsoleUI.Start();
		Trace.Flush();
        }

    }
}
