using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace OxfordV2
{
	public class SavedQueries
	{
	    public string WordID { get; set; }
	    public List<Quote> Quotes { get; set; }

	    public string ExportFileName { get; set; } = "OED-export.txt";
	    
	}
}
