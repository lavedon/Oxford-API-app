using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Net;
using System.Linq;


namespace oed
{
	public static class SavedQueries
	{
	    public static bool Instance { get; set; } = false;
		public static bool DeleteOnExport { get; set; } = true;

	    public static string WordID { get; set; }
	    public static List<Quote> Quotes { get; set; }
	    public static List<Quote> QuotesForExport { get; set; }
	    public static List<Sense> Senses { get; set; }
	    public static List<Sense> SensesForExport { get; set; }

		public static List<Datum> Surfaces { get; set; }
		public static List<Datum> SurfacesForExport { get; set; }
		public static List<Definition> Definitions { get; set; }
		public static List<Definition> DefinitionsForExport { get; set; }

		public static List<Lemmas> Lemmas { get; set; }

	    public static string ExportFileName { get; set; } = "OED-export.xml";

	    static SavedQueries()
	    {
		    Instance=true;
		    Quotes = new();
            QuotesForExport = new();
		    Senses = new();
            SensesForExport = new();
			Definitions = new();
			DefinitionsForExport = new();
			Lemmas = new();
	    }


		public static void AddMember(Sense sense) {
			Senses.Add(sense);
			if (Senses.Count == 1) {
				Trace.WriteLine($"{Senses.Count} sense saved for export.");
			}
			else {
				Trace.WriteLine($"{Senses.Count} senses saved for export.");
			}
		}

		public static void AddMember(Quote quote) {
			Quotes.Add(quote);
			if (Quotes.Count == 1) {
				Console.WriteLine($"{Quotes.Count} quote saved for export.");
			}
			else {
				Console.WriteLine($"{Quotes.Count} quotes saved for export.");
			}
		}

		public static void AddMember(Lemmas lemma)
		{
			Lemmas.Add(lemma!);
			Console.WriteLine("Lemmas saved for export.");
			Console.WriteLine("Creating XML file of all saved material...");
			RenderXML();
		}

		public static void SaveWordId(CurrentQuery query) {
			string wordIDFile = Path.Combine(Environment.CurrentDirectory, "word-id.txt");
			File.Delete(wordIDFile);
			using StreamWriter file = new(wordIDFile);

			foreach (Definition d in query.Definitions)
			{
				file.WriteLine(d.WordID);
			}
		}
		public static void SaveSenseId(CurrentQuery query) {
			string senseIDFile = Path.Combine(Environment.CurrentDirectory, "sense-id.txt");
			File.Delete(senseIDFile);
			using StreamWriter file = new(senseIDFile);

			List<string> senseIds = new();
			foreach (Sense s in query.Senses)
			{
				senseIds.Add(s.SenseID);
			}

			IEnumerable<string> uniqueSenseIds = senseIds.Distinct();
			foreach (var id in uniqueSenseIds)
			{
				file.WriteLine(id);
			}
		}

		public static CurrentQuery LoadSenseIds(CurrentQuery query) {
			string senseIDFile = Path.Combine(Environment.CurrentDirectory, "sense-id.txt");
			string[] lines = System.IO.File.ReadAllLines(senseIDFile);
			foreach (string line in lines) {
				var blankSenseWithId = new Sense();
				blankSenseWithId.SenseID = line;
				query.Senses.Add(blankSenseWithId);
			}
			query.HasLookedUpWord = true;
			return query;
		}
		public static CurrentQuery LoadWordIds(CurrentQuery query) {
			string wordIDFile = Path.Combine(Environment.CurrentDirectory, "word-id.txt");
			string[] lines = System.IO.File.ReadAllLines(wordIDFile);
			foreach (string line in lines) {
				var blankDefinitionWithId = new Definition();
				blankDefinitionWithId.WordID = line;
				query.Definitions.Add(blankDefinitionWithId);
			}
			query.HasLookedUpWord = true;
			return query;
		}

	    public static void RenderXML() {
			    string xmlFile = Path.Combine(Environment.CurrentDirectory, ExportFileName);
			    File.Delete(xmlFile);
			    FileStream xmlFileStream = File.Create(xmlFile);

		    XmlWriter xml = XmlWriter.Create(xmlFileStream,
		      new XmlWriterSettings { Indent = true });

		    xml.WriteStartDocument();
		    xml.WriteStartElement("SuperMemoCollection");
		    int count = Quotes.Count + SensesForExport.Count + Lemmas.Count + DefinitionsForExport.Count + SurfacesForExport.Count;
		    xml.WriteElementString("Count", $"{count}");
		    // @TODO Add count number and ID number

			// Export each category of saved stuff - one by one
		    if (QuotesForExport.Count > 0) {
		    Console.WriteLine("Exporting Quotes...");
		    for (int i = 0; i < QuotesForExport.Count; i++) {
			try {
			    xml.WriteStartElement("SuperMemoElement");
			    int ID = i + 1;
			    xml.WriteElementString("ID", $"{ID}");
			    xml.WriteElementString("Title", $"{QuotesForExport[i].Author} - {QuotesForExport[i].Year}");
			    xml.WriteElementString("Type", "Topic");
			    xml.WriteStartElement("Content");
			    xml.WriteElementString("Question", $"\"{QuotesForExport[i].Text}\" --{QuotesForExport[i].Author}, {QuotesForExport[i].Year}");

			    string encoded = WebUtility.HtmlEncode("<H5 dir=ltr align=left><Font size=\"1\" style=\"color: transparent\"> SuperMemo Reference:</font><br><FONT class=reference>Title:\"My Test Quote\" <br>Source: Oxford English Dictionary");
			    xml.WriteElementString("SuperMemoReference", encoded);

				xml.WriteEndElement();
				xml.WriteEndElement();

				}
				catch (AggregateException ae)
				{
					var ex = ae.Flatten().InnerExceptions;
					Console.WriteLine("Error writing XML document:");
					foreach (var exception in ex)
					{
						Console.WriteLine($"{ex.ToString()}");
					}
				}
		    }
		    }

			if (DefinitionsForExport.Count > 0)
			{
				Console.WriteLine("Exporting Definitions..");
				for (int i = 0; i < DefinitionsForExport.Count; i++) {
			    int ID = i + 1;
			try {
			    xml.WriteStartElement("SuperMemoElement");
			    xml.WriteElementString("ID", $"{ID}");
			    xml.WriteElementString("Title", $"{DefinitionsForExport[i].WordID}");
			    xml.WriteElementString("Type", "Topic");
			    xml.WriteStartElement("Content");
			    xml.WriteElementString("Question", $"{DefinitionsForExport[i].FormattedVerboseOutput}");
			    string encoded = WebUtility.HtmlEncode("<H5 dir=ltr align=left><Font size=\"1\" style=\"color: transparent\"> SuperMemo Reference:</font><br><FONT class=reference>Title:\"My Test Quote\" <br>Source: Oxford English Dictionary");
			    xml.WriteElementString("SuperMemoReference", encoded);

				xml.WriteEndElement();
				xml.WriteEndElement();

				}
				catch (AggregateException ae)
				{
					var ex = ae.Flatten().InnerExceptions;
					Console.WriteLine("Error writing XML document:");
					foreach (var exception in ex)
					{
						Console.WriteLine($"{ex.ToString()}");
					}
				}
		    }
			}
		    if (SensesForExport.Count > 0) {
		    Console.WriteLine("Exporting Senses...");
			    string obsoleteText = "";
			    string mainUsageText = "";
		    for (int i = 0; i < SensesForExport.Count; i++) {

			    if (SensesForExport[i].IsObsolete) {
			    	obsoleteText = "This usage is obsolete.";
			    }
			    else {
				// obsoleteText = "This usage is NOT obsolete.";
				obsoleteText = "";
			    }

			    if (SensesForExport[i].IsMainUsage) {
				mainUsageText = "This sense is the main sense for this word.";
			    }
			    else {
				// mainUsageText = "This sense is NOT the main sense for this word.";
				mainUsageText = "";
			    }

			

			try {
			    xml.WriteStartElement("SuperMemoElement");
			    int ID = i + 1;
			    xml.WriteElementString("ID", $"{ID}");
			    xml.WriteElementString("Title", $"{SensesForExport[i].OedReference}");
			    xml.WriteElementString("Type", "Topic");
			    xml.WriteStartElement("Content");
			    xml.WriteElementString("Question", $"Sense: \"{SensesForExport[i].Definition}\" --This sense was first used in the year {SensesForExport[i].Start},  {obsoleteText}, {mainUsageText}, {SensesForExport[i].OedReference}");

			    string encoded = WebUtility.HtmlEncode("<H5 dir=ltr align=left><Font size=\"1\" style=\"color: transparent\"> SuperMemo Reference:</font><br><FONT class=reference>Title:\"My Test Quote\" <br>Source: Oxford English Dictionary");
			    xml.WriteElementString("SuperMemoReference", encoded);

				xml.WriteEndElement();
				xml.WriteEndElement();

				}
				catch (AggregateException ae)
				{
					var ex = ae.Flatten().InnerExceptions;
					Console.WriteLine("Error writing XML document:");
					foreach (var exception in ex)
					{
						Console.WriteLine($"{ex.ToString()}");
					}
				}
		    }
		    }
			if (Lemmas.Count > 0) {
				Console.WriteLine("Exporting Lemmas...");
				if (Lemmas[0].ZippedLemmas.Count > 0)
				{
					try {
						xml.WriteStartElement("SuperMemoElement");
						xml.WriteElementString("ID", $"{01}");
						xml.WriteElementString("Title", $"Exported Lemmas");
						xml.WriteElementString("Type", "Topic");
						xml.WriteStartElement("Content");
						xml.WriteStartElement("Question");
						xml.WriteString("<table>");
						xml.WriteString("<tr><th>Token:</th><th>Lemma:</th>");
						foreach (KeyValuePair<string, string> item in Lemmas[0].ZippedLemmas)
						{
							xml.WriteString($"<tr><td style='text-align:center'>{item.Key}</td><td style='text-align:center'>{item.Value}</td></tr>");
						}
						xml.WriteString("</table>");
						xml.WriteEndElement();

						string encoded = WebUtility.HtmlEncode("<H5 dir=ltr align=left><Font size=\"1\" style=\"color: transparent\"> SuperMemo Reference:</font><br><FONT class=reference>Title:\"My Test Quote\" <br>Source: Oxford English Dictionary");
						xml.WriteElementString("SuperMemoReference", encoded);

						xml.WriteEndElement();
						xml.WriteEndElement();

						}
						catch (AggregateException ae)
						{
							var ex = ae.Flatten().InnerExceptions;
							Console.WriteLine("Error writing XML document:");
							foreach (var exception in ex)
							{
								Console.WriteLine($"{ex.ToString()}");
							}
						}

				}
			}
			if (SurfacesForExport.Count > 0) {
				Console.WriteLine("Exporting Surface Forms...");
			}
		try {
	  		xml.WriteEndElement();
		    xml.WriteEndDocument();
		    xml.Flush();

		    xml.Close();
		    xmlFileStream.Close();

		    Console.WriteLine("The XML file is saved here:");
		    Console.WriteLine(xmlFile);

		    Trace.WriteLine("The contents of the exported XML file are:");
		    Trace.WriteLine(File.ReadAllText(xmlFile));
		}
			catch (AggregateException ae)
			{
				var ex = ae.Flatten().InnerExceptions;
				Console.WriteLine("Error exporting XML document:");
				foreach (var exception in ex)
				{
					Console.WriteLine($"{ex.ToString()}");
				}
			}
		}
	}
}