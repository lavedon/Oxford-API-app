using System;
using System.Diagnostics;

namespace OxfordV2
{
	class ConsoleUI
	{
	    private static bool running { get; set; }
	    public static void Start()
	    {
		    Trace.WriteLine("Starting Main Menu");
		    running = true;
		    CurrentQuery query = new();
		    while (running) {
			MainMenu(query);
		    }
	    }

	    static void MainMenu(CurrentQuery query)
	    {
		    Trace.WriteLine("In MainMenu()");
		    if (! query.HasLookedUpWord) { 
			    Console.WriteLine("Please Enter a word");
			    string word = Console.ReadLine().ToLower().Trim();
				query.UserEnteredWord = word;
				try {
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
		    }
		    Console.WriteLine();
		    Console.WriteLine("-------------------------");
		    Console.WriteLine("Options:");
		    Console.WriteLine("Definition            - D");
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
			    if (string.IsNullOrWhiteSpace(query.Definition)) {
				    API.APICalls(query);
			    }
			    else 
			    {
				    Console.WriteLine(query.Definition);
			    }
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
				    query.Definition = "";
				    Trace.WriteLine("query.Definition");
				    Trace.WriteLine(query.Definition);
			    break;

			    case ("o" or "options" or "option"):
			    Trace.WriteLine("Options menu selected.");
			    OptionsMenu.StartOptionsMenu(query);
			    break;

			    case ("s" or "senses"):
			    Trace.WriteLine("Get senses selected.");
			    query.QueryMode = Modes.Senses;
			    API.APICalls(query);
			    break;

			    case ("q" or "quotations"):
			    Trace.WriteLine("Get quotaions based on word selected.");
			    query.QueryMode = Modes.Quotations;
			    API.APICalls(query);
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

	}
}
