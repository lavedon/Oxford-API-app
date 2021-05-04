using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Net;


namespace OxfordV2
{
	public static class SavedQueries
	{
	    public static bool Instance { get; set; } = false;
		public static bool DeleteOnExport { get; set; } = true;

	    public static string WordID { get; set; }
	    public static List<Quote> Quotes { get; set; }

	    public static string ExportFileName { get; set; } = "OED-export.xml";

	    static SavedQueries()
	    {
		    Instance=true;
		    Quotes = new();
	    }

		public static void AddQuote(Quote quote) {
			Quotes.Add(quote);
			if (Quotes.Count == 1) {
				Console.WriteLine($"{Quotes.Count} quote saved for export.");
			}
			else {
				Console.WriteLine($"{Quotes.Count} quotes saved for export.");
			}
		}

	    public static void RenderXML() {
			    string xmlFile = Path.Combine(Environment.CurrentDirectory, ExportFileName);
			    File.Delete(xmlFile);
			    FileStream xmlFileStream = File.Create(xmlFile);

		    XmlWriter xml = XmlWriter.Create(xmlFileStream,
		      new XmlWriterSettings { Indent = true });

		    xml.WriteStartDocument();
		    xml.WriteStartElement("SuperMemoCollection");
		    int count = Quotes.Count;
		    xml.WriteElementString("Count", $"{count}");
		    // @TODO Add count number and ID number
		    for (int i = 0; i < Quotes.Count; i++) {
			try {
			    xml.WriteStartElement("SuperMemoElement");
			    int ID = i + 1;
			    xml.WriteElementString("ID", $"{ID}");
			    xml.WriteElementString("Title", $"{Quotes[i].Author} - {Quotes[i].Year}");
			    xml.WriteElementString("Type", "Topic");
			    xml.WriteStartElement("Content");
			    xml.WriteElementString("Question", $"{Quotes[i].Text} --{Quotes[i].Author}, {Quotes[i].Year}");

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
