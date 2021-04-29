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
		if (query.StartYear == 0 || query.EndYear == 0) 
		{
			yesNo = "No";
		}
		else {
			yesNo = "Yes";
		}

		Console.WriteLine($"Is a date range set?: {yesNo}");
		if (yesNo == "Yes")
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
				SetYears();
				break;
			case ("x" or "e" or "exit"):
				Console.WriteLine("Exiting back to main menu.");
				running = false;
				break;

			default:
				Console.WriteLine("Did not recognize input");
			break;
		}

	static (int startYear, int endYear) SetYears()
	{
		int startYearInt = 0;
		int endYearInt = 0;

		var setYearsRunning = true;
		while (setYearsRunning)
		{
			Console.WriteLine("Only get results within this date range:");
			Console.WriteLine("Starting Year:");
		 	string startYear = Console.ReadLine().Trim().ToLower();
			if (validate(startYear)) {
				startYearInt = Int16.Parse(startYear);
			} else {
			    break;
			}
			Console.WriteLine("Ending Year:");
			
			string endYear = Console.ReadLine().Trim().ToLower();
			if (validate(endYear)) {
				endYearInt = Int16.Parse(endYear);
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
