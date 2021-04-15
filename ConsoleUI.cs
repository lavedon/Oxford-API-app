using System;

namespace OxfordV2
{
	class ConsoleUI
	{
	    public static void Start()
	    {
		    MainMenu();
		    RefinementMenu();
	    }

	    static void MainMenu()
	    {
		    var query = new CurrentQuery();
		    Console.WriteLine("Please Enter a word");
		    string word = Console.ReadLine().ToLower().Trim();
		    Console.WriteLine("Options:");
		    Console.WriteLine("Word lookup - W");
		    Console.WriteLine("Lammatize   - L"); 
		    Console.WriteLine("Root        - R");
		    Console.WriteLine("Please make a selection and press enter");
		    string selection = (Console.ReadLine()).ToLower().Trim();

		    switch (selection)
		    {
			    case ("w" or "word" or "word lookup" or "lookup"):
			    // Search by word NOT by technical WORD ID
			    Console.WriteLine("Word look up selected");
			    query.UserEnteredWord = word;
			    query.QueryMode = Modes.Word;

			    API.GetWord(query);
			    break;
			    case ("l" or "lammatize"):
			    Console.WriteLine("Lammatize selected.");
			    query.QueryMode = Modes.Lammatize;
			    // Call WORD API
			    // Return the lammatization of the word 
			    break;
			    case ("r" or "root"):
			    Console.WriteLine("Root selected.");
			    query.QueryMode = Modes.Root;
			    // Call /word/{id}/root/ of the word
			    break;

			    default: 
			    Console.WriteLine("No selection made.");
			    break;

		    }
	    }

	    static void RefinementMenu()
	    {
		    Console.WriteLine("Options:");
		    Console.WriteLine("Get Senses            - S");
		    Console.WriteLine("Get Quotations        - Q");
		    Console.WriteLine("---------------------");
		    Console.WriteLine("New Word (start over) - N");
		    Console.WriteLine("Export Results        - E");
		    Console.WriteLine("Please make a selection and press enter");
		    Console.ReadLine();
	   }
	}
}
