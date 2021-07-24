#nullable enable
using System;

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

		public string? Topic { get; set; }
		public bool RestrictMain { get; set; }


		public SenseOptions(string? lemma, string? restrictRegion, string? restrictUsage, bool restrictMain, string? topic)
		{
			this.Lemma = lemma;
			this.RestrictRegion = restrictRegion;
			this.RestrictUsage = restrictUsage;
			this.RestrictMain = restrictMain;
			this.Topic = Topic;
		}
	}
}
