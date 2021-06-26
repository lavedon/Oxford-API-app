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
            /*
			Console.WriteLine($"args are: {args.ToString()}");
			foreach (var a in args){
				Console.WriteLine(a);
			}
			Console.WriteLine($"args.Length: {args.Length}");
			Console.ReadLine();
            */

/*
            var excludeObsoleteOption = new Option(new[] {
            "--obsolete-exclude","-eo"},
                description: "Exclude obsolete usages."
            );
			*/

            // @Todo implement Lemma sub-command
            var senseCommand = new Command("Sense");
            var senseLemmaArgument = new Argument<string?>(name: "lemma", description: "the word to find senses for. If not specified use the word id from the last query.");
            var senseSynonymArgument = new Argument<string?>(name: "Find synonyms of a sense", description: "Find synonyms for the specified sense.  Note. Requires a sense ID");
            var senseSiblingsArgument = new Argument<string?>(name: "Get senses that are the 'siblings' or the specified sense. Note: Requires a sense id.");

            var senseRegionOption = new Option<string?>(
                new[] {"--restrict-region", "-rr"}, description: "Restrict results to a particular region or dialect. i.e. 'Ireland', 'Northern England'"
            );
            // @TODO make this option an enum of possible values
            var senseUsageOption = new Option<string?>(
                new[] {"--restrict-usage", "-ru"}, description: "Restrict returned sense to a partiular register.  Available values:" + 
                "allusive, archaic, colloquial and slang, derogatory, disused, euphemistic, historical, humorous, ironic, irregular, poetic and literary, rare, regional"
            );
            var senseMainOption = new Option<bool>(
                new[] {"--restrict-main", "-rm"}, description: "Return only the main sense only. Note: The OED does not currently list a main sense for every word."
            );
            senseCommand.AddArgument(senseLemmaArgument);
            senseCommand.AddArgument(senseSynonymArgument);
            senseCommand.AddArgument(senseSiblingsArgument);

            senseCommand.AddOption(senseRegionOption);
            senseCommand.AddOption(senseUsageOption);
            senseCommand.AddOption(senseMainOption);
            




            var quoteCommand = new Command("Quote");
            // @TODO make this an enum with male and female
            var quoteAuthorGender = new Option<string?>(
                new[] {"--author-gender", "-ag"}, description: "Specificy the author's gender. Accepts male and female. For example, -ag male, will return only quotes by men."
            );
            var quoteSourceTitle = new Option<string?>(
                new[] {"--source-title", "-st"}, description: "Find quotations from a particular source, such as a book or periodical.  Example, -st Bleak House"
            );
            var quoteFirstInWord = new Option<bool>(
                new[] {"--first-word", "-fw"}, description: "Restrict results to quotations which are the earliest evidence for a word."
            );
            var quoteFirstInSense = new Option<bool>(
                new[] {"--first-sense", "-fs"}, description: "Restrict results to quotations which are the earliest evidence for a sense."
            );
            quoteCommand.AddOption(quoteAuthorGender);
            quoteCommand.AddOption(quoteSourceTitle);
            quoteCommand.AddOption(quoteFirstInWord);
            quoteCommand.AddOption(quoteFirstInSense);


            var surfaceCommand = new Command("Surfaces");

            var semanticClassCommand = new Command("Semantic");
            var semanticIncludeRegion = new Option<bool>(
                new[] {"--include-region", "-ir"}, description: "If 'false', irregular and regionally-specific variant form are filtered out. Defaults to 'true' if omitted."
            );
            var semanticIncludeInflections = new Option<bool>(
                new[] {"--include-inflections", "-ii"}, description: "If 'false' , inflected forms (of the lemma and of variant spellings) are filtered out, so the results will only include the lemma and its variant spellings. Defaults to 'true' if omitted."
            );
            surfaceCommand.AddOption(semanticIncludeRegion);
            surfaceCommand.AddOption(semanticIncludeInflections);

            var lemmaCommand = new Command("Lemma");
            var lemmaTextArgument = new Argument<string>(name: "text", description: "The text to lemmatize");
            var lemmaTextPretokenized = new Option<bool>(
                new[] {"--tokenize-off", "-to"}, description: "Do not split the entered string into further tokens."
            );
            var lemmaTextTokenizeSeparator = new Option<bool>(
                new[] {"--tokenize-character", "-tc"}, description: "Tokenize the string by this specific character.  i.e. split-up-this-string by '-' character."
            );
            lemmaCommand.AddArgument(lemmaTextArgument);
            lemmaCommand.AddOption(lemmaTextPretokenized);
            lemmaCommand.AddOption(lemmaTextTokenizeSeparator);



            senseCommand.Description = "Get senses for a word.  Takes a word id created by the last word search or simply enter a lemma after this command.";
            quoteCommand.Description = "Get quotations from the OED.  Get quotations based on a word or sense id (uses the last searched sense or word for the id).  Alternatively, you can search all quotations based on some parameters.";
            surfaceCommand.Description = "Get surface-form records based on a word or passed word id.  A surface-form record temize each of the specific orthographic forms that a word may take when it occurs in real-world text";
            semanticClassCommand.Description = "Search semantic classes.  Get related semantic classes based on sense id, branch, parent, ancestorys, children, siblings, descendants, etc.";
            lemmaCommand.Description = "Lemmatize test.  Enter a word or a string of words and return lemmas for each word or words.";



            var rootCommand = new RootCommand
        {
            new Argument<string>(name: "word", getDefaultValue: () => "mail", description: "The word to look up."),
			new Option<bool>(new[] {"--obsolete-only", "-o"}, description: "Only return obsolete usages."),
			new Option<bool>(new[] {"--obsolete-exclude", "-oe"}, description: "Only return NON-obsolete usages."),
            new Option<string?>(new[] {"--part-of-speech", "-ps"}, description: "Only return results to words specific parts of speech"),
            new Option<string?>(new[] {"--years", "-y"}, description: "Years.  Use format 900-1999 or -1999 or 900-.  Used for first recorded, last recorded, and current in."),
			new Option<bool>(new[] {"--current-in", "-c"} , description: "Flag which sets the 'Years' option to work with current in year - as opposed to recorded in year - Restrict results to words current in this year or period. Works with the Years flag.  i.e. -y 280-1900 -c  another example: -y 500 -c will return the words current in the year 500 AD."),
            new Option<bool>(new[] {"--revised", "-r"}, description: "Restrict words taken from new and revised OED entries (OED-3rd edition content)"),
            new Option<bool>(new[] {"--revised-not", "-rn"}, description: "Restrict to non revised sources only. (OED 2nd and 1rst edition)"),
            new Option<string?>(new[] {"--etymology-language", "-el"}, description: "Restrict results to words derived from a certain language.  Languages are grouped by continent and hierarchical.  i.e. European will automatically include German."),
            new Option<string?>(new[] {"--etymology-type", "-et"}, description: "Restrict results only certain etymological types.  compound, derivative, conversion, blend, shortening, backformation, initialism, acronym, variant, arbitrary, imitative, borrowing, properName, unknown")
            // @TODO: add a switch which will prevent the interactive console mode from starting up.
        };

            rootCommand.AddCommand(senseCommand);
            rootCommand.AddCommand(quoteCommand);
            rootCommand.AddCommand(surfaceCommand);
            rootCommand.AddCommand(semanticClassCommand);
            rootCommand.AddCommand(lemmaCommand);

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
                Trace.WriteLine($"CLI word entered was {word}");
                Trace.WriteLine($"obsoleteOnlyOption: {obsoleteOnly}");
                Trace.WriteLine($"excludeObsoleteOption: {obsoleteExclude}.");
                Trace.WriteLine($"partOfSpeech: {partOfSpeech ?? "null"}");
                Trace.WriteLine($"years: {years ?? "null"}");
                Trace.WriteLine($"Current In: {currentIn}");
                Trace.WriteLine($"Revised:? {revised}");
                Trace.WriteLine($"Revised: Old editions only? {revisedNot}");
                Trace.WriteLine($"etymologyLanguage: {etymologyLanguage}");
                Trace.WriteLine($"etymologyType: {etymologyType}");

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
