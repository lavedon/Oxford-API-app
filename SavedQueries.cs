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

		public static bool BlendedExport { get; set; } = false;
		public static bool FirstBlendOption { get; set; } = true;
		public static bool DeleteOnExport { get; set; } = true;

	    public static string WordID { get; set; }

		public static string UserEnteredWord { get; set; }
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
			Surfaces = new();
			SurfacesForExport = new();
			Definitions = new();
			DefinitionsForExport = new();
			Lemmas = new();
			BlendedExport = false;
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
			if (query.QuotesFromWord) {
				for (int i = 0; i < query.WhatToExport.Count; i++) {
					file.WriteLine(query.Definitions[query.WhatToExport[i] - 1].WordID);
				}
			} else if (query.QueryMode == Modes.Quotations) {
                foreach (Quote q in query.Quotes) {
                    file.WriteLine(q.WordID);
                }
            }
			else {
				foreach (Definition d in query.Definitions)
				{
					file.WriteLine(d.WordID);
				}
			}
		}
		public static void SaveSenseId(CurrentQuery query) {
			string senseIDFile = Path.Combine(Environment.CurrentDirectory, "sense-id-all.txt");
			string senseIDFileDistinct = Path.Combine(Environment.CurrentDirectory, "sense-id-distinct.txt");
			File.Delete(senseIDFile);
			File.Delete(senseIDFileDistinct);
			using StreamWriter fileAll = new(senseIDFile);


			List<string> senseIds = new();
			foreach (Sense s in query.Senses)
			{
				fileAll.WriteLine(s.SenseID);
				senseIds.Add(s.SenseID);
			}

			using StreamWriter fileDistinct = new(senseIDFileDistinct);
			IEnumerable<string> uniqueSenseIds = senseIds.Distinct();
			foreach (var id in uniqueSenseIds)
			{
				fileDistinct.WriteLine(id);
			}
		}

		public static CurrentQuery LoadSenseIds(CurrentQuery query) {
			string senseIDFile = Path.Combine(Environment.CurrentDirectory, "sense-id-distinct.txt");
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

		public static void RenderXML(CurrentQuery query) {
            string xmlFile;
            FileStream xmlFileStream;
            XmlWriter xml;
            createXML(out xmlFile, out xmlFileStream, out xml);
			if (query.QueryMode == Modes.QuotesAndSenses) {
			try {
            int totalCount;
			totalCount = 0;
			foreach (var sq in query.SQ_Data)
			{
				foreach (Sens s in sq.senses)
				{
					totalCount = totalCount + s.quotations.Count;
				}
			}

			xml.WriteElementString($"Count", $"{totalCount}");
			int ID;
			ID = 1;
			foreach (var sq in query.SQ_Data)
			{
				int sNum;
				sNum = 1;
				foreach (Sens s in sq.senses)
				{
					xml.WriteStartElement("SuperMemoElement");
					xml.WriteElementString("ID", $"{ID}");
					xml.WriteElementString("Title", $"{s.word_id}-{sNum}");
					xml.WriteElementString("Type", "Item");
					xml.WriteStartElement("Content");

					var questionText = new System.Text.StringBuilder();
					var answerText = new System.Text.StringBuilder();
					// answerText.Append($"{sq.definition} <BR> <BR>");
					// answerText.Append($"Sense #{sNum} <BR>");
					answerText.Append($"{s.lemma} - {s.definition} <BR>");
                    /*
					answerText.Append($"First use: {s.first_use} <BR>");
					answerText.Append($"Part of speech: {s.part_of_speech} <BR>");
					answerText.Append($"{s.daterange.start} - {s.daterange.end} <BR>");
                    */
					if (s.main_current_sense)
					{
						answerText.Append($"This is the main current sense <BR>");
					}
					if (s.daterange.obsolete)
					{
						answerText.Append($"This sense is obsolete <BR>");
					}
					int qNum;
					qNum = 1;
					questionText.Append(s.lemma);
						questionText.Append("<BR> <BR>");
					foreach (Quotation q in s.quotations) 
					{
						// questionText.Append($"Quotation #{qNum}: <BR>");
						questionText.Append($"{q.year.ToString()} ");
						questionText.Append($"{q.source.author}, {q.source.title} ");
						questionText.Append($"&#8220;{q.text.full_text}&#8221; <BR><BR>");
						qNum++;
					}
					
					xml.WriteElementString("Question", questionText.ToString().Replace(sq.lemma, $"<FONT color=#ff0000><STRONG><EM>{sq.lemma}</EM></STRONG></FONT>"));
					xml.WriteElementString("Answer", answerText.ToString());
					string encoded = WebUtility.HtmlEncode("<H5 dir=ltr align=left><Font size=\"1\" style=\"color: transparent\"> SuperMemo Reference:</font><br><FONT class=reference>Title:\"My Test Quote\" <br>Source: Oxford English Dictionary");
					xml.WriteElementString("SuperMemoReference", encoded);

					xml.WriteEndElement();
					xml.WriteEndElement();

					ID++;
					sNum++;
				}
			}
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
            try
            {
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

	    public static void RenderXML()
        {
            /*
				if (FirstBlendOption && BlendedExport) {
					FirstBlendOption = !FirstBlendOption;
					return;
				}
			*/
            string xmlFile;
            FileStream xmlFileStream;
            XmlWriter xml;
            createXML(out xmlFile, out xmlFileStream, out xml);
            int count;
            if (BlendedExport)
            {
                count = DefinitionsForExport.Count;
            }
            else
            {
                count = Quotes.Count + SensesForExport.Count + Lemmas.Count + DefinitionsForExport.Count + SurfacesForExport.Count;
            }

            xml.WriteElementString("Count", $"{count}");
            // @TODO Add count number and ID number

            // Export each category of saved stuff - one by one
            if (BlendedExport)
            {
                int ID = 1;
                foreach (Definition d in DefinitionsForExport)
                {
                    List<Quote> quotesForDefinition = new();
                    foreach (Quote q in QuotesForExport)
                    {
                        if (q.WordID == d.WordID)
                        {
                            quotesForDefinition.Add(q);
                        }
                    }
                    // Render the XML
                    try
                    {
                        xml.WriteStartElement("SuperMemoElement");
                        xml.WriteElementString("ID", $"{ID}");
                        xml.WriteElementString("Title", $"{d.WordID}");
                        xml.WriteElementString("Type", "Item");
                        xml.WriteStartElement("Content");
                        // xml.WriteElementString("Question", $"{UserEnteredWord}<BR><BR>{d.WordDefinition}");
                        var questionText = new System.Text.StringBuilder();
                        foreach (Quote quote in quotesForDefinition)
                        {
                            string quoteText = $"<BR> <BR> \"{quote.Text}\" --{quote.Author}, {quote.Year}";
                            questionText.Append(quoteText);
                            questionText.Append("<BR><BR>");
                        }

                        xml.WriteElementString("Question", $"{UserEnteredWord}<BR><BR>{questionText}");
                        xml.WriteElementString("Answer", $"{d.FormattedVerboseOutput}");

                        string encoded = WebUtility.HtmlEncode("<H5 dir=ltr align=left><Font size=\"1\" style=\"color: transparent\"> SuperMemo Reference:</font><br><FONT class=reference>Title:\"My Test Quote\" <br>Source: Oxford English Dictionary");
                        xml.WriteElementString("SuperMemoReference", encoded);

                        xml.WriteEndElement();
                        xml.WriteEndElement();

                        ID++;
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
            if (QuotesForExport.Count > 0 && !BlendedExport)
            {
                Console.WriteLine("Exporting Quotes...");
                for (int i = 0; i < QuotesForExport.Count; i++)
                {
                    try
                    {
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

            if (DefinitionsForExport.Count > 0 && !BlendedExport)
            {
                Console.WriteLine("Exporting Definitions..");
                for (int i = 0; i < DefinitionsForExport.Count; i++)
                {
                    int ID = i + 1;
                    try
                    {
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
            if (SensesForExport.Count > 0 && !BlendedExport)
            {
                Console.WriteLine("Exporting Senses...");
                string obsoleteText = "";
                string mainUsageText = "";
                for (int i = 0; i < SensesForExport.Count; i++)
                {

                    if (SensesForExport[i].IsObsolete)
                    {
                        obsoleteText = "This usage is obsolete.";
                    }
                    else
                    {
                        // obsoleteText = "This usage is NOT obsolete.";
                        obsoleteText = "";
                    }

                    if (SensesForExport[i].IsMainUsage)
                    {
                        mainUsageText = "This sense is the main sense for this word.";
                    }
                    else
                    {
                        // mainUsageText = "This sense is NOT the main sense for this word.";
                        mainUsageText = "";
                    }



                    try
                    {
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
            if (Lemmas.Count > 0 && !BlendedExport)
           {
                Console.WriteLine("Exporting Lemmas...");
                if (Lemmas[0].ZippedLemmas.Count > 0)
                {
                    try
                    {
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
            if (SurfacesForExport.Count > 0 && !BlendedExport)
            {
                Console.WriteLine("Exporting Surface Forms...");
                for (int i = 0; i < SurfacesForExport.Count; i++)
                {
                    try
                    {
                        xml.WriteStartElement("SuperMemoElement");
                        int ID = i + 1;
                        xml.WriteElementString("ID", $"{ID}");
                        xml.WriteElementString("Title", $"{SurfacesForExport[i].word_id}");
                        xml.WriteElementString("Type", "Topic");
                        xml.WriteStartElement("Content");
                        xml.WriteStartElement("Question");
                        xml.WriteString($"Surfaceform searched: \"{SurfacesForExport[i].form}\", Lemma: {SurfacesForExport[i].lemma}, Normalized: {SurfacesForExport[i].normalized} <BR>");
                        xml.WriteString($"Part of Speech: {SurfacesForExport[i].part_of_speech} <BR>");
                        if (SurfacesForExport[i].region != null)
                        {
                            xml.WriteString($"Region: {SurfacesForExport[i].region} <BR>");
                        }
                        string yesNo = SurfacesForExport[i].standard_us_form ? "Yes" : "No";
                        string usaForm = "Standard USA Form?: " + yesNo;
                        xml.WriteString(usaForm);
                        xml.WriteString("<BR>");
                        string britishYesNo = SurfacesForExport[i].standard_british_form ? "Yes" : "No";
                        string britishForm = "Standard USA Form?: " + yesNo;
                        xml.WriteString(britishForm);
                        xml.WriteString("<BR>");
                        xml.WriteString($"This form is listed as in use for these years: {SurfacesForExport[i].daterange.start} - {SurfacesForExport[i].daterange.end} <BR>");
                        if (SurfacesForExport[i].daterange.obsolete)
                        {
                            xml.WriteString("This form is listed as obsolete");
                        }
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
            try
            {
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

        private static void createXML(out string xmlFile, out FileStream xmlFileStream, out XmlWriter xml)
        {
            xmlFile = Path.Combine(Environment.CurrentDirectory, ExportFileName);
            File.Delete(xmlFile);
            xmlFileStream = File.Create(xmlFile);
            xml = XmlWriter.Create(xmlFileStream,
              new XmlWriterSettings { Indent = true });
            xml.WriteStartDocument();
            xml.WriteStartElement("SuperMemoCollection");
        }
    }
}