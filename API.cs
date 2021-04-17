using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Diagnostics;

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
			Uri requestURL = new Uri(baseURL + @"monitor_nn01/senses/");
			Trace.WriteLine("Making the request");
			Trace.WriteLine(client.GetStringAsync(requestURL));
		};

		Action<object> callQuotationsAPI = (Object obj) => 
		{
			Trace.WriteLine("Called callQuotationsAPI");
			Uri requestURL = new Uri(baseURL + @"orchestra_nn01/quotations/");
			Trace.WriteLine("Making the request");
			Trace.WriteLine(client.GetStringAsync(requestURL));
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
			Uri requestURL = new Uri(baseURL + @"lemmatize/?form=peas");
			Trace.WriteLine("Making the request");
			Trace.WriteLine(client.GetStringAsync(requestURL));
		};
		// @TODO an action to set headers
		// Make a new Task for each API call
		

		if (query.QueryMode == Modes.Word) 
		{
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
			query.Definition= definitionRegEx.Match(apiDataString).ToString();
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
				string rootInfo = JSONResponse.RootElement.GetProperty("data").ToString();
				Console.WriteLine();
				// Etymology_summary
				//
				// first_use 
				// 
				// Source Language
				// (?<="source_language":\s\[\["European languages",)(.*?)(?=\])

			}
		}
		else if (query.QueryMode == Modes.Lammatize)
		{
			Trace.WriteLine("Now to call the .");
		}
		else 
		{
			Console.WriteLine("Query mode not correctly set.");
		}

		client.Dispose();
	}
    }
}
