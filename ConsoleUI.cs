using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace OxfordV2
{
	public class ConsoleUI
	{
	    private static bool running { get; set; }

		public static void Start(string word, CurrentQuery query)
		{
			Trace.WriteLine("Starting Main Menu with passed in word.");
			running = true;
			// Call the definition method before main menu
			getDefinition(query, word);
			if (query.InteractiveMode) {
				while (running) {
					MainMenu(query);
				}
			}
			

		}
	    public static void Start(CurrentQuery query)
	    {
		    Trace.WriteLine("Starting Main Menu");
		    running = true;
		    while (running) {
			MainMenu(query);
		    }
	    }

		// Process the interactive ConsoleApp stuff and look for -o flag
		// Or open and start dates to process years
		private static string processInput(CurrentQuery query, string userInput)
        {
			userInput = userInput.ToLower().Trim();
			if (userInput.Contains("-o")) {
				query.HasLookedUpWord = false;
				query.IncludeObsolete = false;
			} 
			string[] inputTokens = userInput.Split(" ");
			
			List<string> result = inputTokens.Where(t => t.All(char.IsDigit)).ToList();
			// @TODO improve this so queries like "robot - 1900" work
			// @TODO improve so queries with no space between dates works.	
			if (result.Count() == 1) {
				query.StartYear = int.Parse(result[0]);
				query.OpenStart = true;

			}
			else if (result.Count() == 2) {
				query.StartYear = int.Parse(result[0]);
				query.EndYear = int.Parse(result[1]);
				query.OpenStart = false;
			}

			return inputTokens[0];
        }

		static void showEtymology(CurrentQuery query)
		{
			for (int i = 0; i < query.Definitions.Count; i++)
			{
				int dNum = i + 1;
				Console.WriteLine($"Etymology for Definition #{dNum}: {query.Definitions[i].WordDefinition}");
				Console.WriteLine();
				Console.WriteLine(query.Definitions[i].DefinitionEtymology.EtymologySummary);
				  Console.WriteLine($"Etymology type: {query.Definitions[i].DefinitionEtymology.EtymologyType}.");
				  Console.WriteLine($"Etymology language: ");
				 foreach (var eLang in query.Definitions[i].DefinitionEtymology.EtymonLanguage)
				 {
				   Console.Write(eLang +" ");
				 }
				 Console.WriteLine();

				  if (query.Definitions[i].DefinitionEtymology.Etymons.Count == 0)  
				  {
					 Console.WriteLine("No etymons are listed."); 
				  } else {
					  Console.WriteLine("Etymons: ");
					  foreach (var etymon in query.Definitions[i].DefinitionEtymology.Etymons)
					     Console.Write(etymon + " ");
				  }
				  Console.WriteLine();
				  Console.WriteLine("Source languages:");
				  foreach (var s in query.Definitions[i].DefinitionEtymology.SourceLanguage)
				  {
					  Console.Write(s + " ");
				  }
				  Console.WriteLine();
				  Console.WriteLine();

			}
		}

		public static void showDefinitions(CurrentQuery query)
		{
			
			for (int i = 0; i < query.Definitions.Count; i++)
			{
				int dNum = i + 1;
				Console.WriteLine("Definition #{0}:", dNum);
				Console.WriteLine();
				Console.WriteLine(query.Definitions[i].WordDefinition.ToString());
				Console.WriteLine();

				if (query.OptionsMenuVerboseMode) {
					string outputPartsOfSpeech = "";
					string mainDefinition = "";
					string firstUseSource = "";
					string firstUseYear = "";
					string isObsolete = "";

					if (query.Definitions[i].PartsOfSpeech.Count != 0)
					{
						query.Definitions[i].FormattedPartsOfSpeech.Clear();	
						query.Definitions[i].FormatPartsOfSpeech();
						foreach (var p in query.Definitions[i].FormattedPartsOfSpeech)
						{
							outputPartsOfSpeech += p.ToString() + ". ";
						}
					}
					if (query.Definitions[i].IsWordMainDefinition)
					{
						mainDefinition = "Listed as a main definition.";
					} else
					{
						mainDefinition = "NOT listed as a main definition.";
					}

					if (query.Definitions[i].IsWordObsolete)
					{
						isObsolete = "This usage is now obsolete.";
					} else {
						isObsolete = "";
					}

					firstUseSource = query.Definitions[i].RecordedFirstUseSource;
					firstUseYear = query.Definitions[i].RecordedFirstUseYear.ToString();
					Console.WriteLine(outputPartsOfSpeech +  isObsolete + " " + mainDefinition + 
						" The original source of this word is listed as {0}. This word was first recored in {1}", 
						firstUseSource, firstUseYear);
					Console.WriteLine();
				}
				

			}				
		}

	    public static void MainMenu(CurrentQuery query)
	    {
		    Trace.WriteLine("In MainMenu()");
		    if (! query.HasLookedUpWord)
            {
                Console.WriteLine("Please Enter a word");
                string userInput = Console.ReadLine().ToLower().Trim();
                getDefinition(query, userInput);
            }
            Console.WriteLine();
		    Console.WriteLine("-------------------------");
		    Console.WriteLine("Options:");
		    Console.WriteLine("Definition            - D");
		    Console.WriteLine("Etymology             - E");
		    Console.WriteLine("Lammatize             - L"); 
		    Console.WriteLine("Root                  - R");
		    Console.WriteLine("Senses                - S");
		    Console.WriteLine("Quotations            - Q");
		    Console.WriteLine("-------------------------");
		    Console.WriteLine("New Word (start over) - N");
		    Console.WriteLine("Options               - O");
		    Console.WriteLine("Export Results        - E");
		    Console.WriteLine("Exit                  - X");
		    Console.WriteLine("Please make a selection and press enter");
		    string selection = (Console.ReadLine()).ToLower().Trim();
		    Trace.WriteLine("The user's input was read as:");
		    Trace.WriteLine(selection);

		    switch (selection)
		    {
			    case ("d" or "definition"):
			    // Search by word NOT by technical WORD ID
			    Trace.WriteLine("Word look up selected");
			    query.QueryMode = Modes.Word;
			    // Only call API if definition not already set  
			    if (string.IsNullOrWhiteSpace(query.Definitions[0].WordDefinition)) {
				    API.APICalls(query);
			    }
			    else 
			    {
				    showDefinitions(query); 
			    }
			    break;

				case ("e" or "etymology" or "etymons"):
				Trace.WriteLine("Show etymology selected");
				showEtymology(query);
				break;

			    case ("l" or "lammatize"):
			    Trace.WriteLine("Lammatize selected.");
			    query.QueryMode = Modes.Lammatize;
			    API.APICalls(query);
			    // Return the lammatization of the word 
			    break;

			    case ("r" or "root"):
			    Trace.WriteLine("Root selected.");
			    query.QueryMode = Modes.Root;
			    API.APICalls(query);
			    break;

			    case ("n" or "new" or "new word"):
			    Trace.WriteLine("New word");
			    // Dispose object.  Start over.
			    // For now just reset object?
			    	    // Wrap below in a Dispose method?
				    query.HasLookedUpWord = false;
				    Trace.WriteLine("query.HasLookedUpWord");
				    Trace.WriteLine(query.HasLookedUpWord);
					query.Definitions.Clear();
				    
			    break;

			    case ("o" or "options" or "option"):
			    Trace.WriteLine("Options menu selected.");
			    OptionsMenu.StartOptionsMenu(query);
			    break;

                case ("s" or "senses"):
                    GetSenses(query);
                    break;

                case ("q" or "quotations"):
			    Trace.WriteLine("Get quotaions based on word selected.");
			    query.QueryMode = Modes.Quotations;
			    API.APICalls(query);
			    break;

                case ("e" or "export" or "export results"):
                    exportQuery();
                    break;

                case ("x" or "exit"):
			    Trace.WriteLine("Exit selected.");
			    running = false;
			    break;

			    default: 
			    Console.WriteLine("No selection made.");
			    break;
		    }
	    }

        public static void GetSenses(CurrentQuery query)
        {
            Trace.WriteLine("Get senses selected.");
            query.QueryMode = Modes.Senses;
   		         API.APICalls(query);
        }

        private static void exportQuery()
        {
            Trace.WriteLine("Export quotations.");
			// @TODO add Definitions 
            if (SavedQueries.Quotes.Count != 0 || SavedQueries.Senses.Count != 0)
                SavedQueries.RenderXML();
            else
            {
                Console.WriteLine("You have not yet saved anything for export.");
            }
        }

        private static void getDefinition(CurrentQuery query, string userInput)
        {
			// Remove this?
            // query.UserEnteredWord = processInput(query, userInput);
			query.UserEnteredWord = userInput.Trim().ToLower();
            try
            {
                Trace.WriteLine("Automatically looking up user entered word:");
                Trace.WriteLine(query.UserEnteredWord);
                query.QueryMode = Modes.Word;
                API.APICalls(query);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Exception on automatic word look up");
                Trace.WriteLine(ex);
            }
			SavedQueries.SaveWordId(query);
            showDefinitions(query);
			if (query.ExportAfterSearch) {
				exportQuery();
			}
        }
    }
}
