using System.Collections.Generic;
using System;

namespace OxfordV2
{
	public class Sense 
	{
		public string Definition { get; set; }
		public int Start { get; set; } 
		public bool IsObsolete { get; set; }
		public bool IsMainUsage { get; set; }
		public string SenseID { get; set; }
		public string OedReference { get; set; }
		

		public Sense() {
			this.Definition = "";
			this.Start = 0;
			this.IsObsolete = false;
			this.IsMainUsage = false;
			this.SenseID = "";
			this.OedReference = "";
		}
		public Sense(Sense other) 
		{
			Definition = other.Definition;
			OedReference = other.OedReference;
			SenseID = other.SenseID;
			Start = other.Start;
			IsMainUsage = other.IsMainUsage;
			IsObsolete = other.IsObsolete;
		}

	}
}
