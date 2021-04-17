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
		    }
		    Console.WriteLine();
		    Console.WriteLine("-------------------------");
		    Console.WriteLine("Options:");
		    Console.WriteLine("Definition            - D");
		    Console.WriteLine("Lammatize             - L"); 
		    Console.WriteLine("Root                  - R");
		    Console.WriteLine("Get Senses            - S");
		    Console.WriteLine("Get Quotations        - Q");
		    Console.WriteLine("-------------------------");
		    Console.WriteLine("New Word (start over) - N");
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
			    // Call WORD API
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
			    
			    query.HasLookedUpWord = false;
			    Trace.WriteLine("query.HasLookedUpWord");
			    Trace.WriteLine(query.HasLookedUpWord);
			    query.Definition = "";
			    Trace.WriteLine("query.Definition");
			    Trace.WriteLine(query.Definition);
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
