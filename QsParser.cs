#nullable enable
#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace oed
{
    public static class QsParser 
    {
        /*
        static void TestMain(string[] args)
        {
            bool running = true;
            while (running) {
                string qsString;
                Console.WriteLine("Enter the QS parsing string:");
                qsString = Console.ReadLine().Trim().ToLower().Replace(" ", "");
                if (qsString == "q") {
                    return;
                }
                int feq = qsString.Count(x => (x == 's'));
                if (feq > 1)
                {
                    CurrentQuery query = new CurrentQuery();
                    ParseMultiQS(qsString, ref query);
                    Console.WriteLine("The original query was {0}", qsString);
                    displayQuery(query);
                    Console.WriteLine();
                } 
                else {
                    ParseSingleQS(qsString);
                }
            } // end while
        } // end main
        private static void displayQuery(CurrentQuery query)
        {
            Console.WriteLine("Displaying all saved selections in query...");
            foreach (var selection in query.SenseQuoteObjects)
            {
                Console.WriteLine();
                    if (selection.SensesToGetAllQuotes is not null)
                    {
                        Console.WriteLine();
                        Console.Write("For these sense #'s, get all the quotes: ");
                    try {
                        foreach (int s in selection.SensesToGetAllQuotes)
                        {
                            Console.Write($"{s}, ");
                        }
                        Console.WriteLine();
                        continue;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error: " + e.Message);
                    }
                } 
                Console.WriteLine($"Sense Number#:{selection.SenseNum}");
                if (selection.AllQuotesFlag)
                {
                    Console.WriteLine("Just get all quotes for this sense.");
                    continue;
                }
                if (selection.QuotesToGet.Count != 0)
                {
                    Console.Write($"Quotes to get:");
                    foreach (int q in selection.QuotesToGet)
                    {
                        Console.Write($"{q},");
                    }
                }
            }
        }
        */

        public static void ParseMultiQS(string qsString, ref CurrentQuery currentQuery)
        {
            if (qsString[0] == 'q') 
            {
                xConsole.WriteLine("Invalid: QS selection.");
                xConsole.WriteLine("Can not start with a quote # or quote range.");
                return;

            }
            string[] qsStrings = qsString.Split('s').Skip(1).ToArray();
            Trace.WriteLine($"There are {qsStrings.Length} separate commands.");
            // Below code will remove the ability to enter a range of senses.
            // Quotes will only be specified for each individual sense.
            /*
            Match match = Regex.Match(qsString, @"s\d-");
            if (match.Success) {
                Console.WriteLine("Error: Sense range entered in multiple quote selection.");
                Console.WriteLine("You entered: {0}.  This is not valid.", match.Value);
                return;
            }
            */
            int qsCount = 0;
            foreach (string qs in qsStrings)
            {
                SenseQSSelection currentSelection;
                qsCount++;
                if (qs.Contains("q"))
                {
                    // @TODO This will not work for s1q2-5s5-9 
                    // Will need a way to set quotes all for all senses if q is not specified.
                    string quoteArg = qs.Split('q')[1];
                    // Check if quoteArgs contains an extra sense command i.e. s1q2-5s5-9
                    if (quoteArg.Contains("s"))
                    {
                        xConsole.WriteLine("You entered an extra s command without specified quotes");
                        return;
                        // @TODO Actually process this command, iterate through and set 
                        // each ense to all quotes.
                    }
                    string senseArg = qs.Split('q')[0];
                    if (senseArg.Contains("-") || senseArg.Contains(","))
                    {
                        List<int> multiSenseSelection = Program.ParseNumbers(senseArg);
                        if (quoteArg.Contains("-") || quoteArg.Contains(",")) 
                        {
                            List<int> quotesForEach = Program.ParseNumbers(quoteArg);
                            foreach (int sense in multiSenseSelection)
                            {
                                currentSelection = new SenseQSSelection(senseNum: sense, quotesToGet: quotesForEach);
                                currentQuery.SenseQuoteObjects.Add(currentSelection);
                            }
                        } else {
                            foreach (int sense in multiSenseSelection)
                            {
                                List<int> quoteToGet = new List<int>();
                                quoteToGet.Add(int.Parse(quoteArg));
                                currentSelection = new SenseQSSelection(sense, quoteToGet);
                                currentQuery.SenseQuoteObjects.Add(currentSelection);
                            }
                        }
                        // Put this in a block where Quote arg is blank.
                        /*
                        currentSelection = new(multiSenseSelection);
                        Console.WriteLine("Some senses where you want to get all quotes.");
                        Console.ReadLine();
                        */
                    continue;
                    } else {
                        List<int> quotesList = Program.ParseNumbers(quoteArg);
                        currentSelection = new(int.Parse(senseArg), quotesList);
                    } // end if/else sense is a range
                    Trace.WriteLine($"Sense to get: #{qsCount}");
                    Trace.WriteLine($"The sense number is: #{currentSelection.SenseNum}");
                    Trace.WriteLine($"The quotes to get from Sense #{currentSelection.SenseNum} are:");
                    foreach (int quote in currentSelection.QuotesToGet)
                    {
                        Trace.Write(quote + " ");
                    }
                } // end if qs.Contains('q') 
                else if (qs.Contains("-") || qs.Contains(",")) {
                    List<int> sensesWithAllQuotes = Program.ParseNumbers(qs);
                    currentSelection = new(sensesToGetAllQuotes: sensesWithAllQuotes);
                    currentQuery.SenseQuoteObjects.Add(currentSelection);
                    continue;
                } else { // single s command 
                    currentSelection = new(senseNum: int.Parse(qs), true);
                    Trace.WriteLine("Sense to get #" + qsCount);
                    Trace.WriteLine("This is the sense number " + currentSelection.SenseNum);
                    Trace.WriteLine($"I will get ALL quotes from Sense #{currentSelection.SenseNum} are:");
                } // end if/else arg contains a quote selection
                currentQuery.SenseQuoteObjects.Add(currentSelection); 
            } // end foreach qs in qsStrings
            return;
        } // end ParseMultiQS
            public static void ParseSingleQS(string qsString, ref CurrentQuery currentQuery)
            {
            Console.WriteLine(qsString);
            if (qsString.Contains("s") && qsString.Contains("q"))
            {
                foreach (char c in qsString)
                {
                    if (c == 's')
                        break;
                    if (c == 'q')
                    {
                        xConsole.WriteLine("Invalid format!");
                        xConsole.WriteLine("q found before s!");
                        return;
                    }
                }
                // if there is no Q argument
                string[] splitArgs = qsString.Split('q');
                // Process sense arg
                string senseArg = splitArgs[0].Trim().Remove(0, 1);
                string quoteArg = splitArgs[1].Trim();
                List<int> sensesToGet = Program.ParseNumbers(senseArg);
                List<int> quotesToGet = Program.ParseNumbers(quoteArg);
                Console.WriteLine("Here are the senses To get:");
                if (sensesToGet.Count == 1)
                {
                    currentQuery.SenseQuoteObjects.Add(new SenseQSSelection(senseNum: sensesToGet[0],quotesToGet: quotesToGet));
                } else {
                    foreach (int sense in sensesToGet) {
                        currentQuery.SenseQuoteObjects.Add(new SenseQSSelection(senseNum: sense,quotesToGet: quotesToGet));
                    }
                }
                } // end if qsString.Contains("s") && qsString.Contains("q")
                else if (qsString.Contains("s") && !qsString.Contains("q"))
                {
                    string senseArg = qsString.Trim().Remove(0, 1);
                    if (!qsString.Contains("-"))
                    {
                        currentQuery.SenseQuoteObjects.Add(new SenseQSSelection(senseNum: int.Parse(senseArg), allQuotesFlag: true));
                    } else {
                        List<int> sensesToGet = Program.ParseNumbers(senseArg);
                        foreach (int sense in sensesToGet)
                        {
                            currentQuery.SenseQuoteObjects.Add(new SenseQSSelection(senseNum: sense, allQuotesFlag: true));
                        }

                    }
                } // end else if qsString.Contains("s") && !qsString.Contains("q")
            } // end ParseSingleQS
    } // end class Program

} // end namespace qs_parsing