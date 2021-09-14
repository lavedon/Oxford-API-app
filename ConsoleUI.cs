using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace oed
{
	public class ConsoleUI
	{
	    private static bool running { get; set; }

		private static CurrentQuery SelectWhichDefinitionForQuotes(CurrentQuery query)
		{
			return null;

		}
		public static void Start(string word, CurrentQuery query)
		{
			Trace.WriteLine("Starting Main Menu with passed in word.");
			running = true;
			// Call the definition method before main menu
			if (word == "" && query.WordIDsToUse.Count > 0)
			{
				getDefinition(query);
			} else {
			getDefinition(query, word);
			}
			if (query.ExportAfterSearch) {
				string mode = "";
				switch (query.QueryMode)
				{
					case Modes.Word:
						mode = "definitions";
					break;

					case Modes.Lammatize:
						mode = "lemma";
					break;

					default:
						mode = query.QueryMode.ToString();
					break;
				}

				/*
					xConsole.WriteLine("Select which definitions to find quotes for: (enter for all)");
					string selection = Console.ReadLine();
					Program.ParseExport(query, selection);
				*/
			if (!query.QuotesFromWord && query.QueryMode != Modes.QuotesAndSenses) {
				xConsole.WriteLine("Select which returned definitions to export: (enter for all)");
				string export = Console.ReadLine();
				Program.ParseExport(query, export);
				exportQuery(query);
				} else {
			exportQuery(query);
				}
			if (query.InteractiveMode) {
				while (running) {
					MainMenu(query);
				}
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
				xConsole.WriteLine($"Etymology for Definition #{dNum}: {query.Definitions[i].WordDefinition}");
				xConsole.WriteLine();
				xConsole.WriteLine(query.Definitions[i].DefinitionEtymology.EtymologySummary);
				  xConsole.WriteLine($"Etymology type: {query.Definitions[i].DefinitionEtymology.EtymologyType}.");
				  xConsole.WriteLine($"Etymology language: ");
				 foreach (var eLang in query.Definitions[i].DefinitionEtymology.EtymonLanguage)
				 {
				   Console.Write(eLang +" ");
				 }
				 xConsole.WriteLine();

				  if (query.Definitions[i].DefinitionEtymology.Etymons.Count == 0)  
				  {
					 xConsole.WriteLine("No etymons are listed."); 
				  } else {
					  xConsole.WriteLine("Etymons: ");
					  foreach (var etymon in query.Definitions[i].DefinitionEtymology.Etymons)
					     Console.Write(etymon + " ");
				  }
				  xConsole.WriteLine();
				  xConsole.WriteLine("Source languages:");
				  foreach (var s in query.Definitions[i].DefinitionEtymology.SourceLanguage)
				  {
					  Console.Write(s + " ");
				  }
				  xConsole.WriteLine();
				  xConsole.WriteLine();

			}
		}

		public static void showDefinitions(CurrentQuery query)
		{
			
			for (int i = 0; i < query.Definitions.Count; i++)
			{
				int dNum = i + 1;
				xConsole.WriteLine("Definition #{0}:", dNum);
				xConsole.WriteLine();
				xConsole.WriteLine(query.Definitions[i].WordDefinition.ToString());
				switch (query.IPAMode) 
				{
					case IPAOptions.British:
						xConsole.WriteLine("IPA British: " + query.Definitions[i].BritishIPA);
						break;
					case IPAOptions.USA:
						xConsole.WriteLine("IPA USA: " + query.Definitions[i].USIPA);
						break;
					case IPAOptions.Both:
						xConsole.WriteLine("IPA USA: " + query.Definitions[i].USIPA);
						xConsole.WriteLine("IPA British: " + query.Definitions[i].BritishIPA);
						break;
					default:
						break;
				}
				xConsole.WriteLine();

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

					string output = string.Concat(outputPartsOfSpeech +  isObsolete + " " + mainDefinition + 
						string.Format(" The original source of this word is listed as {0}. This word was first recored in {1}", 
						firstUseSource, firstUseYear));
					xConsole.WriteLine(output);
					xConsole.WriteLine();
					query.Definitions[i].FormattedVerboseOutput = string.Concat(query.UserEnteredWord + ": " + query.Definitions[i].WordDefinition.ToString() + output);
				}
				

			}				
		}

	    public static void MainMenu(CurrentQuery query)
	    {
		    Trace.WriteLine("In MainMenu()");
		    if (! query.HasLookedUpWord)
            {
                xConsole.WriteLine("Please Enter a word");
                string userInput = Console.ReadLine().ToLower().Trim();
                getDefinition(query, userInput);
            }
            xConsole.WriteLine();
		    xConsole.WriteLine("-------------------------");
		    xConsole.WriteLine("Options:");
		    xConsole.WriteLine("Definition            - D");
		    xConsole.WriteLine("Etymology             - E");
		    xConsole.WriteLine("Lammatize             - L"); 
		    xConsole.WriteLine("Root                  - R");
		    xConsole.WriteLine("Senses                - S");
		    xConsole.WriteLine("Quotations            - Q");
		    xConsole.WriteLine("-------------------------");
		    xConsole.WriteLine("New Word (start over) - N");
		    xConsole.WriteLine("Options               - O");
		    xConsole.WriteLine("Export Results        - E");
		    xConsole.WriteLine("Exit                  - X");
		    xConsole.WriteLine("Please make a selection and press enter");
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
			    xConsole.WriteLine("No selection made.");
			    break;
		    }
	    }

        public static void GetQuotes(CurrentQuery query)
        {
            Trace.WriteLine("Get senses selected.");
            query.QueryMode = Modes.Quotations;
   		         API.APICalls(query);
			SavedQueries.SaveWordId(query);
			if (query.ExportAfterSearch)
			{
				exportQuery(query);
			}
        }
        public static void GetSenses(CurrentQuery query)
        {
            Trace.WriteLine("Get senses selected.");
            query.QueryMode = Modes.Senses;
   		         API.APICalls(query);
			SavedQueries.SaveSenseId(query);
			if (query.ExportAfterSearch)
			{
				exportQuery(query);
			}
        }

		public static void GetSurfaces(CurrentQuery query)
		{
			Trace.WriteLine("Get surfaces selected.");
            query.QueryMode = Modes.Surfaces;
   		         API.APICalls(query);
			// We do not save the sense ids of the returned surfaces
			// Surfaces use sense-ids, not "surface ids" as they are senses
			// @TODO change?
			if (query.ExportAfterSearch)
			{
				exportQuery(query);
			}
		}

		public static void GetDerivatives(CurrentQuery query, string wordID)
		{
			Trace.WriteLine("Get derivaties selected.");
			query.QueryMode = Modes.Derivatives;
				API.APICalls(query, wordID);
			if (query.ExportAfterSearch)
			{
				exportQuery(query);
			}
		}

        private static void exportQuery()
		{
            Trace.WriteLine("Export quotations.");
			// @TODO add Definitions 
            if (SavedQueries.Quotes.Count != 0 || SavedQueries.Senses.Count != 0)
                SavedQueries.RenderXML();
            else
            {
                xConsole.WriteLine("You have not yet saved anything for export.");
            }
		}
        private static void exportQuery(CurrentQuery query)
        {
			SavedQueries.IPA = query.IPAMode;
			if (query.QueryMode == Modes.Word)
			{
				SavedQueries.DefinitionsForExport.Clear();
				for(int i = 0;i < query.WhatToExport.Count; i++)
				{
					Trace.WriteLine($"Trying to export word definition #{i}");
					SavedQueries.DefinitionsForExport.Add(query.Definitions[query.WhatToExport[i] - 1]);
				}
				if (query.QuotesFromWord || query.QuotesAndSenses) {
					return;
				}
			}
			else if (query.QueryMode == Modes.Senses)
			{
				SavedQueries.SensesForExport.Clear();
				for(int i = 0;i < query.WhatToExport.Count; i++)
				{
					Trace.WriteLine($"Trying to export sense #{i}");
					SavedQueries.SensesForExport.Add(new Sense(query.Senses[query.WhatToExport[i] - 1]));
				}
			}
			else if (query.QueryMode == Modes.Quotations)
			{
				SavedQueries.QuotesForExport.Clear();
				for(int i = 0;i < query.WhatToExport.Count; i++)
				{
					Trace.WriteLine($"Trying to export quote #{i}");
					SavedQueries.QuotesForExport.Add(new Quote(query.Quotes[query.WhatToExport[i] - 1]));
				}
			}
			else if (query.QueryMode == Modes.Surfaces)
			{
				SavedQueries.SurfacesForExport.Clear();
				for(int i = 0;i < query.WhatToExport.Count; i++)
				{
					Trace.WriteLine($"Trying to export surface #{i}");
					SavedQueries.SurfacesForExport.Add((query.Surfaces[query.WhatToExport[i] - 1]));
				}
			}
			
			SavedQueries.RenderXML();
			
        }

		private static void getDefinition(CurrentQuery query)
		{
			Trace.WriteLine("getDefinition called.");
			try {
                query.QueryMode = Modes.Word;
                API.APICalls(query);
			}
			catch (Exception ex)
			{
				Trace.WriteLine("Exception on looking up words by id");
				Trace.WriteLine(ex.Message);
			}
			showDefinitions(query);
			SavedQueries.SaveWordId(query);
		}

        private static void getDefinition(CurrentQuery query, string userInput)
        {
			// Remove this?
            // query.UserEnteredWord = processInput(query, userInput);
			query.UserEnteredWord = userInput.Trim().ToLower();
			SavedQueries.UserEnteredWord = query.UserEnteredWord;
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
			showDefinitions(query);
			if (query.QuotesFromWord) {
					xConsole.WriteLine("Select which definitions to find quotes for: (enter for all)");
					string selection = Console.ReadLine();
					query = Program.ParseExport(query, selection);
					SavedQueries.SaveWordId(query);
			} else {
				SavedQueries.SaveWordId(query);
			}
			// @TODO Make a version of this to call
			// https://oed-researcher-api.oxfordlanguages.com/oed/api/v0.2/word/{word_id}?include_senses=true&include_quotations=true
			// Similar to calling the Sense ID API
			if (query.QuotesFromWord) {
				string[] newArgs = Program.GetNewArgs();
				Program.UserArgs = newArgs;
				Program.RunCLIAgain = true;
				SavedQueries.BlendedExport = true;
				/*
				// Never need to call this as XML is only rendered after quotes are returned
				if (query.ExportAfterSearch)
				{
					SavedQueries.RenderXML();
				}
				*/
			// TODO call this from a place where we get the word_ids.
			} else if (query.QuotesAndSenses) {
				query.QueryMode = Modes.QuotesAndSenses;
				API.APICalls(query);
			}
        }

		public static void DisplayLemmas(CurrentQuery query)
		{
			xConsole.WriteLine("{0,-20} {1,5}\n", "Token", "Lemma");
			foreach(KeyValuePair<string, string> tokenLemma in query.Lemmas.ZippedLemmas)
			{
				xConsole.WriteLine("{0,-20} {1,5:N1}", tokenLemma.Key, tokenLemma.Value);
			}
		}
    }
}
