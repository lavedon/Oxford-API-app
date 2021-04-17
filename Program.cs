using System;
using System.Diagnostics;

namespace OxfordV2
{
    class Program
    {
        static void Main(string[] args)
        {
		// Log to console
		// TextWriterTraceListener tr1 = new TextWriterTraceListener(System.Console.Out);
		// Trace.Listeners.Add(tr1);
		string path = string.Concat(Environment.CurrentDirectory, $"\\logs\\Log_OxfordApplication_{DateTime.Now.ToString("yyyyMMdd-HHmm")}.txt");
		Trace.WriteLine("Path is {0}", path);
		TextWriterTraceListener tr2 = new TextWriterTraceListener(System.IO.File.CreateText(path));
		Trace.Listeners.Add(tr2);
		Trace.WriteLine("Leaving Main method.");
		ConsoleUI.Start();
		Trace.Flush();
        }

    }
}
