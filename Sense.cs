using System.Collections.Generic;

namespace OxfordV2
{
	public class Sense
	{
		public string Definition { get; set; }
		public int Start { get; set; } 
		public bool IsObsolete { get; set; }
		public string PrimarySenseID { get; set; }
		public string EtymologySummary { get; set; }

		public Dictionary<string, string> Etymons { get; set; }
		public Dictionary<string, string> Inflections { get; set; }
		
		public Sense() {
			this.IsObsolete = false;
			this.Etymons = new Dictionary<string, string>();
			this.Inflections = new Dictionary<string, string>();
		}

	}
}
