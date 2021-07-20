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
			this.IsObsolete = false;
		}

	}
}
