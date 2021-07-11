#nullable enable
#nullable disable warnings
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace OxfordV2
{
	public enum Modes
	{
		Word,
		Root,
		Lammatize,
		Senses,
		Quotations
	}
	public class CurrentQuery : IDisposable
	{
		// Create the object the same time the user looks up a word
		public SenseOptions? CurrentSenseOptions { get; set; }
		public QuoteOptions? CurrentQuoteOptions { get; set; }
		public string UserEnteredWord { get; set; } 
		
		private string _lemmaText;
		public string LemmaText { 
			get { return _lemmaText; } 
			set { 
				_lemmaText = value.Replace(" ", "%20");
			}}
		public string CurrentSenseID { get; set; }
		public string Quote { get; set; }

		public List<Definition> Definitions { get; set; }
		public List<Quote> Quotes { get; set; }
		public List<Sense> Senses { get; set; }

		public string EtymologySummary { get; set; }
		public string? EtymologyType { get; set; }
		public string? EtymologyLanguage { get; set; }
		public string FirstUse { get; set; }
		public string SourceLanguage { get; set; }
		public string? PartsOfSpeech { get; set; }


		public Modes QueryMode { get; set; }
		public Lemmas Lemmas { get; set; }

		public bool HasLookedUpWord { get; set; }
		public bool OptionsMenuVerboseMode { get; set;}
		public bool? IncludeObsolete { get; set; }
		public bool? IncludeRevised { get; set; }
		public bool DateRangeSet { get; set; }

		public int NumberOfQuotes { get; set; }
		public bool CurrentIn { get; set; }
		public int StartYear { get; set; }
		public bool OpenStart { get; set;}
		public int EndYear { get; set; }
		public bool OpenEnd { get; set;}
		public bool InteractiveMode { get; set; }
		public bool ExportAfterSearch { get; set; }
		public string? CurrentWordID { get; set; }

		public CurrentQuery() {
			this.HasLookedUpWord = false;
			this.DateRangeSet = false;
			this.IncludeObsolete = null;
			this.IncludeRevised = null;
			this.PartsOfSpeech = null;
			this.EtymologyType = null;
			this.EtymologyLanguage = null;
			this.OptionsMenuVerboseMode = true;
			this.Definitions = new List<Definition>();
			this.Quotes = new List<Quote>();
			this.Senses = new List<Sense>();
			this.Lemmas = new Lemmas();
			this.EtymologySummary = 
			this.CurrentIn = false;
			this.CurrentWordID = null;
			this.StartYear = 0;
			this.OpenStart = false;
			this.EndYear = 0;
			this.OpenEnd = false;
			this.InteractiveMode = false;
			this.ExportAfterSearch = false;
			this.CurrentSenseOptions = null;
			this.CurrentQuoteOptions = null;
			this.LemmaText = "";
			
		} 
		public string Source { get; set; }

		public void Dispose ()
		{
			Trace.WriteLine("Called Dispose");
			Trace.WriteLine("Dispose not yet implemented.");
		}
	}
}
