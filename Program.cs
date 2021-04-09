using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace OxfordV2
{
    class Program
    {
	static void resetHeaders(HttpClient client)
	{
		try {
		   UserInfo.GetKeys();
		}
		catch(Exception ex)
		{
			Console.WriteLine("Could not get the API key.");
			Console.WriteLine($"{ex.GetType()} says {ex.Message}");
			throw;
		}
		Console.WriteLine("resetHeaders called.");
		Console.WriteLine("App Id is:");
		Console.WriteLine(UserInfo.appId);
		Console.WriteLine("Key is:");
		Console.WriteLine(UserInfo.appKey);
		// Use client.CancelPendingRequests? To make sure all is clear?
		client.CancelPendingRequests();
		client.DefaultRequestHeaders.Clear();
		client.DefaultRequestHeaders.Accept.Add(
			new MediaTypeWithQualityHeaderValue("application/json"));
		// Not using the Authorization headers i.e. client.DefaultRequestHeaders.Authorization
		client.DefaultRequestHeaders.Add("app_id", UserInfo.appId);
		client.DefaultRequestHeaders.Add("app_key", UserInfo.appKey);
		Console.WriteLine("Here are the request Headers...");
		Console.WriteLine(client.DefaultRequestHeaders);

	}

        static void Main(string[] args)
        {
		APICalls();
        }

	static void APICalls()
	{
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
		var client = new HttpClient();
		string baseURL = "https://oed-researcher-api.oxfordlanguages.com/oed/api/v0.2/";
		client.BaseAddress = baseURL;

		Action<object> callWordsAPI = (Object obj) => 
		{
			Console.WriteLine("Called callWordsAPI");
			Uri requestURL = new Uri("words/?lemma=mail");
			Console.WriteLine("Making the request");
			Console.WriteLine(client.GetStringAsync(requestURL));
		};

		Action<object> callWordsByIdAPI = (Object obj) => 
		{
			Console.WriteLine("Called callWordsByIdAPI");
		};

		Action<object> callSensesAPI = (Object obj) => 
		{
			Console.WriteLine("Called callSensesAPI");
		};

		Action<object> callQuotationsAPI = (Object obj) => 
		{
			Console.WriteLine("Called callQuotationsAPI");
		};

		Action<object> callRootsAPI = (Object obj) => 
		{
			Console.WriteLine("Called callRootsAPI");
		};

		Action<object> callDerivativesAPI = (Object obj) => 
		{
			Console.WriteLine("Called callDerivativesAPI");
		};

		Action<object> callSurfaceformsAPI = (Object obj) => 
		{
			Console.WriteLine("Called callSurfaceformsAPI");
		};

		Action<object> callFrequencyAPI = (Object obj) => 
		{
			Console.WriteLine("Called callFrequencyAPI");
		};

		Action<object> callLemmatizeAPI = (Object obj) => 
		{
			Console.WriteLine("Called callLemmatizeAPI");
		};
		// @TODO an action to set headers
		var callAPI = new Task<string>( () => { Console.WriteLine("Task {0} (syncTask) executing on Thread {1}",
									Task.CurrentId,
									Thread.CurrentThread.ManagedThreadId);
						// Reset headers before each call
						resetHeaders(client);
						string cat = "muffin";
						Task apiCall = Task.Factory.StartNew(callWordsAPI, "test");
						apiCall.Wait();

						return cat;
						});
		callAPI.RunSynchronously();
		Console.WriteLine("A crazy cat is named {0}", callAPI.Result);
		
	}
    }
}
