using System;
using System.Diagnostics;
using System.Linq;

namespace OxfordV2
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

		Console.WriteLine("-------------------------");
		Console.WriteLine("Options Menu:");
		Console.WriteLine("-------------------------");
		
		if (query.IncludeObsolete == true)
		{
			yesNo = "Yes";
		}
		else {
			yesNo = "No";
		}
		Console.WriteLine($"Include obsolete usage?: {yesNo}");
		if (query.DateRangeSet)
		{
			yesNo = "Yes";
		}
		else {
			yesNo = "No";
		}

		Console.WriteLine($"Is a date range set?: {yesNo}");
		if (query.DateRangeSet)
		{
		   Console.WriteLine($"Year Range: {query.StartYear} - {query.EndYear}");
		}
	
		Console.WriteLine("-------------------------");
		Console.WriteLine("Toggle obsolete usage - O");
		Console.WriteLine("Set date range:       - D");
		Console.WriteLine("Exit to main menu     - X");
		var input = Console.ReadLine().Trim().ToLower();
		switch (input)
		{
			case ("o" or "obsolete" or "ob"):
				Console.WriteLine("Toggle Obsolete");
				query.IncludeObsolete = false;
				break;
			case ("d" or "date" or "date range"):
				Console.WriteLine("Enter Date Range");
				(query.StartYear, query.EndYear) = SetYears(query);
				break;
			case ("x" or "e" or "exit"):
				Console.WriteLine("Exiting back to main menu.");
				running = false;
				break;

			default:
				Console.WriteLine("Did not recognize input");
			break;
		}

	static (int startYear, int endYear) SetYears(CurrentQuery query)
	{
		int startYearInt = 0;
		int endYearInt = 0;

		var setYearsRunning = true;
		while (setYearsRunning)
		{
			Console.WriteLine("Only get results within this date range:");
			Console.WriteLine("Starting Year: ");
			Console.WriteLine("(or Press Enter for open ended start)");
		 	string startYear = Console.ReadLine().Trim().ToLower();

			if (string.IsNullOrWhiteSpace(startYear)) {
				query.OpenStart = true;
				startYearInt = 0;
				Console.WriteLine("Set to open ended start year.");
			} 
			else if (validate(startYear)) {
				startYearInt = Int16.Parse(startYear);
			} else {
			    break;
			}
			Console.WriteLine("Ending Year:");
			Console.WriteLine("(or Press Enter for open ended end)");
			string endYear = Console.ReadLine().Trim().ToLower();
			if (string.IsNullOrWhiteSpace(endYear)) {
				if (query.OpenStart) {
					Console.WriteLine("You can not have both an open start and an open end date.");
					query.DateRangeSet = false;
					Console.WriteLine("Please start over.");
					break;
				}
				query.OpenEnd = true;
				endYearInt = 0;
				Console.WriteLine("Set to open ended end year.");
				setYearsRunning = false;
				break;
			} 
			else if (validate(endYear)) {
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
				Console.WriteLine("Invalid Year.");
				Console.WriteLine("Please enter a 3 digit or 4 digit year.");
				return false;
				}
			}
		}
		return (startYearInt, endYearInt);

	}
     }
 }
}
