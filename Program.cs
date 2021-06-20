#nullable enable
using System;
using System.Diagnostics;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

namespace OxfordV2
{
    class Program
    {

        static void Main(string[] args)
        {
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

            // @Todo implement Lemma sub-command
            var lemmaCommand = new Command("Lemma");
            var senseCommand = new Command("Sense");

            var rootCommand = new RootCommand
        {
            new Argument<string>(name: "word", getDefaultValue: () => "mail", description: "The word to look up."),
			new Option<bool>(new[] {"--obsolete-only", "-o"} , description: "Only return obsolete usages."),
			new Option<bool>(new[] {"--obsolete-exclude", "-oe"} , description: "Only return NON-obsolete usages."),
            new Option<string?>(new[] {"--part-of-speech", "-ps"}, description: "Only return results to words specific parts of speech"),
            new Option<string?>(new[] {"--years", "-y"}, description: "Years.  Use format 900-1999 or -1999 or 900-.  Used for first recorded, last recorded, and current in."),
			new Option<bool>(new[] {"--current-in", "-c"} , description: "Flag which sets the 'Years' option to work with current in year - as opposed to recorded in year - Restrict results to words current in this year or period. Works with the Years flag.  i.e. -y 280-1900 -c  another example: -y 500 -c will return the words current in the year 500 AD."),
            new Option<bool>(new[] {"--revised", "-r"}, description: "Restrict words taken from new and revised OED entries (OED-3rd edition content)"),
            new Option<bool>(new[] {"--revised-not", "-rn"}, description: "Restrict to non revised sources only. (OED 2nd and 1rst edition)"),
            new Option<string?>(new[] {"--etymology-language", "-el"}, description: "Restrict results to words derived from a certain language.  Languages are grouped by continent and hierarchical.  i.e. European will automatically include German."),
            new Option<string?>(new[] {"--etymology-type", "-et"}, description: "Restrict results only certain etymological types.  compound, derivative, conversion, blend, shortening, backformation, initialism, acronym, variant, arbitrary, imitative, borrowing, properName, unknown")
        };

            rootCommand.Description = "An app which processes the Oxford English Dictionary Researcher API, and exports to SuperMemo.";
            rootCommand.Handler = CommandHandler.Create<string, bool, bool, string?, string?, bool, bool, bool, string?, string?>(HandleArgs);


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
            rootCommand.Invoke(args);

        }

        public static void HandleArgs(string word, bool obsoleteOnly, bool obsoleteExclude, string? partOfSpeech, string? years, bool currentIn, bool revised, bool revisedNot, string? etymologyLanguage, string? etymologyType)
        {
                Console.WriteLine($"CLI word entered was {word}");
                Console.WriteLine($"obsoleteOnlyOption: {obsoleteOnly}");
                Console.WriteLine($"excludeObsoleteOption: {obsoleteExclude}.");
                Console.WriteLine($"partOfSpeech: {partOfSpeech ?? "null"}");
                Console.WriteLine($"years: {years ?? "null"}");
                Console.WriteLine($"Current In: {currentIn}");
                Console.WriteLine($"Revised:? {revised}");
                Console.WriteLine($"Revised: Old editions only? {revisedNot}");
                Console.WriteLine($"etymologyLanguage: {etymologyLanguage}");
                Console.WriteLine($"etymologyType: {etymologyType}");

				Console.ReadLine();
                CurrentQuery query = new();
                if (obsoleteOnly)
                {
                    query.IncludeObsolete = true;
                }
                if (obsoleteExclude)
                {
                    query.IncludeObsolete = false;
                }
                if (string.IsNullOrWhiteSpace(word))
                {
                    Console.WriteLine("No CLI word entered.");
                    ConsoleUI.Start(query);
                }
                else
                {
                    Console.WriteLine($"Entered CLI word is {word}");
                    var includeObsoleteProp = query.IncludeObsolete is null ? "null" : query.IncludeObsolete.Value.ToString();
                    Console.WriteLine($"query.IncludeObsolete: {includeObsoleteProp}"); 
                    ConsoleUI.Start(word, query);
                }
        }
    }
}
