#nullable enable
#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
                Console.WriteLine("Invalid: QS selection.");
                Console.WriteLine("Can not start with a quote # or quote range.");
                return;

            }
            string[] qsStrings = qsString.Split('s').Skip(1).ToArray();
            Console.WriteLine("There are {0} separate commands.", qsStrings.Length);
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
                        Console.WriteLine("You entered an extra s command without specified quotes");
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
                    Console.WriteLine("Sense to get: #{0}", qsCount);
                    Console.WriteLine("The sense number is: #{0}", currentSelection.SenseNum);
                    Console.WriteLine("The quotes to get from Sense #{0} are:", currentSelection.SenseNum);
                    foreach (int quote in currentSelection.QuotesToGet)
                    {
                        Console.Write(" {0},", quote);
                    }
                } else if (qs.Contains("-") || qs.Contains(",")) {
                    List<int> sensesWithAllQuotes = Program.ParseNumbers(qs);
                    currentSelection = new(sensesToGetAllQuotes: sensesWithAllQuotes);
                    currentQuery.SenseQuoteObjects.Add(currentSelection);
                    continue;
                } else {
                    currentSelection = new(senseNum: int.Parse(qs), true);
                    Console.WriteLine("Sense to get #{0}", qsCount);
                    Console.WriteLine("This is the sense number {0}", currentSelection.SenseNum);
                    Console.WriteLine("I will get ALL quotes from Sense #{0} are:", currentSelection.SenseNum);
                } // end if/else arg contains a quote selection
                Console.ReadLine();
                Console.WriteLine();
                currentQuery.SenseQuoteObjects.Add(currentSelection); 
            } // end foreach qs in qsStrings
            return;
        } // end ParseMultiQS
        public static void ParseSingleQS(string qsString)
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
                        Console.WriteLine("Invalid format!");
                        Console.WriteLine("q found before s!");
                        return;
                    }
                }
                Console.WriteLine("Valid format!");
                string[] splitArgs = qsString.Split('q');
                Console.WriteLine("Sense arg:");
                Console.WriteLine(splitArgs[0]);
                Console.WriteLine("Quote arg:");
                Console.WriteLine(splitArgs[1]);
                // Process sense arg
                string senseArg = splitArgs[0].Trim().Remove(0, 1);
                string quoteArg = splitArgs[1].Trim();
                Console.WriteLine("Quote arg:");
                List<int> sensesToGet = Program.ParseNumbers(senseArg);
                List<int> quotesToGet = Program.ParseNumbers(quoteArg);
                Console.WriteLine("Here are the senses To get:");
                foreach (int i in sensesToGet)
                {
                    Console.WriteLine(i);
                }
                Console.ReadLine();
                Console.WriteLine("Here are the quotes to get:");
                foreach (int i in quotesToGet)
                {
                    Console.WriteLine(i);
                }
                Console.ReadLine();
                } // end if qsString.Contains("s") && qsString.Contains("q")
                Console.WriteLine("No q or s command entered.");
                // No need to return a crazy object? 
                // Use like the normal Senses command.
        }
    } // end class Program

} // end namespace qs_parsing