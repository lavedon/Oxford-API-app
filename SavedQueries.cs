using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Net;
using System.Linq;
using System.Text;


namespace oed
{
	public static class SavedQueries
	{
        private static bool _appendXML = false;
	    public static bool Instance { get; set; } = false;

        public static IPAOptions IPA { get; set; } = new IPAOptions();

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
            query.Definitions.Clear();
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
            totalCount = totalCount + query.SQID_Data.Count;
            foreach (var sq in query.SQID_Data)
            {
                totalCount = totalCount + sq.quotations.Count;
            }

			xml.WriteElementString($"Count", $"{totalCount}");
			int sqID;
			sqID = 1;
            int sqsSenseNum;
            sqsSenseNum = 1;
			foreach (var sqs in query.SQID_Data)
            {
                    xml.WriteStartElement("SuperMemoElement");
                    xml.WriteElementString("ID", $"{sqID}");
                    xml.WriteElementString("Title", $"{sqs.id}");
                    xml.WriteElementString("Type", "Item");
                    xml.WriteStartElement("Content");
                    var sqsQuestionText = new System.Text.StringBuilder();
                    var sqsAnswerText = new System.Text.StringBuilder();
                    // Add sense #?
                    sqsAnswerText.Append($"{sqs.lemma} - {sqs.definition} <BR>");
                    if (sqs.main_current_sense)
                    {
                        sqsAnswerText.Append($"This is the main current sense <BR>");
                    }
                    if (sqs.daterange.obsolete)
                    {
                        sqsAnswerText.Append($"This sense is obsolete <BR>");
                    }

                int sqsQNum;
                sqsQNum = 1;
                sqsQuestionText.Append($"{sqs.lemma} <BR> <BR>");
                foreach (Quotation q in sqs.quotations)
                {
                    sqsQuestionText = writeXMLQuote(sqsQuestionText, sqsQNum, q);
                }
                    xml.WriteElementString("Question", sqsQuestionText.ToString().Replace(sqs.lemma, $"<STRONG>{sqs.lemma}</STRONG>"));
					xml.WriteElementString("Answer", sqsAnswerText.ToString());
					string encoded = WebUtility.HtmlEncode("<H5 dir=ltr align=left><Font size=\"1\" style=\"color: transparent\"> SuperMemo Reference:</font><br><FONT class=reference>Title:\"My Test Quote\" <br>Source: Oxford English Dictionary");
					xml.WriteElementString("SuperMemoReference", encoded);

					xml.WriteEndElement();
					xml.WriteEndElement();
                    sqID++;
                    sqsSenseNum++;

            }
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
					xml.WriteElementString("Title", $"{s.id}-{sNum}");
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
                                questionText = writeXMLQuote(questionText, qNum, q);
                            }

                            // Removed <FONT color="#ff0000"> and </FONT> from the text
                            xml.WriteElementString("Question", questionText.ToString().Replace(sq.lemma, $"<STRONG>{sq.lemma}</STRONG>"));
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

                Trace.WriteLine("A XML file is being saved here:");
                Trace.WriteLine(xmlFile);

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
            if (_appendXML)
            {
               AppendXML.Append(xmlFile);
               return; 
            }

            static StringBuilder writeXMLQuote(StringBuilder questionText, int qNum, Quotation q)
            {
                if (string.IsNullOrWhiteSpace(q.source.author))
                {
                    q.source.author = "Unknown";
                }
                questionText.Append($"{q.year.ToString()} ");
                questionText.Append($"{q.source.author}, {q.source.title} ");
                questionText.Append($"&#8220;{q.text.full_text}&#8221; <BR><BR>");
                qNum++;
                return questionText;
            }
        }
        public static void RenderTextFile(CurrentQuery query)
        {
            string txtFile;
            StreamWriter sw;
            createTXT(out txtFile, out sw);
            if (query.IPAMode != IPAOptions.None)
            {
                // Similar to definition export
                Console.WriteLine("Exporting pronunciation cards to text file...");
                for (int i = 0; i < DefinitionsForExport.Count; i++)
                {
                    switch (query.IPAMode) 
                    {
                        case IPAOptions.British:
                            sw.WriteLine("IPA British: " + query.Definitions[i].BritishIPA);
                            break;
                        case IPAOptions.USA:
                            sw.WriteLine("IPA USA: " + query.Definitions[i].USIPA);
                            break;
                        case IPAOptions.Both:
                            sw.WriteLine("IPA USA: " + query.Definitions[i].USIPA);
                            sw.WriteLine("IPA British: " + query.Definitions[i].BritishIPA);
                            break;
                        default:
                            break;
                    }
                    try
                    {
                        /*
                        if (!string.IsNullOrWhiteSpace(DefinitionsForExport[i].BritishIPA))
                        {
                            sw.WriteLine($"{DefinitionsForExport[i].BritishIPA}");
                        }
                        if (!string.IsNullOrWhiteSpace(DefinitionsForExport[i].USIPA))
                        {
                            sw.WriteLine($"{DefinitionsForExport[i].USIPA}");
                        }
                        */
                            sw.WriteLine(DefinitionsForExport[i].WordID);
                            sw.WriteLine(DefinitionsForExport[i].FormattedVerboseOutput);
                            sw.WriteLine();
                    } catch (AggregateException ae) 
                    {
                        var ex = ae.Flatten().InnerExceptions;
                        Console.WriteLine("Error writing Pronunciation cards to Text file:");
                        foreach (var exception in ex) 
                        {
                            Console.WriteLine($"{ex.ToString()}");
                        }
                    }
                }
            }

            if (BlendedExport)
            {
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
                    try
                    {
                        var questionText = new System.Text.StringBuilder();
                        sw.WriteLine(UserEnteredWord);
                        foreach (Quote quote in quotesForDefinition)
                        {
                            if (string.IsNullOrWhiteSpace(quote.Author))
                            {
                                quote.Author = "Unknown";
                            }
                            sw.WriteLine($"\"{quote.Text}\"");
                            sw.WriteLine($"--{quote.Author}, {quote.Year}");
                            sw.WriteLine();
                            /*
                            string quoteText = $"\"{quote.Text}\" --{quote.Author}, {quote.Year}";
                            sw.WriteLine(quoteText);
                            sw.WriteLine();
                            */
                        }

                        sw.WriteLine(questionText);
                        sw.WriteLine(d.FormattedVerboseOutput);

                    }
                    catch (AggregateException ae)
                    {
                        var ex = ae.Flatten().InnerExceptions;
                        Console.WriteLine("Error writing to Text file");
                        foreach (var exception in ex)
                        {
                            Console.WriteLine($"{ex.ToString()}");
                        }
                    }
                }
            }

			if (query.QueryMode == Modes.QuotesAndSenses) {
			try {
            // sq s1,5 type query from sense/{id}/ end point 
                int senseNum;
                senseNum = 1;
            foreach (var qs in query.SQID_Data) {
                sw.WriteLine($"Sense #{senseNum}");
                sw.WriteLine($"{qs.lemma} - {qs.definition}");
                if (qs.main_current_sense) 
                {
                    sw.WriteLine($"This is the main current sense");
                }
                if (qs.daterange.obsolete)
                {
                    sw.WriteLine($"This sense is obsolete");
                }
                sw.WriteLine();
                int qNum;
                qNum = 1;
                foreach (var q in qs.quotations) {
                    if (string.IsNullOrWhiteSpace(q.source.author))
                    {
                        q.source.author = "Unknown";
                    }
                    sw.WriteLine($"Quote #{qNum}");
                    sw.WriteLine($"{q.text.full_text}");
                    sw.WriteLine($"{q.source.author}, {q.year.ToString()}, {q.source.title} ");
                    sw.WriteLine();
                    qNum++;
                }
            senseNum++;
            }
            int sNum;
            sNum = 1;
			foreach (var sq in query.SQ_Data)
			{
				foreach (Sens s in sq.senses)
				{
					// answerText.Append($"{sq.definition} <BR> <BR>");
					// answerText.Append($"Sense #{sNum} <BR>");
                    sw.WriteLine($"Sense #{sNum}");
					sw.WriteLine($"{s.lemma} - {s.definition}");
                    /*
					answerText.Append($"First use: {s.first_use} <BR>");
					answerText.Append($"Part of speech: {s.part_of_speech} <BR>");
					answerText.Append($"{s.daterange.start} - {s.daterange.end} <BR>");
                    */
					if (s.main_current_sense)
					{
						sw.WriteLine($"This is the main current sense");
					}
					if (s.daterange.obsolete)
					{
						sw.WriteLine($"This sense is obsolete");
					}
					int qNum;
					qNum = 1;
                    sw.WriteLine();
					sw.WriteLine(s.lemma);

					foreach (Quotation q in s.quotations) 
					{
                        /*
                            if (string.IsNullOrWhiteSpace(quote.Author))
                            {
                                quote.Author = "Unknown";
                            }
                            sw.WriteLine($"\"{quote.Text}\"");
                            sw.WriteLine($"--{quote.Author}, {quote.Year}");
                            sw.WriteLine();


                        */
						// questionText.Append($"Quotation #{qNum}: <BR>");
                        if (string.IsNullOrWhiteSpace(q.source.author))
                        {
                            q.source.author = "Unknown";
                        }
                        sw.WriteLine($"Quote #{qNum}");
						sw.WriteLine($"{q.text.full_text}");
						sw.WriteLine($"{q.source.author}, {q.year.ToString()}, {q.source.title} ");
                        sw.WriteLine();
						qNum++;
					}
    
                    // Removed <FONT color="#ff0000"> and </FONT> from the text
                    sw.WriteLine();
                    sw.WriteLine();
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

                sw.Close();

                Trace.WriteLine("A Text file is being saved here:");
                Trace.WriteLine(txtFile);

            }
            catch (AggregateException ae)
            {
                var ex = ae.Flatten().InnerExceptions;
                Console.WriteLine("Error exporting text file:");
                foreach (var exception in ex)
                {
                    Console.WriteLine($"{ex.ToString()}");
                }
            }
			}
            if (_appendXML)
            {
               AppendXML.AppendTxtFile(txtFile);
               return; 
            }
        }

        public static void RenderTextFile()
        {
            string txtFile;
            StreamWriter sw;
            createTXT(out txtFile, out sw);
            if (QuotesForExport.Count > 0 && !BlendedExport)
            {
                Console.WriteLine("Exporting Quotes to text file...");
                for (int i = 0; i < QuotesForExport.Count; i++)
                {
                    try
                    {
                     // sw.WriteLine($"{QuotesForExport[i].Author} - {QuotesForExport[i].Year}");
                        if (string.IsNullOrWhiteSpace(QuotesForExport[i].Author))
                        {
                            QuotesForExport[i].Author = "Unknown";
                        }
                        sw.WriteLine($"\"{QuotesForExport[i].Text}\"");
                        sw.WriteLine($"--{QuotesForExport[i].Author}, {QuotesForExport[i].Year}");
                        sw.WriteLine();
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
                Console.WriteLine("Exporting Definitions to text file..");
                for (int i = 0; i < DefinitionsForExport.Count; i++)
                {
                    try
                    {
                        sw.WriteLine(DefinitionsForExport[i].WordID);
                        sw.WriteLine(DefinitionsForExport[i].FormattedVerboseOutput);
                        if (SavedQueries.IPA != IPAOptions.None)
                        {
                            switch (SavedQueries.IPA) 
                            {
                                case IPAOptions.British:
                                    sw.WriteLine("IPA British: " + DefinitionsForExport[i].BritishIPA);
                                    break;
                                case IPAOptions.USA:
                                    sw.WriteLine("IPA USA: " + DefinitionsForExport[i].USIPA);
                                    break;
                                case IPAOptions.Both:
                                    sw.WriteLine("IPA USA: " + DefinitionsForExport[i].USIPA);
                                    sw.WriteLine("IPA British: " + DefinitionsForExport[i].BritishIPA);
                                    break;
                                default:
                                    break;
                            }
                        }
                        sw.WriteLine();
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
                } // end for loop of Definitions
            } // end if Definitions and not BlendedExport
            if (SensesForExport.Count > 0 && !BlendedExport)
            {
                Console.WriteLine("Exporting Senses to a text file...");
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
                        sw.WriteLine(SensesForExport[i].OedReference);
                        sw.WriteLine($"Sense: \"{SensesForExport[i].Definition}\" --This sense was first used in the year {SensesForExport[i].Start},  {obsoleteText}, {mainUsageText}, {SensesForExport[i].OedReference}");
                        sw.WriteLine();
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
                        sw.WriteLine("Token:            Lemma:");
                        foreach (KeyValuePair<string, string> item in Lemmas[0].ZippedLemmas)
                        {
                            sw.WriteLine($"{item.Key}:          {item.Value}");
                        }
                        sw.WriteLine();
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
                        sw.WriteLine(SurfacesForExport[i].word_id);
                        sw.WriteLine($"Surfaceform searched: \"{SurfacesForExport[i].form}\", Lemma: {SurfacesForExport[i].lemma}, Normalized: {SurfacesForExport[i].normalized}");
                        sw.WriteLine($"Part of Speech: {SurfacesForExport[i].part_of_speech}");
                        if (SurfacesForExport[i].region != null)
                        {
                            sw.WriteLine($"Region: {SurfacesForExport[i].region}");
                        }
                        string yesNo = SurfacesForExport[i].standard_us_form ? "Yes" : "No";
                        string usaForm = "Standard USA Form?: " + yesNo;
                        sw.WriteLine(usaForm);
                        string britishYesNo = SurfacesForExport[i].standard_british_form ? "Yes" : "No";
                        string britishForm = "Standard USA Form?: " + yesNo;
                        sw.WriteLine(britishForm);
                        sw.WriteLine($"This form is listed as in use for these years: {SurfacesForExport[i].daterange.start} - {SurfacesForExport[i].daterange.end}");
                        if (SurfacesForExport[i].daterange.obsolete)
                        {
                            sw.WriteLine("This form is listed as obsolete");
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

                }
            }
            try
            {

                sw.Close();

                Trace.WriteLine("A TXT file is being saved here:");
                Trace.WriteLine(txtFile);
            }
            catch (AggregateException ae)
            {
                var ex = ae.Flatten().InnerExceptions;
                Console.WriteLine("Error exporting text file:");
                foreach (var exception in ex)
                {
                    Console.WriteLine($"{ex.ToString()}");
                }
            }
            if (_appendXML)
            {
               AppendXML.AppendTxtFile(txtFile);
               return; 
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
            } else if (IPA != IPAOptions.None)
            {
                count = Quotes.Count + SensesForExport.Count + Lemmas.Count + (DefinitionsForExport.Count * 2) + SurfacesForExport.Count;
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
                Console.WriteLine("Exporting Quotes to XML file...");
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
                int ID;
                StringBuilder IPAString = new(); 
                Console.WriteLine("Exporting Definitions for SuperMemo..");
                ID = 1;
                for (int i = 0; i < DefinitionsForExport.Count; i++)
                {
                    try
                    {
                        xml.WriteStartElement("SuperMemoElement");
                        xml.WriteElementString("ID", $"{ID}");
                        // Make IPA Cards if using 'p' mode
                        // Make the inverse card also with ID + 1
                        if (SavedQueries.IPA != IPAOptions.None)
                        {
                            xml.WriteElementString("Title", $"{DefinitionsForExport[i].WordID}");
                            xml.WriteElementString("Type", "Item");
                            xml.WriteStartElement("Content");
                            xml.WriteElementString("Question", $"{UserEnteredWord}: {DefinitionsForExport[i].WordDefinition}");
                            switch (SavedQueries.IPA)
                            {
                                case IPAOptions.Both:
                                IPAString.Append("US IPA: " + DefinitionsForExport[i].USIPA + "<BR>" + "British IPA: " + DefinitionsForExport[i].BritishIPA);
                                break;
                                case IPAOptions.British:
                                IPAString.Append("British IPA: " + DefinitionsForExport[i].BritishIPA);
                                break;
                                case IPAOptions.USA:
                                IPAString.Append("American IPA: " + DefinitionsForExport[i].USIPA);
                                break;
                            }
                            xml.WriteElementString("Answer", IPAString.ToString());
                        } else {
                            xml.WriteElementString("Title", $"{DefinitionsForExport[i].WordID}");
                            xml.WriteElementString("Type", "Topic");
                            xml.WriteStartElement("Content");
                            xml.WriteElementString("Question", $"{DefinitionsForExport[i].FormattedVerboseOutput}");
                        }
                        string encoded = WebUtility.HtmlEncode("<H5 dir=ltr align=left><Font size=\"1\" style=\"color: transparent\"> SuperMemo Reference:</font><br><FONT class=reference>Title:\"My Test Quote\" <br>Source: Oxford English Dictionary");
                        xml.WriteElementString("SuperMemoReference", encoded);

                        xml.WriteEndElement(); // Content
                        xml.WriteEndElement(); // SuperMemoElement
                        // Write the inverse item
                        if (SavedQueries.IPA != IPAOptions.None)
                        {
                        (xml, ID) = createInverseItem(xml, IPAString, DefinitionsForExport[i], ID, encoded);
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
                    ID++;
                } // end for loop of Definitions
            } // end if 
            if (SensesForExport.Count > 0 && !BlendedExport)
            {
                Console.WriteLine("Exporting Senses for SuperMemo...");
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
                Console.WriteLine("Exporting Surface Forms for SuperMemo...");
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

                Trace.WriteLine("An XML file is being saved here:");
                Trace.WriteLine(xmlFile);

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
            if (_appendXML)
            {
               AppendXML.Append(xmlFile);
            }
            RenderTextFile();
        }

        private static void createTXT(out string txtFile, out StreamWriter sw)
        {
            txtFile = Path.Combine(Environment.CurrentDirectory, "OED-export.txt");
            if (File.Exists(txtFile))
            {
                Trace.WriteLine("Creating a new TXT file. This file will then get appended.");
                txtFile = Path.Combine(Path.GetTempPath(), "OED-export-append.txt");
                _appendXML = true;
            } else {
                _appendXML = false;
            }
            sw = File.CreateText(txtFile);

            


        }

        private static void createXML(out string xmlFile, out FileStream xmlFileStream, out XmlWriter xml)
        {
            xmlFile = Path.Combine(Environment.CurrentDirectory, ExportFileName);
	    // @TODO Change - if File exists read the Count # '
	    // Then modify it.
            if (File.Exists(xmlFile))
            {
                Trace.WriteLine("Creating a new XML file.  This file will then get appended.");
                xmlFile = Path.Combine(Environment.CurrentDirectory, "append-me.xml");
                _appendXML = true;
            } else {
                _appendXML = false;
            }
            // @TODO delete the file only if the user specified to do so.
            // File.Delete(xmlFile);
            xmlFileStream = File.Create(xmlFile);
            xml = XmlWriter.Create(xmlFileStream,
              new XmlWriterSettings { Indent = true });
            xml.WriteStartDocument();
            xml.WriteStartElement("SuperMemoCollection");
        }
        private static (XmlWriter, int) createInverseItem(XmlWriter xml, StringBuilder IPAString, Definition definitionToExport, int ID,
            string superMemoRefernce)
        {
                ID += 1;
                Trace.WriteLine($"Writing the reverse IPA item for {definitionToExport.WordID}");
                xml.WriteStartElement("SuperMemoElement");
                xml.WriteElementString("ID", $"{ID}");
                xml.WriteElementString("Type", "Item");
                xml.WriteElementString("Title", $"{definitionToExport.WordID}-Inverse");
                xml.WriteStartElement("Content");
                xml.WriteElementString("Question", IPAString.ToString());
                string answerText = UserEnteredWord + ": " + definitionToExport.WordDefinition;
                xml.WriteElementString("Answer", answerText);
                xml.WriteElementString("SuperMemoReference", superMemoRefernce);
                xml.WriteEndElement(); // Content
                xml.WriteEndElement(); // SuperMemoElement
                IPAString.Clear();
            return (xml, ID);
        }

            /*
            } catch (AggregateException ae)
            {
                Console.WriteLine("Error creating XML file.");
                Console.WriteLine("Is the file open in another program? In SuperMemo?");
                var flatExs = ae.Flatten().InnerExceptions;
                foreach (var ex in flatExs)
                {
                    Console.WriteLine($"{ex.ToString()}");
                }
                Console.ReadKey();
                */
    }
}