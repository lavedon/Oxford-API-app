using System;
using System.Diagnostics;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Reflection;
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
            /*
            */
			Console.WriteLine($"args are: {args.ToString()}");
			foreach (var a in args){
				Console.WriteLine(a);
			}
			Console.WriteLine($"args.Length: {args.Length}");
			Console.ReadLine();

/*
            var excludeObsoleteOption = new Option(new[] {
            "--obsolete-exclude","-eo"},
                description: "Exclude obsolete usages."
            );
			*/


            var rootCommand = new RootCommand
        {
            new Argument<string>("Word"),
			new Option<bool>("-o" , description: "Only return obsolete usages.")
        };

            rootCommand.Description = "An app which processes the Oxford English Dictionary Researcher API, and exports to SuperMemo.";
            rootCommand.Handler = CommandHandler.Create<string, bool, IConsole>(
    HandleArgs);


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
            finally { }

            Trace.WriteLine("Leaving Main method.");
            Trace.Flush();
            // ConsoleUI.Start();
            // @TODO Make an overload of ConsoleUI.Start(which passes a word from the command line)
            return rootCommand.Invoke(args);

        }

        public static void HandleArgs(string word, bool obsoleteOnlyOption, IConsole console)
        {
                Console.WriteLine($"CLI word entered was {word}");
                Console.WriteLine($"obsoleteOnlyOption: {obsoleteOnlyOption}");
				Console.ReadLine();
           //     Console.WriteLine($"excludeObsoleteOption: {excludeObsoleteOption}.");
                CurrentQuery query = new();
                if (obsoleteOnlyOption)
                {
                    query.IncludeObsolete = true;
                }
				/*
                if (excludeObsoleteOption)
                {
                    query.IncludeObsolete = false;
                }
				*/
                if (string.IsNullOrWhiteSpace(word))
                {
                    Console.WriteLine("No CLI word entered.");
                    ConsoleUI.Start(query);
                }
                else
                {
                    Console.WriteLine($"Entered CLI word is {word}");
                    Console.WriteLine($"query.IncludeObsolete: {query.IncludeObsolete}");
                    ConsoleUI.Start(word, query);
                }
        }
    }
}
