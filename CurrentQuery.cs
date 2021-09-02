#nullable enable
#nullable disable warnings
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace oed
{
	public enum Modes
	{
		Word,
		Root,
		Lammatize,
		Senses,
		Quotations,
		Surfaces,
		Derivatives,
		QuotesAndSenses
		
	}
	public class CurrentQuery : IDisposable
	{
		// Create the object the same time the user looks up a word
		public SenseOptions? CurrentSenseOptions { get; set; }
		public QuoteOptions? CurrentQuoteOptions { get; set; }
		public SurfaceOptions? CurrentSurfaceOptions { get; set; }
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
		public Lemmas Lemmas { get; set; }

		public string EtymologySummary { get; set; }
		public string? EtymologyType { get; set; }
		public string? EtymologyLanguage { get; set; }
		public string FirstUse { get; set; }
		public string SourceLanguage { get; set; }
		public string? PartsOfSpeech { get; set; }


		public Modes QueryMode { get; set; }

		public bool HasLookedUpWord { get; set; }
		public bool OptionsMenuVerboseMode { get; set;}
		public bool? IncludeObsolete { get; set; }
		public bool? IncludeRevised { get; set; }
		public bool DateRangeSet { get; set; }

		public int NumberOfQuotes { get; set; }
		public bool CurrentIn { get; set; }
		public string? StartYearString { get; set; }
		public int StartYear { get; set; }
		public bool OpenStart { get; set;}
		public int EndYear { get; set; }
		public string? EndYearString { get; set; }
		public bool StartEndYearRanges { get; set; }
		public bool OpenEnd { get; set;}
		public bool QuotesFromWord { get; set ; }
		public bool QuotesAndSenses { get; set ; }
		public List<int> QSDefSelection { get; set; }
		public List<int> QSSenseSelection { get; set; }
		public bool QSFromSenses { get; set; }
		public bool QSFromDefinitions { get; set; }

		// RootCommand if user wants to get definitions
		// for past returned quote or past returned sense.
		public string? FromSense { get; set; } = null;
		public List<string> WordIDsToUse { get; set; }
		public List<string> BlankDefsWithIDs { get; set; }
		public bool InteractiveMode { get; set; }
		public bool ExportAfterSearch { get; set; }
		public List<int> WhatToExport { get; set; }
		public bool ExportAll { get; set;}
		public string? CurrentWordID { get; set; }
		// public List<string> PreviousWordIds { get; set;}

		public List<Datum> Surfaces { get; set; }
		// public SurfaceFormDeJSON? SurfaceJson  { get; set;}
		public List <Derivatives> Derivatives { get; set; }
		// SQ_Data is Data taken from the word/{id}/?include=quotes&include=senses endpoint
		// Data is displayed differently than it is from the sense/{id}/ endpoint
		// Most notably, sense/{id}/ includes a list of quotations which SQ_Data does not.
		public List <SQ_Data> SQ_Data { get; set; }

		// Data from the sense/{id}/?include=quotations
		public List <SQID_Data> SQID_Data { get; set; }

		public SQID_Root SQID_Root { get; set; }

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
			this.EtymologySummary = null;
			this.CurrentIn = false;
			this.CurrentWordID = null;
			this.StartYear = 0;
			this.OpenStart = false;
			this.EndYear = 0;
			this.OpenEnd = false;
			this.InteractiveMode = false;
			this.ExportAfterSearch = false;
			this.WhatToExport = new List<int>();
			this.CurrentSenseOptions = null;
			this.CurrentQuoteOptions = null;
			this.CurrentSurfaceOptions = null;
			this.LemmaText = "";
			this.QuotesFromWord = false;
			this.QuotesAndSenses = false;
			this.QSFromSenses = false;
			this.QSFromDefinitions = false;
			this.QSDefSelection = new List<int>();
			this.QSSenseSelection = new List<int>();

			// this.SurfaceJson = new();
			this.Surfaces = new List<Datum>();
			this.Derivatives = new List<Derivatives>();
			this.SQ_Data = new List<SQ_Data>();
			this.SQID_Data = new List<SQID_Data>();
			this.WhatToExport = new List<int>();
			this.FromSense = "";
			this.WordIDsToUse = new List<string>();
			this.BlankDefsWithIDs = new List<string>();
			this.StartEndYearRanges = false;
			// this.PreviousWordIds = new();
			
		} 
		public string Source { get; set; }

		public void Dispose ()
		{
			Trace.WriteLine("Called Dispose");
			Trace.WriteLine("Dispose not yet implemented.");
		}
	}
}
