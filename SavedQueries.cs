using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Net;
using System.Linq;


namespace OxfordV2
{
	public static class SavedQueries
	{
	    public static bool Instance { get; set; } = false;
		public static bool DeleteOnExport { get; set; } = true;

	    public static string WordID { get; set; }
	    public static List<Quote> Quotes { get; set; }
	    public static List<Sense> Senses { get; set; }
		public static List<Definition> Definitions { get; set; }

	    public static string ExportFileName { get; set; } = "OED-export.xml";

	    static SavedQueries()
	    {
		    Instance=true;
		    Quotes = new();
		    Senses = new();
	    }

		public static void AddMember(Sense sense) {
			Senses.Add(sense);
			if (Senses.Count == 1) {
				Console.WriteLine($"{Senses.Count} sense saved for export.");
			}
			else {
				Console.WriteLine($"{Senses.Count} senses saved for export.");
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
		    int count = Quotes.Count + Senses.Count;
		    xml.WriteElementString("Count", $"{count}");
		    // @TODO Add count number and ID number
		    if (Quotes.Count > 0) {
		    Console.WriteLine("Exporting Quotes...");
		    for (int i = 0; i < Quotes.Count; i++) {
			try {
			    xml.WriteStartElement("SuperMemoElement");
			    int ID = i + 1;
			    xml.WriteElementString("ID", $"{ID}");
			    xml.WriteElementString("Title", $"{Quotes[i].Author} - {Quotes[i].Year}");
			    xml.WriteElementString("Type", "Topic");
			    xml.WriteStartElement("Content");
			    xml.WriteElementString("Question", $"\"{Quotes[i].Text}\" --{Quotes[i].Author}, {Quotes[i].Year}");

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
		    if (Senses.Count > 0) {
		    Console.WriteLine("Exporting Senses...");
			    string obsoleteText = "";
			    string mainUsageText = "";
		    for (int i = 0; i < Senses.Count; i++) {

			    if (Senses[i].IsObsolete) {
			    	obsoleteText = "This usage is obsolete.";
			    }
			    else {
				obsoleteText = "This usage is NOT obsolete.";
			    }

			    if (Senses[i].IsMainUsage) {
				mainUsageText = "This sense is the main sense for this word.";
			    }
			    else {
				mainUsageText = "This sense is NOT the main sense for this word.";
			    }


			try {
			    xml.WriteStartElement("SuperMemoElement");
			    int ID = i + 1;
			    xml.WriteElementString("ID", $"{ID}");
			    xml.WriteElementString("Title", $"{Senses[i].Definition}");
			    xml.WriteElementString("Type", "Topic");
			    xml.WriteStartElement("Content");
			    xml.WriteElementString("Question", $"Sense: \"{Senses[i].Definition}\" --This sense was first used in the year {Senses[i].Start},  {obsoleteText}, {mainUsageText}, {Senses[i].OedReference}");

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
		    Console.WriteLine("Please press enter:..........");
		    Console.ReadLine();
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
