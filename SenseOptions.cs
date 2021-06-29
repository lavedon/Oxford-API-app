#nullable enable
using System;

namespace OxfordV2
{
	public class SenseOptions
	{
		private string? _restrictRegion;
		private string? _lemma;

		public string? Lemma {
			get => _lemma;
			set => _lemma = value!.Trim().ToLower();
		}
		public string? RestrictRegion { 
			get => _restrictRegion; 
			set => _restrictRegion = value!.Trim().Replace(" ","%20");
		} 
		public string? RestrictUsage { get; set; }

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
