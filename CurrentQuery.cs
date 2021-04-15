using System;

namespace OxfordV2
{
	public enum Modes
	{
		Word,
		Root,
		Lammatize
	}
	class CurrentQuery
	{
		public string WordID { get; set; }
		public string UserEnteredWord { get; set; }
		public Modes QueryMode { get; set; }

		public CurrentQuery() {} 
	}
}
