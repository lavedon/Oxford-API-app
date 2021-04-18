using System;
using System.Diagnostics;

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
	class CurrentQuery : IDisposable
	{
		public bool HasLookedUpWord { get; set; }
		public string WordID { get; set; }
		// Create the object the same time the user looks up a word
		public string UserEnteredWord { get; set; } 
		public string Definition { get; set; }
		public string[] SenseIDs { get; set; }
		public string Lamma { get; set; }
		public int NumberOfQuotes { get; set; }
		public string Quote { get; set; }
		public string[] Quotes { get; set; }

		public string EtymologySummary { get; set; }
		public string FirstUse { get; set; }
		public string SourceLanguage { get; set; }

		public Modes QueryMode { get; set; }

		public CurrentQuery() {
			this.HasLookedUpWord = false;
		} 

		public void Dispose ()
		{
			Trace.WriteLine("Called Dispose");
			Trace.WriteLine("Dispose not yet implemented.");
		}
	}
}
