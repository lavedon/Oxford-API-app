#nullable enable
#nullable disable warnings
using System;
using System.Collections.Generic;

namespace oed
{
	public class SenseOptions
	{
		private string? _restrictRegion;
		private string? _restrictUsage;
		private string? _lemma;

		public string? Lemma {
			get => _lemma;
			set => _lemma = value!.Trim().ToLower();
		}
		public string? RestrictRegion { 
			get => _restrictRegion; 
			set => _restrictRegion = value!.Trim().Replace(" ","%20");
		} 
		public string? RestrictUsage { 
			get => _restrictUsage; 
			set {
			if(value!.Contains("slang")) 
			{
				_restrictUsage = "colloquial%20and%20slang";
			} else {
				_restrictUsage = value!.Trim().Replace(" ","%20");
				}
			}
		}

		public string? FromDefinition { get; set; } = null;
		public List<string> WordIDsToUse { get; set; }

		//@TODO Do this on WORDIDS? 
		// Should quotes save word IDS?
		public string? FromQuotes { get; set; } = null;
		public List<string> QuotesToUse { get; set; }

		public string? Topic { get; set; }
		public bool RestrictMain { get; set; }


		public SenseOptions(string? lemma, string? restrictRegion, string? restrictUsage, bool restrictMain, string? topic, string? fromDefinition, string? fromQuotes)
		{
			this.Lemma = lemma;
			this.RestrictRegion = restrictRegion;
			this.RestrictUsage = restrictUsage;
			this.RestrictMain = restrictMain;
			this.Topic = Topic;
			this.FromDefinition = fromDefinition;
			this.FromQuotes = fromQuotes;
			this.WordIDsToUse = new List<string>();
		}
	}
}
