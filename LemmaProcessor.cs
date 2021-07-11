using System;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace OxfordV2
{
    class LemmaProcessor
    {
        /*
        static void Main(string[] args)
        {
            List<string> tokens = new List<string>() { "0", "1", "2", "3", "4"};
            List<string> lemmas =  new List<string>() { "", "One", "", "Three", "" };
            GetLemmas();
        }
        */

        //@TODO return a tuple of JsonDocument and CurrentQuery?
        //Or just return CurrentQuery?
        public static CurrentQuery GetLemmas(JsonDocument data, CurrentQuery query)
        {
            /*
            var path = Environment.CurrentDirectory + ".\\lemma-response.json";
            string data = File.ReadAllText(path);
            */

            Dictionary<string, string> wordWithLemmaDict = new();
            List<String> tokenList = new();
            List<string> lemmaWords = new();

            using (JsonDocument json = JsonDocument.Parse(data))
            {
                JsonElement root = json.RootElement;
                // works;
                var tokens = root.EnumerateObject()
                    .Where(block => block.Value.ValueKind == JsonValueKind.Array && block.Name == "data")
                    .SelectMany(block => block.Value.EnumerateArray().Select(subBlock => subBlock.GetProperty("token").GetString()));

                IEnumerable<JsonElement> lemmaBlocks = root.EnumerateObject()
                    .Where(block => block.Value.ValueKind == JsonValueKind.Array && block.Name == "data")
                    .SelectMany(block => block.Value.EnumerateArray().Select(subBlock => subBlock.GetProperty("lemmatizations")));

                List<JsonElement> lemmaList = new();

                lemmaList.AddRange(lemmaBlocks);
                tokenList.AddRange(tokens);

                // @TODO Extract into a local function or method - the actual processing - maybe?
                for (var i = 0; i < lemmaList.Count; i++)
                {
                    try
                    {
                        var lemmas = lemmaList[i].EnumerateArray();

                        // @TODO make sure if lemmas is blank that a "" word gets added
                        if (lemmaList[i].GetArrayLength() == 0)
                        {

                            lemmaWords.Add(string.Empty);
                        }
                        else
                        {
                            while (lemmas.MoveNext())
                            {
                                var lwordsObj = lemmas.Current.EnumerateObject();

                                while (lwordsObj.MoveNext())
                                {
                                    if (lwordsObj.Current.Name == "word")
                                    {
                                        lemmaWords.Add(lwordsObj.Current.Value.GetProperty("lemma").GetString());
                                        goto lemmaFound;
                                    }
                                }

                            }
                        }
                    //  var lWord = lemmas.Current.TryGetProperty("lemma", out JsonElement lemmaElement);
                    lemmaFound:;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error while enumerating lemmas");
                        Console.WriteLine(ex);
                    }
                }
            }
            var result = zipLemmas(tokenList, lemmaWords);
            Console.WriteLine("Show Results");
            foreach(var r in result)
            {
                Console.WriteLine(r);
            }
            query.Lemmas.ZippedLemmas = zipLemmas;
            return query;
        }

        private static Dictionary<string, string> zipLemmas(List<string> tokens, List<string> lemmas)
        {
            Dictionary<string, string> zippedLemmas = new();
            for (var i = 0; i < tokens.Count; i++)
            {
                for (var y = 0; y < lemmas.Count; y++)
                {
                    if (string.IsNullOrWhiteSpace(lemmas[y]))
                    {
                        lemmas[y] = tokens[i];
                    }
                    zippedLemmas.Add(tokens[i], lemmas[y]);
                    i++;
                }
            }
                return zippedLemmas;
        }
    }
}
