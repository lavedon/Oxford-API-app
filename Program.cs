#nullable enable
#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace oed
{
    class Program
    {

        public static bool qsExists = false;
        public static string UserEnteredWord = "";
        private static string[]? _userArgs;
        public static string[] UserArgs { 
            get => _userArgs!;
            set => _userArgs = value ??= new string[1] { "" }; 
        }
        public static bool RunCLIAgain { get; set; }
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            UserArgs = args!;
            RunCLIAgain = false;
			Trace.WriteLine($"args are: {args.ToString()}");
			Trace.WriteLine($"args.Length: {args.Length}");


            var senseCommand = new Command("Sense");
            senseCommand.AddAlias("sense");
            var senseLemmaArgument = new Option<string>(
                new[] {"--lemma", "l"}, description: "the word to find senses for. If not specified use the word id from the last query."
            );
            /*
            var senseSynonymArgument = new Option<string?>(name: "synonyms", description: "Find synonyms for the specified sense.  Note. Requires a sense ID");
            var senseSiblingsArgument = new Option<string?>(name: "siblings", description: "Get senses that are the 'siblings' or the specified sense. Note: Requires a sense id.");
            */

            var senseRegionOption = new Option<string>(
                new[] {"--restrict-region", "rr"}, description: "Restrict results to a particular region or dialect. i.e. 'Ireland', 'Northern England'"
            ){ IsRequired = false };
            // @TODO make this option an enum of possible values
            var senseUsageOption = new Option<string>(
                new[] {"--restrict-usage", "ru"}, description: "Restrict returned sense to a partiular register.  Available values:" + 
                "allusive, archaic, colloquial and slang, derogatory, disused, euphemistic, historical, humorous, ironic, irregular, poetic and literary, rare, regional"
            ){ IsRequired = false };
            var senseMainOption = new Option<bool>(
                new[] {"--restrict-main", "rm"}, description: "Return only the main sense only. Note: The OED does not currently list a main sense for every word."
            ){ IsRequired = false };
            var senseTopicOption = new Option<string>(
                new[] {"--topic", "t"}, 
                description: "Restrict results to senses relating to a particular topic or domain, e.g. 'Heraldry', 'Physics', 'Basketball'. (For the full set of available values, see the 'Subject' section at http://www.oed.com/browsecategory (paywalled)."
            ){
                IsRequired = false
            };
            var senseFromDefinition = new Option<string>(
                new[] {"--from-definition", "d"},
                description: "Look up senses for specific returned words.  i.e. d 4-9,15."
            ){ 
                IsRequired = false 
            };
            var senseFromQuote = new Option<string>(
                new[] {"--from-quote", "q"},
                description: "Look up senses from specific returned quotes.  i.e. q 4-9,15."
            ){
                IsRequired = false
            };

            senseCommand.AddOption(senseLemmaArgument);
            // senseCommand.AddArgument(senseSynonymArgument);
            // senseCommand.AddArgument(senseSiblingsArgument);
            senseCommand.AddOption(senseRegionOption);
            senseCommand.AddOption(senseUsageOption);
            senseCommand.AddOption(senseMainOption);
            senseCommand.AddOption(senseTopicOption);
            senseCommand.AddOption(senseFromDefinition);
            senseCommand.AddOption(senseFromQuote);

            // Handle sense command - Too many arguments? Will 
            senseCommand.Handler = CommandHandler.Create((SenseCommand sense) => {});

            var quoteCommand = new Command("Quote");
            quoteCommand.AddAlias("quote");
            /*
            // @TODO make this an enum with male and female
            var quoteAuthorGender = new Option<string>(
                new[] {"--author-gender", "-ag"}, description: "Specificy the author's gender. Accepts male and female. For example, -ag male, will return only quotes by men."
            ){ IsRequired = false };
            */
            var quoteByWord = new Option<bool>(
                new[] {"--word", "w"}, 
                description: "Find quotes for a word. Enter a word, all word Ids are gathered behind the scenes, then quotations for those word IDs are returned.  Identical to 'your word' w  option in the root command."
            ){ IsRequired = false };
            var quoteMale = new Option<bool>(
                new[] {"--male", "m"}, description: "Only retrieve quotes from male authors."
            ){ IsRequired = false };
            var quoteFemale = new Option<bool>(
                new[] {"--female", "f"}, description: "Only retrieve quotes from female authors."
            ){ IsRequired = false };
            var quoteSourceTitle = new Option<string>(
                new[] {"--source-title", "st"}, description: "Find quotations from a particular source, such as a book or periodical.  Example, -st Bleak House"
            ){ IsRequired = false };
            var quoteAuthor = new Option<string>(
                new[] {"--author", "a"}, description: "Find quotations from a particular author.  Authors listed by first initial and last name (usually)."
            ){ IsRequired = false };
            var quoteFirstInWord = new Option<bool>(
                new[] {"--first-word", "fw"}, description: "Restrict results to quotations which are the earliest evidence for a word."
            ){ IsRequired = false };
            var quoteFirstInSense = new Option<bool>(
                new[] {"--first-sense", "fs"}, description: "Restrict results to quotations which are the earliest evidence for a sense."
            ){ IsRequired = false };
            var quoteUseWords = new Option<bool>(
                new[] {"--use-words", "uw"}, description: "Use the saved list of looked up words.  Required first using the main root command to look up a word.  Finds quotes for all words with wordIDs saved in word-id.txt.  This is also how Sense works if you do not supply it a word.  This option is not default with Quote (unlike Sense) to allow you to look up quotes by source."
            ){ IsRequired = false };
            var quoteFromDefinition = new Option<string>(
                new[] {"--from-definition", "d" }, description: "Get quotes for one of the returned definitions in the last search.  i.e. d1 to get quotes for the first definition d2-4 to gather quotes for definitions 2 through 4."
            ){ IsRequired = false };
            var quoteUseSenses = new Option<bool>(
                new[] {"--use-senses", "us"}, description: "Use the saved list of looked up senses. Requires first using the Sense sub-command/verb.  Similar to --use-words."
            ){ IsRequired = false };
            var quoteFromSense = new Option<string>(
                new[] {"--from-sense", "s"}, description: "Get quotes for one of the returned senses in the last search.  i.e. fs1 to get quotes for the first sense fs2-4 to gather quotes for senses 2 through 4."
            ){ IsRequired = false };


            // quoteCommand.AddOption(quoteAuthorGender);
            quoteCommand.AddOption(quoteByWord);
            quoteCommand.AddOption(quoteMale);
            quoteCommand.AddOption(quoteFemale);
            quoteCommand.AddOption(quoteSourceTitle);
            quoteCommand.AddOption(quoteAuthor);
            quoteCommand.AddOption(quoteFirstInWord);
            quoteCommand.AddOption(quoteFirstInSense);
            quoteCommand.AddOption(quoteUseWords);
            quoteCommand.AddOption(quoteFromDefinition);
            quoteCommand.AddOption(quoteUseSenses);
            quoteCommand.AddOption(quoteFromSense);

            quoteCommand.Handler = CommandHandler.Create<bool, bool, bool, string, string, bool, bool, string, bool, bool, string, string?, bool, bool, bool>(HandleQuoteArgs);

            var surfaceCommand = new Command("Form");
            surfaceCommand.AddAlias("form");

            var semanticClassCommand = new Command("Semantic");
            semanticClassCommand.AddAlias("semantic");

            var semanticIncludeRegion = new Option<bool>(
                new[] {"--include-region", "r"}, description: "If 'false', irregular and regionally-specific variant form are filtered out. Defaults to 'true' if omitted."
            );
            var semanticIncludeInflections = new Option<bool>(
                new[] {"--include-inflections", "i"}, description: "If 'false' , inflected forms (of the lemma and of variant spellings) are filtered out, so the results will only include the lemma and its variant spellings. Defaults to 'true' if omitted."
            );
            var formArgument = new Argument<string>(name: "form", getDefaultValue: () => "peas", description: "The surface form itself, e.g. 'muskettes', 'peas' (case-, space-, and diacritic-insensitive).");
            surfaceCommand.AddArgument(formArgument);
            surfaceCommand.AddOption(semanticIncludeRegion);
            surfaceCommand.AddOption(semanticIncludeInflections);

            surfaceCommand.Handler = CommandHandler.Create<string, string?, string?, bool, bool, bool, bool, bool, string?, string?>(HandleSurfaceArgs);

            var lemmaCommand = new Command("Lemma");
            lemmaCommand.AddAlias("lemma");
            var lemmaTextArgument = new Argument<string>(name: "text", description: "The text to lemmatize");
            var lemmaTextPretokenized = new Option<bool>(
                new[] {"--tokenize-off", "to"}, description: "Do not split the entered string into further tokens."
            );
            var lemmaTextTokenizeSeparator = new Option<bool>(
                new[] {"--tokenize-character", "tc"}, description: "Tokenize the string by this specific character.  i.e. split-up-this-string by '-' character."
            );
            lemmaCommand.AddArgument(lemmaTextArgument);
            lemmaCommand.AddOption(lemmaTextPretokenized);
            lemmaCommand.AddOption(lemmaTextTokenizeSeparator);

            lemmaCommand.Handler = CommandHandler.Create<string, bool, bool, bool, bool>(HandleLemmaArgs);



            senseCommand.Description = "Get senses for a word.  Takes a word id created by the last word search or simply enter a lemma after this command.";
            quoteCommand.Description = "Get quotations from the OED.  Get quotations based on a word or sense id (uses the last searched sense or word for the id).  Alternatively, you can search all quotations based on some parameters.";
            surfaceCommand.Description = "Get surfaceform records based on a word or passed word id.  A surfaceform record itemizes each of the specific orthographic forms that a word may take when it occurs in real-world text";
            semanticClassCommand.Description = "Search semantic classes.  Get related semantic classes based on sense id, branch, parent, ancestorys, children, siblings, descendants, etc.";
            lemmaCommand.Description = "Lemmatize text.  Enter a word or a string of words and return lemmas for each word or words.";



            var rootCommand = new RootCommand
        {
            // Non-Global Options
            new Argument<string>(name: "word", getDefaultValue: () => "mail", description: "The word to look up."),
            new Option<string?>(new[] {"--etymology-language", "el"}, description: "Restrict results to words derived from a certain language.  Languages are grouped by continent and hierarchical.  i.e. European will automatically include German."),
            new Option<string?>(new[] {"--etymology-type", "et"}, description: "Restrict results only certain etymological types.  compound, derivative, conversion, blend, shortening, backformation, initialism, acronym, variant, arbitrary, imitative, borrowing, properName, unknown"),
            new Option<bool>(new[] {"--interactive", "i"}, description: "Open the interactive text menu features, where you can run follow-up queries, change options, and export queries. Has reduced features when compared to the command line."),
            new Option<string>(new[] {"--from-quote", "fq"},
                description: "Get definitions for one of the quotes returned in the last search.  i.e. q 1 to get definitions for the first quote q 2-4 to loop-up definitions for quotes 2 through 4."
        ){ IsRequired = false },
        /*
            new Option<string>(new[] {"--from-sense", "s"}, 
                description: "Get quotes for one of the senses returned in the last search.  i.e. s 1 to get quotes for the first sense s 2-4 to look up definitions for senses 2 through 4.")
            { IsRequired = false },
        */
        };
        // @TODO should this be a command?
        var derivativesCommand = new Command("Derivatives");
        derivativesCommand.AddAlias("d");
        derivativesCommand.AddAlias("derivatives");
        derivativesCommand.TreatUnmatchedTokensAsErrors = false;
        derivativesCommand.Description = "Get derivatives for selected words.  Select which words returned by your last search, you would like to look up derivatives for. Enter the # of the last returned word you want to look up.  Works with range syntax as well (i.e. 1-9,11,15).  Works with the /word/{id}/derivatives endpoint.";
        var derivativesSelection = new Argument<string>(name: "selection", getDefaultValue: () => "", description: "The selection of words to find derivatives for.");
        derivativesCommand.AddArgument(derivativesSelection);
        rootCommand.AddCommand(derivativesCommand);
        derivativesCommand.Handler = CommandHandler.Create<string, bool, bool, string?, string?, bool, bool, bool, string?, string?, bool, bool, bool, string?, string?>(HandleDerivativesArgs);

            // new Option<string?>(new[] {"--part-of-speech", "-ps"}, description: "Only return results to words specific parts of speech"),

            rootCommand.AddGlobalOption(new Option<string?>(new[] {"--part-of-speech", "ps"}, description: "Only return results where the result relates to a specific part of speech (i.e. only nouns, only verbs)"));
			rootCommand.AddGlobalOption(new Option<bool>(new[] {"--obsolete-only", "o"}, description: "Only return obsolete usages."));
			rootCommand.AddGlobalOption(new Option<bool>(new[] {"--obsolete-exclude", "oe"}, description: "Only return NON-obsolete usages."));
            rootCommand.AddGlobalOption(new Option<bool>(new[] {"--clear-export-file", "cf"}, description: "Delete the current export file.  The next time you export - you will start with a brand new file instead of continuing to append to the current file."));
            rootCommand.AddGlobalOption(new Option<string?>(new[] {"--start-year", "sy"}, description: "Specify a start year. You can enter a range for a start year '-' i.e. 500-900"));
            rootCommand.AddGlobalOption(new Option<string?>(new[] {"--end-year", "ey"}, description: "Specify an end year. You can enter a range using a end year '-' i.e. 500-900"));
            rootCommand.AddGlobalOption(new Option<bool>(new[] {"--pronunciation", "p"}, description: "Include IPA pronunciation with returned words. Both American and British.}"));
            rootCommand.AddGlobalOption(new Option<bool>(new[] {"--pronunciation-usa", "pa"}, description: "Include American/USA IPA pronunciation with returned words."));
            rootCommand.AddGlobalOption(new Option<bool>(new[] {"--pronunciation-british", "pb"}, description: "Include British IPA pronunciation with returned words."));

            
            // @TODO make this some kind of string where you can pass a string argument to the command
            // By using the ArgumentAirty.ZeroOrOne 
            var exportOption = new Option<bool>("--export", 
                    description: "Export the results of this query.  Saved as XML for SuperMemo import. File will be saved as OED-export.xml"
                    )
                    {
                        IsRequired = false,
                    };

            exportOption.AddAlias("e");
            rootCommand.AddGlobalOption(exportOption);
            rootCommand.AddGlobalOption(new Option<string?>(new[] {"--years", "y"}, description: "Years.  Use format 900-1999 or -1999 or 900-.  Used for first recorded, last recorded, and current in."));
			rootCommand.AddGlobalOption(new Option<bool>(new[] {"--current-in", "c"} , description: "Flag which sets the 'Years' option to work with current in year - as opposed to recorded in year - Restrict results to words current in this year or period. Works with the Years flag.  i.e. -y 280-1900 -c  another example: -y 500 -c will return the words current in the year 500 AD."));
            rootCommand.AddGlobalOption(new Option<bool>(new[] {"--revised", "r"}, description: "Restrict words taken from new and revised OED entries (OED-3rd edition content)"));
            rootCommand.AddGlobalOption(new Option<bool>(new[] {"--revised-not", "rn"}, description: "Restrict to non revised sources only. (OED 2nd and 1rst edition)"));

            // Other non-global root options for complex queries
            var quotes = new Option<bool>("--quotes",
                    description: "Return quotations for each word definition.  Use with export to make SuperMemo items where quotations are matched with their respective defintion."
            )
            {
                IsRequired = false
            };
            quotes.AddAlias("q");
            rootCommand.AddOption(quotes);

// _ => "xxxx", true,
            var quotesAndSenses = new Option<string?>(
                "--quotes-and-senses",  
                "Return quotations for each sense definition.  Senses, and the quotations for each sense, are automatically matched.  Enter a selection as d1-3,15 or s1,3 etc. to get qs for a specific sense or definition."
            ){
                IsRequired = false,
                ArgumentHelpName = "Selection",
                Arity = ArgumentArity.ZeroOrOne
            };
            

            quotesAndSenses.AddAlias("qs");
            // @TODO fix this:
            rootCommand.AddOption(quotesAndSenses);
            rootCommand.AddCommand(senseCommand);
            rootCommand.AddCommand(quoteCommand);
            rootCommand.AddCommand(surfaceCommand);
            rootCommand.AddCommand(semanticClassCommand);
            rootCommand.AddCommand(lemmaCommand);

            rootCommand.Description = "An app which processes the Oxford English Dictionary Researcher API, and exports to SuperMemo.";
            rootCommand.Handler = CommandHandler.Create((HandleArgs handleArgs) => {});


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
                
                /*
                TextWriterTraceListener tr1 = new TextWriterTraceListener(System.Console.Out);
                Trace.Listeners.Add(tr1);
                */

                TextWriterTraceListener tr2 = new TextWriterTraceListener(System.IO.File.CreateText(fullPath));
                Trace.Listeners.Add(tr2);
            }

            catch (Exception e)
            {
                xConsole.WriteLine($"The process failed: {e.ToString()}");
            }
            finally { }

            Trace.WriteLine("Leaving Main method.");
            Trace.Flush();
            // ConsoleUI.Start();
            // @TODO Make an overload of ConsoleUI.Start(which passes a word from the command line)
            qsExists = rootCommand.Parse(args).CommandResult.FindResultFor(quotesAndSenses) is { };
            rootCommand.Invoke(args);
            if (RunCLIAgain)
            {
                rootCommand.Invoke(UserArgs);
            }

        bool interactiveMode = true;
        while (interactiveMode)
        {
            qsExists = false;
            xConsole.WriteLine("q to exit.");
            Console.Write(">");
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                xConsole.WriteLine("No arguments entered");
                continue;
            }
            else if (input == "q") {
                interactiveMode = false;
                continue;
            }
            else if (input == "clr") {
                Console.Clear();
                continue;
            }
            else if (input == "cf") {
                deleteExportFile();
                continue;
            }
            qsExists = rootCommand.Parse(input).CommandResult.FindResultFor(quotesAndSenses) is { };
            if (qsExists)
            {
                Char[] inchars = input.Trim().ToCharArray();
                if (inchars[0] == 'q')
                input = UserEnteredWord + " " + input;
            }
            rootCommand.Invoke(input);
            if (RunCLIAgain)
            {
                rootCommand.Invoke(UserArgs);
            }
            }
        }

        public static string[] GetNewArgs() {
               // Need args. 
               // @TODO Need args
                string[] quoteArgs = new string[] { "Quote", "uw" };
                string[] newArgs = quoteArgs.Concat(UserArgs.Skip(2)).ToArray();
                return newArgs;
        }

        public static CurrentQuery RunQuotesFromWordId(CurrentQuery query, Command rootCommand, string[] args)
        {
            if (query.QuotesFromWord)
            {
                Trace.WriteLine("Running rootCommand again to get quotes.");
                Trace.WriteLine("Adding Quote uw to args.");
                string[] quoteArgs = new string[] { "Quote", "uw" };
                string[] newArgs = quoteArgs.Concat(args.Skip(2)).ToArray();
                rootCommand.Invoke(newArgs);

            }
            else {
                Trace.WriteLine("Not set to gather quotes from wordID. No point in being in this method.");
            }
                return query;
        }

        public class SenseCommand 
        {
            private string? _lemma;
            private bool _obsoleteOnly;
            private bool _obsoleteExclude;
            private string? _restrictRegion;
            private string? _years;
            private bool _currentIn;
            private bool _revised;
            private bool _revisedNot;
            private string? _restrictUsage;
            private bool _restrictMain;
            private string? _topic;
            private string? _partOfSpeech;
            private string _fromDefinition;
            private string _fromQuote;
            private bool _interactive;
            private bool _export;
            private bool _clearExportFile;

            private string? _startYear;
            private string? _endYear;

            public SenseCommand(string? lemma, bool obsoleteOnly, bool obsoleteExclude, string? restrictRegion, string? years, 
                bool currentIn, bool revised, bool revisedNot, string? restrictUsage, bool restrictMain, string? topic, 
                string? partOfSpeech, string fromDefinition, string fromQuote, bool interactive, bool export, bool clearExportFile,
                string? startYear, string? endYear)
            {
                this._lemma = lemma;
                this._obsoleteOnly = obsoleteOnly;
                this._obsoleteExclude = obsoleteExclude;
                this._restrictRegion = restrictRegion;
                this._years = years;
                this._currentIn = currentIn;
                this._revised = revised;
                this._revisedNot = revisedNot;
                this._restrictUsage = restrictUsage;
                this._restrictMain = restrictMain;
                this._topic = topic;
                this._partOfSpeech = partOfSpeech;
                this._fromDefinition = fromDefinition;
                this._fromQuote = fromQuote;
                this._interactive = interactive;
                this._export = export;
                this._clearExportFile = clearExportFile;
                this._startYear = startYear;
                this._endYear = endYear;

                Trace.WriteLine($"Sense sub command entered.");
                Trace.WriteLine($"lemma: {lemma}");
            //  Trace.WriteLine($"synonyms: {synonyms}");
            //  Trace.WriteLine($"siblings: {siblings}");
                Trace.WriteLine($"restrictRegion: {restrictRegion}");
                Trace.WriteLine($"restrictUsage: {restrictUsage}");
                Trace.WriteLine($"restrictMain: {restrictMain}");
                Trace.WriteLine($"obsoleteOnlyOption: {obsoleteOnly}");
                Trace.WriteLine($"excludeObsoleteOption: {obsoleteExclude}.");
                Trace.WriteLine($"partOfSpeech: {partOfSpeech ?? "null"}");
                Trace.WriteLine($"years: {years ?? "null"}");
                Trace.WriteLine($"topic: {topic}");
                Trace.WriteLine($"Current In: {currentIn}");
                Trace.WriteLine($"FromDefinition: {fromDefinition}");
                Trace.WriteLine($"FromQuote: {fromQuote}");
                Trace.WriteLine($"interactive: {interactive}");
                Trace.WriteLine($"export: {export}");
                Trace.WriteLine($"clearExportFile: {clearExportFile}");
                Trace.WriteLine($"startYear: {startYear}");
                Trace.WriteLine($"endYear: {endYear}");
                processSenseArgs();
            }

            private void processSenseArgs()
            {
                Trace.WriteLine("Processing sense arguments.");
                CurrentQuery query = new();
                if (_clearExportFile)
                {
                    deleteExportFile();
                }

                query.CurrentSenseOptions = new(_lemma, _restrictRegion, _restrictUsage, _restrictMain, _topic, _fromDefinition, _fromQuote);
                processCommonOptions(_obsoleteOnly, _obsoleteExclude, _partOfSpeech, _years, _currentIn, _revised, _revisedNot, 
                    _interactive, _export, query, _startYear, _endYear);
                // Implement the non common options (i.e. the options not in available in the Word endpoint)
                // region, main_current_sense, usage, topic
                if(!string.IsNullOrWhiteSpace(_topic))
                {
                    query.CurrentSenseOptions.Topic = _topic;
                }
            if(!string.IsNullOrWhiteSpace(_restrictRegion))
                {
                query.CurrentSenseOptions.RestrictRegion = _restrictRegion; 
                } 
                if(_restrictMain)
                {
                    query.CurrentSenseOptions.RestrictMain = true;
                }
                if(!string.IsNullOrWhiteSpace(_restrictUsage))
                {
                    query.CurrentSenseOptions.RestrictUsage = _restrictUsage;
                }
                
                if (!string.IsNullOrWhiteSpace(_fromDefinition) || !string.IsNullOrWhiteSpace(_fromQuote))
                {
                    string nums = string.IsNullOrWhiteSpace(_fromDefinition) ? _fromQuote :  _fromDefinition;

                    query.CurrentSenseOptions.WordIDsToUse = GetSelectWordIds(nums);
                    ConsoleUI.GetSenses(query);

                }
                else if (string.IsNullOrWhiteSpace(_lemma))
                {
                    Trace.WriteLine("Looking up word IDs from file");
                    ConsoleUI.GetSenses(SavedQueries.LoadWordIds(query));
                }
                else
                { 
                    query.HasLookedUpWord = true;
                    ConsoleUI.GetSenses(query);
                }

            }
        }

        public static void HandleDerivativesArgs(string selection, bool obsoleteOnly, bool obsoleteExclude, string? partOfSpeech, string? years, bool currentIn, bool revised, bool revisedNot, string? etymologyLanguage, string? etymologyType, bool interactive, bool export, bool clearExportFile, string? startYear, string? endYear)
        {
            Trace.WriteLine("Derivatives sub command entered.");
            Trace.WriteLine($"CLI entered selection was {selection}");
            Trace.WriteLine($"obsoleteOnlyOption: {obsoleteOnly}");
            Trace.WriteLine($"excludeObsoleteOption: {obsoleteExclude}.");
            Trace.WriteLine($"partOfSpeech: {partOfSpeech ?? "null"}");
            Trace.WriteLine($"years: {years ?? "null"}");
            Trace.WriteLine($"Current In: {currentIn}");
            Trace.WriteLine($"Revised:? {revised}");
            Trace.WriteLine($"Revised: Old editions only? {revisedNot}");
            Trace.WriteLine($"etymologyLanguage: {etymologyLanguage}");
            Trace.WriteLine($"etymologyType: {etymologyType}");
            Trace.WriteLine($"interactive: {interactive}");
            Trace.WriteLine($"export: {export}");
            Trace.WriteLine($"StartYear: {startYear}");
            Trace.WriteLine($"EndYear: {endYear}");

            CurrentQuery query = new();
            if (clearExportFile)
                {
                    deleteExportFile();
                }

            string wordIDFile = Path.Combine(Environment.CurrentDirectory, "word-id.txt");
            string[] wordIds = System.IO.File.ReadAllLines(wordIDFile);
            processCommonOptions(obsoleteOnly, obsoleteExclude, partOfSpeech, years, currentIn, revised, revisedNot, interactive, export, query, startYear, endYear);

            if (!string.IsNullOrWhiteSpace(selection))
            {
                // Open the file containing the word IDs
                List<int> parsedSelection = Program.ParseNumbers(selection);
                Trace.WriteLine("You want to get derivatives for these words (from your last search)");

                foreach (int id in parsedSelection)
                {                                                
                    ConsoleUI.GetDerivatives(query, wordIds[id]);
                }
            }
        }

        public static void HandleSurfaceArgs(string form, string? partOfSpeech, string? years, bool includeRegion, 
            bool includeInflections, bool interactive, bool export, bool clearExportFile, 
            string? startYear, string? endYear)
        {
            Trace.WriteLine($"Surfaceforms sub command entered.");
            Trace.WriteLine($"includeRegion {includeRegion}");
            Trace.WriteLine($"includeInflections {includeInflections}");
            Trace.WriteLine($"interactive {interactive}");
            Trace.WriteLine($"export {export}");

            CurrentQuery query = new();
            if (clearExportFile)
                {
                    deleteExportFile();
                }

            // As SurfaceForms only has a "current in" style years mode
            // When first set "current in" to true, then pass the years
            query.QueryMode = Modes.Surfaces;
            query.CurrentSurfaceOptions = new SurfaceOptions(includeRegion, includeInflections, form, partOfSpeech!);
            bool currentIn = true;

            if (string.IsNullOrWhiteSpace(form))
            {
                xConsole.WriteLine("No form word entered.");
                xConsole.WriteLine("Please enter a form to look up with the Surfaceforms endpoint.");
                query.CurrentSurfaceOptions.Form = Console.ReadLine()!;
            } else 
            {
                Trace.WriteLine($"Getting surfaceforms for {form}");
            }
            processCommonOptions(obsoleteOnly: false, obsoleteExclude: false, partOfSpeech, years, currentIn, revised: false, revisedNot: false, interactive, export, query, startYear, endYear);
            ConsoleUI.GetSurfaces(query);
        }
        // Also handling some global options
        public static void HandleLemmaArgs(string text, bool tokenizeOff, bool tokenizeCharacter, bool export, bool clearExportFile)
        {
            Trace.WriteLine($"Lemma sub command entered.");
            Trace.WriteLine($"Text to lemmatize was: {text}");
            Trace.WriteLine($"tokenizeOff: {tokenizeOff}");
            Trace.WriteLine($"tokenizeCharacter: {tokenizeCharacter}");
            CurrentQuery query = new();
            if (clearExportFile)
                {
                    deleteExportFile();
                }

            try {
            query.LemmaText = text.Trim('\'');
            } catch (Exception ex) {
                Trace.WriteLine($"{ex}");
            }
            query.QueryMode = Modes.Lammatize;
   		         API.APICalls(query);

            if (export)
            {
                // export Lemmas
                xConsole.WriteLine("Starting export process");
                SavedQueries.AddMember(query.Lemmas);
            }
        }
        public static void HandleQuoteArgs(bool word, bool male, bool female, string sourceTitle, string author, bool firstWord, bool firstSense, string fromDefinition, bool useWords, bool useSenses, string fromSense, string? years, bool interactive, bool export, bool clearExportFile)
        {
            Trace.WriteLine($"Quote sub command entered.");
            Trace.WriteLine($"male: {male}");
            Trace.WriteLine($"female: {female}");
            Trace.WriteLine($"author: {author}");
         // Trace.WriteLine($"authorGender: {authorGender}");
            Trace.WriteLine($"sourceTitle: {sourceTitle}");
            Trace.WriteLine($"firstWord: {firstWord}");
            Trace.WriteLine($"firstSense: {firstSense}");
            Trace.WriteLine($"From word: {fromDefinition}");
            Trace.WriteLine($"useWords: {useWords}");
            Trace.WriteLine($"useSenses: {useSenses}");
            Trace.WriteLine($"export: {export}");
            Trace.WriteLine($"interactive: {interactive}");
            Trace.WriteLine($"years: {years ?? "null"}");

            CurrentQuery query = new();
            if (clearExportFile)
                {
                    deleteExportFile();
                }

            processCommonOptions(years, interactive, export, query, startYear: null, endYear: null);

            query.CurrentQuoteOptions = new(male, female, sourceTitle, author, firstWord, firstSense, fromDefinition, fromSense, useWords, useSenses);
            /*
            if (!string.IsNullOrWhiteSpace(authorGender))
            {
                query.CurrentQuoteOptions.AuthorGender = authorGender;
            }
            */
            if (male)
            {
                query.CurrentQuoteOptions.Male = true;
            }
            if (female)
            {
                query.CurrentQuoteOptions.Female = true;
            }
            if (!string.IsNullOrWhiteSpace(sourceTitle))
            {
                query.CurrentQuoteOptions.SourceTitle = sourceTitle;
            }
            if (!string.IsNullOrWhiteSpace(author))
            {
                query.CurrentQuoteOptions.Author = author;
            }
            if (firstWord)
            {
                query.CurrentQuoteOptions.FirstWord = true;
            }

            if (firstSense)
            {
                query.CurrentQuoteOptions.FirstSense = true;
            }
            if (!string.IsNullOrWhiteSpace(fromDefinition))
            {
                // @TODO reuse this for Senses and other sub-commands?
                query.CurrentQuoteOptions.WordIDsToUse = GetSelectWordIds(fromDefinition);
                ConsoleUI.GetQuotes(query);
            }
            else if (!string.IsNullOrWhiteSpace(fromSense))
            {
                // @TODO Merge this into GetSelectWordIds method?
                try {
                    List<int> whatSenses = ParseNumbers(fromSense);
                    string senseIDFile = Path.Combine(Environment.CurrentDirectory, "sense-id-all.txt");
                    string[] senseIds = File.ReadAllLines(senseIDFile);
                    foreach (int n in whatSenses)
                    {
                        Trace.WriteLine($"File line to look up: {n}");
                        Trace.WriteLine($"Sense id to look up ${senseIds[n - 1]}");
                        query.CurrentQuoteOptions.SenseIDsToUse.Add(senseIds[n - 1]);

                    }
                }
                catch (Exception ex) {
                    Trace.WriteLine("Could not get ids from sense-id-all.txt");
                    Trace.WriteLine("Could not retrieve the requested sense Ids from the sense id file.");
                    Trace.WriteLine(ex);
                }
                ConsoleUI.GetQuotes(query);

            }
            else if (useWords)
            {
                // Cycle through word-id.txt
                // Run /word/{id}/quotations/ endpoint
                // @TODO - Only LoadWordIDs depending on what was selected 
                // Previously with Definition export?
                ConsoleUI.GetQuotes(SavedQueries.LoadWordIds(query));

            }
            else if (useSenses)
            {
                // Run /sense/{id}/quotations/ endpoint
                ConsoleUI.GetQuotes(SavedQueries.LoadSenseIds(query));

            }
            else {
                query.CurrentQuoteOptions.UseNonIdEndpoint = true;
                xConsole.WriteLine("Returning all quotes based on parameters.  If you want quotes for the past sense or word search...");
                xConsole.WriteLine("...then please re-run the Quote command, and specify if you want quotes based on the previous word search or sense search.");
                xConsole.WriteLine("uw flag or us flag");
                ConsoleUI.GetQuotes(query);
            }

        }

        private static List<string> GetSelectWordIds(string numString)
        {
            // @TODO put in a bool to look up from senses or word-ids?
            List<string> returnIds = new();
            try
            {
                List<int> whatWords = ParseNumbers(numString);
                string wordIDFile = Path.Combine(Environment.CurrentDirectory, "word-id.txt");
                // Where do I save the word-ids? 
                // In query.QuoteOptions? 
                string[] ids = File.ReadAllLines(wordIDFile);
                if (ids.Length == 0)
                {
                    xConsole.WriteLine("No word ids found in word-id.txt");
                    return null;
                } else {
                foreach (int n in whatWords)
                {
                    int y;
                    y = n - 1;
                    Trace.WriteLine($"File line to look up: {y}"); 
                    returnIds.Add(ids[y]);
                    returnIds.Distinct();
                }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Could not get ids from word-id.txt");
                Trace.WriteLine($"{ex}");
            }
            return returnIds;
        }

        public class HandleArgs
        {
            private string? _word;
            private bool _quotes;
            private string? _quotesAndSenses;
            private bool _obsoleteOnly;
            private bool _obsoleteExclude;
            private string? _partOfSpeech; 
            private string? _years;
            private bool _currentIn; 
            private bool _revised; 
            private bool _revisedNot; 
            private string? _etymologyLanguage;
            private string? _etymologyType;
            private string _fromQuote; 
            private bool _interactive; 
            private bool _export; 
            private bool _clearExportFile; 
            private string? _startYear;
            private string? _endYear;

            private bool _pronunciation;
            private bool _pronunciationUSA;
            private bool _pronunciationBritish;
        public HandleArgs(string word, bool quotes, string? quotesAndSenses, bool obsoleteOnly, bool obsoleteExclude, string? partOfSpeech, 
            string? years, bool currentIn, bool revised, bool revisedNot, string? etymologyLanguage, string? etymologyType, string fromQuote, 
            bool interactive, bool export, bool clearExportFile, string? startYear, string? endYear, bool pronunciation, 
            bool pronunciationUSA, bool pronunciationBritish)
        {
            Trace.WriteLine($"CLI word entered was {word}");
            Trace.WriteLine($"Return quotes from word search {quotes}");
            Trace.WriteLine($"Return senses with quotes {quotesAndSenses}");
            Trace.WriteLine($"obsoleteOnlyOption: {obsoleteOnly}");
            Trace.WriteLine($"excludeObsoleteOption: {obsoleteExclude}.");
            Trace.WriteLine($"partOfSpeech: {partOfSpeech ?? "null"}");
            Trace.WriteLine($"years: {years ?? "null"}");
            Trace.WriteLine($"Current In: {currentIn}");
            Trace.WriteLine($"Revised:? {revised}");
            Trace.WriteLine($"Revised: Old editions only? {revisedNot}");
            Trace.WriteLine($"etymologyLanguage: {etymologyLanguage}");
            Trace.WriteLine($"etymologyType: {etymologyType}");
            Trace.WriteLine($"interactive: {interactive}");
            Trace.WriteLine($"export: {export}");
            Trace.WriteLine($"clearExportFile: {clearExportFile}");
            Trace.WriteLine($"startYear: {startYear}");
            Trace.WriteLine($"endYear: {endYear}");
            Trace.WriteLine($"Prounciation {pronunciation}");
            Trace.WriteLine($"Pronunciation {pronunciationUSA}");
            Trace.WriteLine($"Prouniciation {pronunciationBritish}");

            this._word = word;
            this._quotes = quotes;
            this._quotesAndSenses = quotesAndSenses;
            this._obsoleteOnly = obsoleteOnly;
            this._obsoleteExclude = obsoleteExclude;
            this._partOfSpeech = partOfSpeech; 
            this._years = years;
            this._currentIn = currentIn; 
            this._revised = revised; 
            this._revisedNot = revisedNot; 
            this._etymologyLanguage = etymologyLanguage;
            this._etymologyType = etymologyType;
            this._fromQuote = fromQuote; 
            this._interactive = interactive; 
            this._export = export; 
            this._clearExportFile = clearExportFile; 
            this._startYear = startYear;
            this._endYear = endYear;
            this._pronunciation = pronunciation;
            this._pronunciationUSA = pronunciationUSA;
            this._pronunciationBritish = pronunciationBritish;

            CurrentQuery query = new();
            if (_clearExportFile)
                {
                    deleteExportFile();
                }

            processCommonOptions(_obsoleteOnly, _obsoleteExclude, _partOfSpeech, _years, _currentIn, _revised, _revisedNot, _interactive, _export, query, _startYear, _endYear);
            if (_pronunciation) {
                query.IPAMode = IPAOptions.Both;
            }
            if (_pronunciationBritish) {
                query.IPAMode = IPAOptions.British;
            }
            if (_pronunciationUSA) {
                query.IPAMode = IPAOptions.USA;
            }

            if (_quotes)
            {
                query.QuotesFromWord = true;
            }
            // Check for default value "xxxx" which means the user only entered the option
            if (qsExists) {
                query.QuotesFromWord = false;
                query.QuotesAndSenses = true;
                query.QSFromDefinitions = true;
                if (!string.IsNullOrWhiteSpace(_quotesAndSenses))
                {
                    // check if sense selected
                        Trace.WriteLine("Quote and Senses turned on with a sense selection");
                        // remove all non integers

                        string trimedQS = _quotesAndSenses.Replace(" ", "").ToLower().Trim();
                        // @TODO sepearate both a sense and a d selection and find a selection for each
                        if (trimedQS.Contains("d") && trimedQS.Contains("s"))
                        {
                            string senseSelection = Regex.Match(trimedQS, @"(?<=s)(.*?)(?=[a-zA-z])").Value;
                            xConsole.WriteLine("QS for a selection of both definitions and senses not yet implemented.");
                            xConsole.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                        } else if (trimedQS.Contains("d") && !trimedQS.Contains("s"))
                        {
                            xConsole.WriteLine("QS from a specific definition");
                            query.QSFromDefinitions = true;
                            query.QSFromSenses = true;
                        } else if (trimedQS.Contains("s") && !trimedQS.Contains("d"))
                        {
                            query.QSFromSenses = true;
                            query.QSFromDefinitions = false;
                            xConsole.WriteLine("QS from a specific sense.");
                        } else {
                            query.QSFromDefinitions = true;
                        }
                        string cleanSelection = Regex.Replace(_quotesAndSenses, "[A-Za-z]", ""); 
                        List<int> result = ParseNumbers(cleanSelection);
                        if (query.QSFromSenses) {
                            xConsole.WriteLine("You want me to run qs on these senses:");
                            query.QSSenseSelection = result;
                        } else {
                            xConsole.WriteLine("You want me to run qs on these definitions:");
                            query.QSDefSelection = result;
                        }
                        foreach (int n in result)
                        {
                            xConsole.WriteLine($"{n}");
                        }
                } // if _quotesAndSenses has text
            } // if qsExists
            
            if (!string.IsNullOrWhiteSpace(_etymologyLanguage))
            {
                query.EtymologyLanguage = _etymologyLanguage;
            }

            if (!string.IsNullOrWhiteSpace(_etymologyType))
            {
                query.EtymologyType = _etymologyType;
            }

            if (string.IsNullOrWhiteSpace(_word))
            {
                xConsole.WriteLine("No CLI word entered.");
                ConsoleUI.Start(query);
            }
            else
            {
                Trace.WriteLine($"Entered CLI word is {word}");
                var includeObsoleteProp = query.IncludeObsolete is null ? "null" : query.IncludeObsolete.Value.ToString();
                Trace.WriteLine($"query.IncludeObsolete: {includeObsoleteProp}");
                if (!string.IsNullOrWhiteSpace(_fromQuote))
                {
                    Trace.WriteLine("fromQuote complex query activated in RootCommand / Definitions.");
                    query.WordIDsToUse = GetSelectWordIds(_fromQuote);
                    ConsoleUI.Start(word = "", query);
                } else {
                    ConsoleUI.Start(word, query);
                }
                if (query.QuotesFromWord)
                {
                    string[] newArgs = GetNewArgs();
                   // Use reflection to get the rootCommand of Program?!? Then run that?
                  //  RunQuotesFromWordId(query, Program.programState.RootCommand, state.GlobalArgs); 
                }
            }
            }
        }

        private static void processCommonOptions(string? years, bool interactive, bool export, CurrentQuery query, 
            string? startYear, string? endYear)
        {
            processYears(years, query, startYear, endYear);
            if (interactive)
            {
                query.InteractiveMode = true;
            }
            if (export)
            {
                // SavedQueries.ExportFileName = export;
                query.ExportAfterSearch = true;
            }

        }
        private static void processCommonOptions(bool obsoleteOnly, bool obsoleteExclude, string? partOfSpeech, string? years, 
            bool currentIn, bool revised, bool revisedNot, bool interactive, bool export, CurrentQuery query, 
            string? startYear, string? endYear)
        {
            processYears(years, query, startYear, endYear);
            if (currentIn)
            {
                query.CurrentIn = true;
            }
            if (!string.IsNullOrWhiteSpace(partOfSpeech))
            {
                query.PartsOfSpeech = partOfSpeech;
            }
            if (obsoleteOnly)
            {
                query.IncludeObsolete = true;
            }
            if (obsoleteExclude)
            {
                query.IncludeObsolete = false;
            }
            if (revised)
            {
                query.IncludeRevised = true;
            }
            if (revisedNot)
            {
                query.IncludeRevised = false;
            }
            if (interactive)
            {
                query.InteractiveMode = true;
            }
            if (export)
            {
                // SavedQueries.ExportFileName = export;
                query.ExportAfterSearch = true;
            }
        }

        private static void deleteExportFile()
        {
            string filePath = "";
            try {
            filePath = Path.Combine(Environment.CurrentDirectory, SavedQueries.ExportFileName);
            }
            catch (Exception ex) {
                xConsole.WriteLine("Trying to delete XML export file.");
                xConsole.WriteLine("But could not find the file: " + ex.Message);
                xConsole.WriteLine(ex.InnerException?.ToString());
            }
            try {
                xConsole.WriteLine($"Deleting XML export file: {filePath}");
                File.Delete(filePath);
            }
            catch (Exception ex) {
                xConsole.WriteLine($"Error while deleting XML export file: {filePath}");
                xConsole.WriteLine("Is the export file open in another program? in SuperMemo?");
                xConsole.WriteLine(ex.Message);
                xConsole.WriteLine(ex.InnerException?.ToString());
            }
            // Delete the text file also
            try {
                filePath = Path.Combine(Environment.CurrentDirectory, "OED-export.txt");
                xConsole.WriteLine($"Deleting TXT export file: {filePath}");
                File.Delete(filePath);
            } 
            catch (Exception ex) {
                xConsole.WriteLine("Trying to delete text export file.");
                xConsole.WriteLine("But could not find the file: " + ex.Message);
                xConsole.WriteLine(ex.Message);
                xConsole.WriteLine(ex.InnerException?.ToString());
            }
        }

        private static void processYears(string? years, CurrentQuery query, string? startYear, string? endYear)
        {
            // @TODO improve this parser. No end year should be supported and 
            // not throw an exception.
            if (!string.IsNullOrWhiteSpace(startYear))
            {
                
                query.StartYearString = startYear.Trim().ToLower().Replace(" ", "");
                query.StartEndYearRanges = true;

            }
            if (!string.IsNullOrWhiteSpace(endYear))
            {
                query.EndYearString = endYear.Trim().ToLower().Replace(" ", "");
                query.StartEndYearRanges = true;
            }
            
            string cleanYears = years?.Trim().Replace("'", "").Replace("\"", "");
            if (!string.IsNullOrWhiteSpace(cleanYears))
            {
                if (cleanYears.Contains("-")) {
                string[] yearDates = cleanYears.Split('-');
                try
                {
                    if (!string.IsNullOrWhiteSpace(yearDates[0])) {
                        query.StartYear = int.Parse(yearDates[0].Trim());
                        Trace.WriteLine($"query.StartYear: {query.StartYear}");
                        query.DateRangeSet = true;
                    } else {
                        Trace.WriteLine("No start year entered. Open ended start.");
                        query.OpenStart = true;
                        query.DateRangeSet = true;
                    }
                }
                catch(Exception e)
                {
                    Trace.WriteLine($"{e.GetType()}: {e.Message}");
                    Trace.WriteLine("No start year.");
                }
                try
                {
                    if (!string.IsNullOrWhiteSpace(yearDates[1])) {
                        query.EndYear = int.Parse(yearDates[1].Trim());
                        Trace.WriteLine($"query.EndYear: {query.EndYear}");
                        query.OpenEnd = false;
                        query.DateRangeSet = true;
                    } else {
                        Trace.WriteLine("No end year. Open ended first year");
                        query.OpenEnd = true;
                        query.DateRangeSet = true;
                    }
                }
                catch(Exception e)
                {
                    Trace.WriteLine($"{e.GetType()}: {e.Message}");
                    Trace.WriteLine("No end year.");
                }
            } else {
                Trace.WriteLine("No end year. Only one year entered.");
                try {
                    query.StartYear = int.Parse(years);
                    query.OpenEnd = false;
                    query.DateRangeSet = true;
                } 
                catch(Exception e) 
                {
                    Trace.WriteLine("Error reading the only entered year.");
                    Trace.WriteLine($"{e.GetType()}: {e.Message}");
                }
            }

            }
        }

        public static CurrentQuery ParseExport(CurrentQuery query, string export)
        {
        Trace.WriteLine("Parsing export argument...");
        Trace.WriteLine("...");

        if (export.Length == 0)
        {
            query.ExportAll = true;
            xConsole.WriteLine("Will export all results");
            switch (query.QueryMode)
            {
                case Modes.Word:
                    for(int i = 0; i < query.Definitions.Count; i++)
                    {
                        query.WhatToExport.Add(i + 1);
                    }
                break;
                case Modes.Root:
                break;
                case Modes.Lammatize:
                break;
                case Modes.Senses:
                    for(int i = 0; i < query.Senses.Count; i++)
                    {
                        query.WhatToExport.Add(i + 1);
                    }
                break;
                case Modes.Quotations:
                    for(int i = 0; i < query.Quotes.Count; i++)
                    {
                        query.WhatToExport.Add(i + 1);
                    }
                break;
                case Modes.Surfaces:
                break;
                case Modes.Derivatives:
                break;
                

            }
            
        } else {
            query.WhatToExport = ParseNumbers(export);
        }
            return query;
        }
    
        public static List<int> ParseNumbers(string export)
        {
            List<int> nums = new();
            try {
            export = export.Trim().ToLower();
            export = export.Replace(" ", string.Empty);
            Trace.WriteLine("Linq splitting the ',' and '-'");
            IEnumerable<int> result = export.Split(',')
                    .SelectMany(x => x.Contains('-') ? 
                    Enumerable.Range(int.Parse(x.Split('-')[0]), 
                    int.Parse(x.Split('-')[1]) - int.Parse(x.Split('-')[0]) + 1) : new int[] { int.Parse(x) });

            Trace.WriteLine("These are the numbers I parsed from the supplied selection.");
            foreach (var r in result)
            {
                Trace.WriteLine(r);
                nums.Add(r);
            }
            } catch(Exception ex)
            {
                xConsole.WriteLine("Number selection entered in incorrect format.");
                xConsole.WriteLine("Did you use a space to seperate values instead of a comma?");
                Trace.WriteLine($"error parsing export numbers. {ex}");
            }
            return nums;
        }

    }
}