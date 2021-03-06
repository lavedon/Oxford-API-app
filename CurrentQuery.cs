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
		public string UserEnteredWord { get; set; } 
		
		public string CurrentSenseID { get; set; }
		public string Lamma { get; set; }
		public string Quote { get; set; }

		public List<Definition> Definitions { get; set; }
		public List<Quote> Quotes { get; set; }
		public List<Sense> Senses { get; set; }

		public string EtymologySummary { get; set; }
		public string FirstUse { get; set; }
		public string SourceLanguage { get; set; }

		public Modes QueryMode { get; set; }

		public bool HasLookedUpWord { get; set; }
		public bool OptionsMenuVerboseMode { get; set;}
		public bool? IncludeObsolete { get; set; }
		public bool DateRangeSet { get; set; }

		public int NumberOfQuotes { get; set; }
		public int StartYear { get; set; }
		public bool OpenStart { get; set;}
		public int EndYear { get; set; }
		public bool OpenEnd { get; set;}

		public CurrentQuery() {
			this.HasLookedUpWord = false;
			this.DateRangeSet = false;
			this.IncludeObsolete = null;
			this.OptionsMenuVerboseMode = true;
			this.Definitions = new List<Definition>();
			this.Quotes = new List<Quote>();
			this.Senses = new List<Sense>();
			this.StartYear = 0;
			this.OpenStart = false;
			this.EndYear = 0;
			this.OpenEnd = false;
			
		} 
		public string Source { get; set; }

		public void Dispose ()
		{
			Trace.WriteLine("Called Dispose");
			Trace.WriteLine("Dispose not yet implemented.");
		}
	}
}
