using System;
using System.Diagnostics;
using System.Linq;

namespace oed
{
	public static class OptionsMenu
	{
	    private static string yesNo = "";
	    private static bool running = false;

	    public static void StartOptionsMenu(CurrentQuery query)
	    {
		    running = true;
		    while (running) {
			    ShowOptionsMenu(query);
		    }
	    }

	    public static void ShowOptionsMenu(CurrentQuery query)
	    {

		xConsole.WriteLine("-------------------------");
		xConsole.WriteLine("Options Menu:");
		xConsole.WriteLine("-------------------------");
		if (SavedQueries.DeleteOnExport)
		{
			yesNo = "Yes";
		}
		else {
			yesNo = "No";
		}
		xConsole.WriteLine($"Delete old XML file and saved quotations on export?: {yesNo}");
		
		if (query.IncludeObsolete == true)
		{
			yesNo = "Yes";
		}
		else {
			yesNo = "No";
		}
		xConsole.WriteLine($"Include obsolete usage?: {yesNo}");
		if (query.DateRangeSet)
		{
			yesNo = "Yes";
		}
		else {
			yesNo = "No";
		}

		xConsole.WriteLine($"Is a date range set?: {yesNo}");
		if (query.OptionsMenuVerboseMode)
		{
			yesNo = "On";
		}
		else {
			yesNo = "Off";
		}
		xConsole.WriteLine($"Verbose mode: {yesNo}");


		if (query.DateRangeSet)
		{
           string printStart = "";
		   string printEnd = "";
		   if (!query.OpenStart)
		   {
			   printStart = query.StartYear.ToString();
		   }
		   if (!query.OpenEnd)
			{
				printEnd = query.EndYear.ToString();

			}

		   xConsole.WriteLine($"Year Range: {printStart} - {printEnd}");
		}
	
		xConsole.WriteLine("----------------------------");
		xConsole.WriteLine("Toggle obsolete usage    - O");
		xConsole.WriteLine("Toggle verbose mode      - V");
		xConsole.WriteLine("Delete old XML on Export - E");
		xConsole.WriteLine("Set date range:          - D");
		xConsole.WriteLine("Exit to main menu        - X");
		var input = Console.ReadLine().Trim().ToLower();
		switch (input)
		{
			case ("o" or "obsolete" or "ob"):
				xConsole.WriteLine("Toggle Obsolete");
				query.HasLookedUpWord = false;
				query.Definitions.Clear();
				query.IncludeObsolete = !query.IncludeObsolete;
				ConsoleUI.MainMenu(query);
				break;

			case ("v" or "verbose"):
				xConsole.WriteLine("Toggle Verbose Mode");
				query.OptionsMenuVerboseMode = !query.OptionsMenuVerboseMode;
				break;

			case ("e" or "export" or "delete"):
				SavedQueries.DeleteOnExport = !SavedQueries.DeleteOnExport;
				break;

			case ("d" or "date" or "date range"):
				xConsole.WriteLine("Enter Date Range");
				(query.StartYear, query.EndYear) = SetYears(query);
				break;
			case ("x" or "e" or "exit"):
				xConsole.WriteLine("Exiting back to main menu.");
				running = false;
				break;

			default:
				xConsole.WriteLine("Did not recognize input");
			break;
		}

	static (int startYear, int endYear) SetYears(CurrentQuery query)
	{
		int startYearInt = 0;
		int endYearInt = 0;

		var setYearsRunning = true;
		while (setYearsRunning)
		{
			if (query.DateRangeSet) {
				xConsole.WriteLine("Turn off date range? Y/N");
				string result = Console.ReadLine().ToLower().Trim();
					if (result == "y" || result == "yes")
					{
						query.DateRangeSet = !query.DateRangeSet;
						setYearsRunning = !setYearsRunning;
						break;
					}
				}
			xConsole.WriteLine("Only get results within this date range:");
			xConsole.WriteLine("Starting Year: ");
			xConsole.WriteLine("(or Press Enter for open ended start)");
		 	string startYear = Console.ReadLine().Trim().ToLower();

			if (string.IsNullOrWhiteSpace(startYear)) {
				query.OpenStart = true;
				startYearInt = 0;
				xConsole.WriteLine("Set to open ended start year.");
			} 
			else if (validate(startYear)) {
				query.OpenStart = false;
				startYearInt = Int16.Parse(startYear);
			} else {
			    break;
			}
			xConsole.WriteLine("Ending Year:");
			xConsole.WriteLine("(or Press Enter for open ended end)");
			string endYear = Console.ReadLine().Trim().ToLower();
			if (string.IsNullOrWhiteSpace(endYear)) {
				if (query.OpenStart) {
					xConsole.WriteLine("You can not have both an open start and an open end date.");
					query.DateRangeSet = false;
					xConsole.WriteLine("Please start over.");
					Console.ReadLine();
					break;
				}
				query.OpenEnd = true;
				endYearInt = 0;
				xConsole.WriteLine("Set to open ended end year.");
				query.DateRangeSet = true;
				setYearsRunning = false;
				break;
			} 
			else if (validate(endYear)) {
				query.OpenEnd = false;
				endYearInt = Int16.Parse(endYear);
				query.DateRangeSet = true;
				setYearsRunning = false;
				break;
			} else {
			    break;
			}

			static bool validate(string year) {
			if (year.Length <= 4 || year.All(char.IsDigit)) {
				return true;
			} else {
				xConsole.WriteLine("Invalid Year.");
				xConsole.WriteLine("Please enter a 3 digit or 4 digit year.");
				return false;
				}
			}
		}
		return (startYearInt, endYearInt);
		}
	}
	}
}
