using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace OxfordV2
{
    class API
    {

	public static JsonDocument JSONResponse { get; set; }


	static void resetHeaders(HttpClient client)
	{
		try {
		   UserInfo.GetKeys();
		}
		catch(Exception ex)
		{
			Trace.WriteLine("Could not get the API key.");
			Trace.WriteLine($"{ex.GetType()} says {ex.Message}");
			throw;
		}
		Trace.WriteLine("resetHeaders called.");
		Trace.WriteLine("App Id is:");
		Trace.WriteLine(UserInfo.appId);
		Trace.WriteLine("Key is:");
		Trace.WriteLine(UserInfo.appKey);
		// Use client.CancelPendingRequests? To make sure all is clear?
		client.CancelPendingRequests();
		client.DefaultRequestHeaders.Clear();
		client.DefaultRequestHeaders.Accept.Add(
			new MediaTypeWithQualityHeaderValue("application/json"));
		// Not using the Authorization headers i.e. client.DefaultRequestHeaders.Authorization
		client.DefaultRequestHeaders.Add("app_id", UserInfo.appId);
		client.DefaultRequestHeaders.Add("app_key", UserInfo.appKey);
		Trace.WriteLine("Here are the request Headers...");
		Trace.WriteLine(client.DefaultRequestHeaders);

	}

	public static void APICalls(CurrentQuery query)
	{
		// Have CurrentQuery - QueryMode - tell 
		// Which API to call next 
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
		var client = new HttpClient();
		string baseURL = "https://oed-researcher-api.oxfordlanguages.com/oed/api/v0.2/";

		// @TODO make this one Action delegate - with a method that parses the query and responses
		Action<object> callWordsAPI = (Object obj) => 
		{
			Trace.WriteLine("Called callWordsAPI");
			Uri requestURL = new Uri(baseURL + "words/?lemma=" + query.UserEnteredWord + "&limit=1");
			Trace.WriteLine("Making the request");

			var response = client.GetStreamAsync(requestURL).Result;
			Trace.WriteLine("Got responses.");
			JSONResponse = JsonDocument.Parse(response);
			Trace.WriteLine("Set JSONResponse to the response.");
		};

			

		Action<object> callWordsByIdAPI = (Object obj) => 
		{
			Trace.WriteLine("Called callWordsByIdAPI");
			Uri requestURL = new Uri(baseURL + @"word/orchestra\_nn01?include\_senses=false&include\_quotations=false");
			Trace.WriteLine("Making the request");
			Trace.WriteLine(client.GetStringAsync(requestURL));
		};

		Action<object> callSensesAPI = (Object obj) => 
		{
			Trace.WriteLine("Called callSensesAPI");
			string queryURL;
			if (! query.IncludeObsolete) {
				queryURL = @"word/" + query.WordID + @"/senses/?obsolete=false";
			}
			else {
				queryURL = @"word/" + query.WordID + @"/senses/";
			}
			Uri requestURL = new Uri(baseURL + queryURL);
			Trace.WriteLine("Making the request");
			try {
				client.Timeout = TimeSpan.FromMinutes(10);

				var response = client.GetStreamAsync(requestURL).Result;
				Trace.WriteLine(response);
			}
			catch (AggregateException ae)
			{
				Console.WriteLine("Caught aggregate exception-Task.Wait behavior");
				var flatExs = ae.Flatten().InnerExceptions;
				foreach (var ex in flatExs)
				{
					Console.WriteLine($"{ex}");
				}
				ae.Handle( (x) => 
				{
					if (x is UnauthorizedAccessException) // This we know how to handle.
					{
						Console.WriteLine("You do not have access.");
						return true;
					}
					return false; // Let anything else stop the application.
				});
			}

			try {
			var response = client.GetStreamAsync(requestURL).Result;
			Trace.WriteLine("Got senses responses.");
			JSONResponse = JsonDocument.Parse(response);
			Trace.WriteLine("Set JSONResponse to the response.");
			}
			catch(Exception ex)
			{
				Trace.WriteLine("Exception");
				Trace.WriteLine(ex.GetType());
				Trace.WriteLine(ex.Message);
			}

		};

		Action<object> callQuotationsAPI = (Object obj) => 
		{
			Trace.WriteLine("Called callQuotationsAPI");

			Uri requestURL; 
			string startYear; 
			string endYear;
			if (query.DateRangeSet) {
				if (query.OpenStart)
				{
					startYear = "";
					endYear = Convert.ToString(query.EndYear);
				}
				else if (query.OpenEnd)
				{
					startYear = Convert.ToString(query.StartYear);
					endYear = "";
				}
				else {
					startYear = Convert.ToString(query.StartYear);
					endYear = Convert.ToString(query.EndYear);
				}
				requestURL = new Uri(baseURL + "word/" + query.WordID + "/quotations/" + "?year=" + startYear + "-" + endYear);
			}
			else {
				requestURL = new Uri(baseURL + "word/" + query.WordID + "/quotations/");
			}
			
			try {
			var response = client.GetStreamAsync(requestURL).Result;
			Trace.WriteLine("Got quotation responses.");
			JSONResponse = JsonDocument.Parse(response);
			Trace.WriteLine("Set JSONResponse to the response.");
			}
			catch(Exception ex)
			{
				Trace.WriteLine("Exception");
				Trace.WriteLine(ex.GetType());
				Trace.WriteLine(ex.Message);
			}
		};

		Action<object> callRootsAPI = (Object obj) => 
		{
			Trace.WriteLine("Called callRootsAPI");
			Uri requestURL = new Uri(baseURL + query.WordID + "/roots/");
			Trace.WriteLine("Making the request");
			Trace.WriteLine(client.GetStringAsync(requestURL));

			var response = client.GetStreamAsync(requestURL).Result;
			Trace.WriteLine("Got responses from ROOTS.");
			JSONResponse = JsonDocument.Parse(response);
			Trace.WriteLine("Set JSONResponse to the response.");
		};

		Action<object> callDerivativesAPI = (Object obj) => 
		{
			Trace.WriteLine("Called callDerivativesAPI");
			Uri requestURL = new Uri(baseURL + @"word/brain_nn01/derivatives/");
			Trace.WriteLine("Making the request");
			Trace.WriteLine(client.GetStringAsync(requestURL));
		};

		Action<object> callSurfaceformsAPI = (Object obj) => 
		{
			Trace.WriteLine("Called callSurfaceformsAPI");
			Uri requestURL = new Uri(baseURL + @"word/musket_nn02/surfaceforms/");
			Trace.WriteLine("Making the request");
			Trace.WriteLine(client.GetStringAsync(requestURL));
		};

		Action<object> callFrequencyAPI = (Object obj) => 
		{
			Trace.WriteLine("Called callFrequencyAPI");
			Uri requestURL = new Uri(baseURL + @"word/brain_nn01/frequency/");
			Trace.WriteLine("Making the request");
			Trace.WriteLine(client.GetStringAsync(requestURL));
		};

		Action<object> callLemmatizeAPI = (Object obj) => 
		{
			Trace.WriteLine("Called callLemmatizeAPI");
			Uri requestURL = new Uri(baseURL + @"lemmatize/?form=" + query.UserEnteredWord);
			Trace.WriteLine("Making the request");
			Trace.WriteLine(client.GetStringAsync(requestURL).Result);

			try {
			var response = client.GetStreamAsync(requestURL).Result;
			Trace.WriteLine("Got Lamma response.");
			JSONResponse = JsonDocument.Parse(response);
			Trace.WriteLine("Set JSONResponse to the response.");
			}
			catch(Exception ex)
			{
				Trace.WriteLine("Exception hit during Lamma call");
				Trace.WriteLine(ex.GetType());
				Trace.WriteLine(ex.Message);
			}
		};
		// @TODO an action to set headers
		// Make a new Task for each API call
		

		if (query.QueryMode == Modes.Word) 
		{
			// @TODO put this repeated parsing in a method
			// Use enums to select which option
			Trace.WriteLine("Found that QueryMode is set to words.");
			Trace.WriteLine("Looking up the word:" );
			Trace.WriteLine(query.UserEnteredWord);
			Trace.WriteLine("Now to call the words endpoint.");
			resetHeaders(client);
			Task getWords = new Task(callWordsAPI, "CallWords");
			Trace.WriteLine("Calling the API");
			getWords.RunSynchronously();
			Trace.WriteLine("Left getWords task with the JSONResponse.");
			Trace.WriteLine("Parsing JSON");
			JsonElement apiData = JSONResponse.RootElement.GetProperty("data");
			// Malformed JSON data is returned.  Only 1 element in "data" property
			// Convert To String and RegEx the string
			string apiDataString = apiData.ToString();
			Trace.WriteLine("We now have the data as a string.");
			Trace.WriteLine(apiDataString);
			var definitionRegEx = new Regex("(?<=definition\":\\s)(.*?)(?=\",\\s\"main_entry\")");
			query.Definition = definitionRegEx.Match(apiDataString).ToString();
			Trace.WriteLine("Extracted definition.");
			Console.WriteLine(query.Definition);
			Trace.WriteLine("Set definition to query object.");
			Trace.WriteLine("Now to get and set the word ID.");
			var wordIdRegex = new Regex("(?<=\"id\":\\s\")(.*?)(?=\",)");
			query.WordID = wordIdRegex.Match(apiDataString).ToString();
			Trace.WriteLine("The wordID was grabbed as:");
			Trace.Write(query.WordID);
			query.HasLookedUpWord = true;

		}
		else if (query.QueryMode == Modes.Root)
		{
			Trace.WriteLine("Now to call the root endpoint.");
			if (query.HasLookedUpWord == false) 
			{
				Console.WriteLine("You need to first ask for a definition, sorry..."); 
			}
			else 
			{
				Trace.WriteLine("Going to look up roots");
				Trace.WriteLine("API call URL is:");
				Trace.WriteLine(baseURL);
				Trace.WriteLine("Making the request:");
				resetHeaders(client);
				Task getRoots = new Task(callRootsAPI, "CallRoots");
				Trace.WriteLine("Calling the API to get ROOTS. Bloody Roots!!");
				getRoots.RunSynchronously();
				Trace.WriteLine("Ran roots synchronously.");
				// JsonElement apiData = JSONResponse.RootElement.GetProperty("data");
				string rootDataString = JSONResponse.RootElement.GetProperty("data").ToString();
				Console.WriteLine();
				// Etymology_summary
				// (?<="etymology_summary":\s")(.*?)(?="},)
				var etymologySummaryRegEx = new Regex("(?<=\"etymology_summary\":\\s\")(.*?)(?=\"},)");
				query.EtymologySummary = etymologySummaryRegEx.Match(rootDataString).ToString();
				Console.WriteLine("Where the word \"{0}\" came from:", query.UserEnteredWord);
				Console.WriteLine(query.EtymologySummary);
				Console.WriteLine();


				// first_use 
				// (?<="first_use":\s")(.*?)(?=",) 
				var firstUseRegEx = new Regex("(?<=\"first_use\":\\s\")(.*?)(?=\",)");
				query.FirstUse = firstUseRegEx.Match(rootDataString).ToString();
				Console.WriteLine("First Recorded Use:");
				Console.WriteLine(query.FirstUse);
				Console.WriteLine();
				

				// Source Language
				// (?<="source_language":\s\[\["European languages",)(.*?)(?=\])
				var sourceLanguageRegEx = new Regex("(?<=\"source_language\":\\s\\[\\[\"European languages\",)(.*?)(?=\\])");
				query.SourceLanguage = sourceLanguageRegEx.Match(rootDataString).ToString();
				Console.WriteLine("The word \"{0}\" has origins in these European languages: ", query.UserEnteredWord);
				Console.WriteLine(query.SourceLanguage);
				Console.WriteLine("-----------");
				Console.WriteLine("Please Press Enter...");
				Console.ReadLine();


			}
		}
		else if (query.QueryMode == Modes.Lammatize)
		{
			Trace.WriteLine("Now to call the Lammatize API");
			Trace.WriteLine("Figure out of one word or multiple words have been entered.");
			if (query.HasLookedUpWord == false)
			{
				// @TODO remove this. Have it auto-call up the first ID.
				Console.WriteLine("You need to first ask for a definition.");
			}
			else 
			{
				resetHeaders(client);
				Task getLemmas = new Task(callLemmatizeAPI, "CallSenses");
				getLemmas.ConfigureAwait(false);
				getLemmas.RunSynchronously();
				Console.WriteLine("Getting Lammas");

				JsonElement root = JSONResponse.RootElement;
				JsonElement lemmaData = root.GetProperty("data");
				JsonElement firstWord = lemmaData[0].GetProperty("word");
				/*
				foreach (object item in firstWord.EnumerateObject())
				{
					Console.WriteLine(item);
					Console.ReadLine();
				}
				*/
				string lemma = firstWord.GetProperty("lemma").GetString();
				Console.WriteLine($"The lemma of {query.UserEnteredWord} is {lemma}");
//				Console.WriteLine(lamma.ToString());
				
			}
		}
		else if (query.QueryMode == Modes.Senses)
		{
			Trace.WriteLine("API.cs is starting senses mode.");
			if (query.HasLookedUpWord == false)
			{
				// @TODO remove this. Have it auto-call up the first ID.
				Console.WriteLine("You need to first ask for a definition.");
			}
			else 
			{
				resetHeaders(client);
				Task getSenses = new Task(callSensesAPI, "CallSenses");
				getSenses.ConfigureAwait(false);
				getSenses.RunSynchronously();


			Trace.WriteLine("Ran senses using start.");

				JsonElement senseData = JSONResponse.RootElement.GetProperty("data");

				Sense currentSense = new();
				foreach (JsonElement item in senseData.EnumerateArray())
				{
				try {
					// Print the quote
					
					currentSense.Definition = item.GetProperty("definition").ToString();
					currentSense.Start = item.GetProperty("daterange").GetProperty("start").GetInt16(); 
					currentSense.IsObsolete = item.GetProperty("daterange")
						.GetProperty("obsolete").GetBoolean();
				    currentSense.IsMainUsage = item.GetProperty("main_current_sense").GetBoolean();
					currentSense.OedReference = item.GetPropertyExt("oed_reference")?.ToString();
					// currentSense.Usage = item.GetProperty("")
					Console.WriteLine(currentSense.Definition);
					Console.WriteLine("Sense first used: {0}", currentSense.Start);
					if (currentSense.IsObsolete)
					{
						Console.WriteLine("This usage is obsolete.");
					}
					else 
					{
						Console.WriteLine("This sense is not obsolete.");
					}
					if (currentSense.IsMainUsage)
					{
						Console.WriteLine("This sense is the main sense for the word.");
					}
					else 
					{
						Console.WriteLine("Not the main sense for the word.");
					}
					Console.WriteLine(currentSense.OedReference);

					query.Senses.Add(currentSense);
					Console.WriteLine();

					Console.WriteLine("---- S to Save - X to exit - Enter for more----");
					string input = Console.ReadLine().Trim().ToLower();
					if (input == "s")
					{
					    SavedQueries.AddMember(currentSense);
					}
					else if (input == "x")
					    break;
					}
					catch (Exception ex)
					{
						Trace.WriteLine(ex);
					}

				}
			}
		}
		else if (query.QueryMode == Modes.Quotations)
		{
			Trace.WriteLine("API.cs is starting quotations mode.");
			if (query.HasLookedUpWord == false)
			{
				Console.WriteLine("Quotations you need to ask for a definition.");
				Console.WriteLine("first.");
			}
			else
			{
				resetHeaders(client);
				Task getQuotes = new Task(callQuotationsAPI, "Call Quotations");
				getQuotes.RunSynchronously();
				Trace.WriteLine("Ran quotations synchronously.");
				Trace.WriteLine("Parsing quotations JSON.");

				JsonElement root = JSONResponse.RootElement;
				JsonElement quoteData = root.GetProperty("data");
				foreach (JsonElement item in quoteData.EnumerateArray())
				{
					try {
					// Print the quote
					JsonElement quoteInfoBlock = item.GetProperty("text");
					JsonElement actualQuote = quoteInfoBlock.GetProperty("full_text");

					// Get the source of the quote
					JsonElement quoteSourceBlock = item.GetProperty("source");
					JsonElement quoteTitle = quoteSourceBlock.GetProperty("title");
					JsonElement quoteAuthor = quoteSourceBlock.GetProperty("author");

					
					// quoteSourceBlock.TryGetProperty("author", out JsonElement quoteAuthor);


					// Get what year the quote is from
					JsonElement quoteYear = item.GetProperty("year");
					Console.WriteLine("\"{0}\", Year: {1}, Source: {2} {3}", 
						actualQuote.ToString(), quoteYear.ToString(), quoteAuthor.ToString(), 
						quoteTitle.ToString());
				    Quote currentQuote = new();
					currentQuote.Year = quoteYear.GetInt16();
					currentQuote.Text = actualQuote.ToString();
					currentQuote.Title = quoteTitle.ToString();
					currentQuote.Author = quoteAuthor.ToString();

					// @TODO add all quotes whether you have seen them or not?
					query.Quotes.Add(currentQuote);
					Console.WriteLine();
					Console.WriteLine("---- S to Save - X to exit - Enter for more----");
					string input = Console.ReadLine().Trim().ToLower();
					if (input == "s")
					{
					    Console.WriteLine("Quote saved.");
						SavedQueries.AddMember(currentQuote);
					}
					else if (input == "x")
					    break;
					}
					catch (Exception ex) {
						Console.WriteLine(ex);
					}
					
				}	
				/*
				string quotesDataString = JSONResponse.RootElement.GetProperty("data").ToString();
				// (?<="full_text":\s")(.*?)(?=",)
				var quotesRegex = new Regex("(?<=\"full_text\":\\s\")(.*?)(?=\",)");
				query.NumberOfQuotes = quotesRegex.Matches(quotesDataString).Count;
				// query.Quotes = quotesRegex.Matches(quotesDataString);
				Console.WriteLine("{0} quotes found.", query.NumberOfQuotes);
				// query.Quote = quotesRegex.Match(quotesDataString).ToString();
				foreach (Match match in Regex.Matches(quotesDataString, quotesRegex.ToString()))
				{
					string input = "";
					Console.WriteLine("\"{0}\"", match.Value);
				}
				*/

				Trace.WriteLine("First quote grabbed as:");
				Trace.WriteLine(query.Quote);


				Console.WriteLine(query.Quote);
			}
		}
		else 
		{
			Console.WriteLine("Query mode not correctly set.");
		}

		// client.Dispose();
	}
    }
}
// @TODO If a word is not found return a message "word not found in dictionary."  
// Instead of just a blank response.
