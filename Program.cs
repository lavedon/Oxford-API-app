using System;
using System.Diagnostics;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

namespace OxfordV2
{
    class Program
    {
        static int Main(string[] args)
        {
		// Show Trace on console
		// TextWriterTraceListener tr1 = new TextWriterTraceListener(System.Console.Out);
		// Trace.Listeners.Add(tr1);

		// Try to create the logs directory
		var wordOption = new Option<string>(
			"--word",
			description: "The word to look up.  Most advanced features first require a WordID which we can only acquire by looking up a word"
		);

		wordOption.AddAlias("-w");
		wordOption.IsRequired = false;

		var obsoleteOption = new Option<bool>(
			"--obsolete-exclude",
			description: "Do NOT include obsolete words. (Default is to include BOTH obsolete and non obsolete words)"
		);
		obsoleteOption.AddAlias("-oe");

		var onlyObsoleteOption = new Option<bool>(
			"--obsolete-only",
			description: "ONLY return obsolete words. (Default is to include BOTH obsolete and non obsolete words)"
		);
		onlyObsoleteOption.AddAlias("-o");


		var rootCommand = new RootCommand
		{
			wordOption,
			obsoleteOption,
			onlyObsoleteOption
		};

		rootCommand.Description = "An app which processes the Oxford English Dictionary Researcher API, and exports to SuperMemo.";
		rootCommand.Handler = CommandHandler.Create<string, string, bool, bool>((word, etomny, obsoleteOption, onlyObsoleteOption) =>
		{
			Trace.WriteLine($"CLI word entered was {word}");
		    CurrentQuery query = new();
			if(string.IsNullOrWhiteSpace(word)) {
				Console.WriteLine("No CLI word entered.");
				ConsoleUI.Start(query);	
			} else {
				Console.WriteLine($"Entered CLI word is {word}");
				ConsoleUI.Start(word, query);
			}

		});


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
			Trace.Flush();
			// ConsoleUI.Start();
			// @TODO Make an overload of ConsoleUI.Start(which passes a word from the command line)
			return rootCommand.Invoke(args);
        }

    }
}
