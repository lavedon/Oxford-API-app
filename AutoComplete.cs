using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace oed
{
    public static class AutoComplete
    {
        public static void ClearCurrentLine()
        {
            var currentLine = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.Write('>');
            Console.SetCursorPosition(1, currentLine);
        }
        public static StringBuilder HandleTabInput(StringBuilder builder)
        {
            var currentInput = builder.ToString()!;
            if (string.IsNullOrWhiteSpace(currentInput))
            {
                return builder;
            }
            // @TODO: trim the input string to only auto-complete relevant sections
            string match;
            // @TODO change from "Contains" to Read X characters before cursor position - or end of input)
            string[] inputTokens = currentInput.Split(" ");
            if (inputTokens.Length > 0)
            {
                for (int i = inputTokens.Length - 1; i >= inputTokens.Length - 5; i--)
                {
                    if (inputTokens[i] == "el" || inputTokens[i] == "ps" || inputTokens[i] == "et" || inputTokens[i] == "rr" || inputTokens[i] == "ru")
                    {
                        break;
                    }
                    if (inputTokens[i] == "t")
                    {
                        var relevantInput = inputTokens.SkipWhile(t => t != "t");
                        string renderedInput = string.Join(" ", relevantInput);
                        return parseCompletion(ref builder, currentInput, out match, token: "t", CompletionData.Topics, offset: 2);
                    }
                }
            }

            switch (inputTokens[^2])
            {
                case "el":
                    return parseCompletion(ref builder, currentInput, out match, token: "el", CompletionData.ELanguageData);
                case "ps":
                    return parseCompletion(ref builder, currentInput, out match, token: "ps", CompletionData.PartsOfSpeech);
                case "et":
                    return parseCompletion(ref builder, currentInput, out match, token: "et", CompletionData.ETypesData);
                /*
                case "t":
                        var relevantInput = inputTokens.SkipWhile(t => t != "t");
                        string renderedInput = string.Join(" ", relevantInput);
                        return parseCompletion(ref builder, currentInput, out match, token: "t", CompletionData.Topics, offset: 2);
                */
                case "rr":
                    return parseCompletion(ref builder, currentInput, out match, token: "rr", CompletionData.Regions);
                case "ru":
                    return parseCompletion(ref builder, currentInput, out match, token: "ru", CompletionData.Usage);
            }

            /*
            if (currentInput.EndsWith("el "))
            {
                return parseCompletion(ref builder, currentInput, out match, token: "el", CompletionData.ELanguageData);
            } else if (currentInput.Contains("ps ")) 
            {
                return parseCompletion(ref builder, currentInput, out match, token: "ps", CompletionData.PartsOfSpeech);
            } else if (currentInput.Contains("et "))
            {
                return parseCompletion(ref builder, currentInput, out match, token: "et", CompletionData.ETypesData);
                // @TODO: must make sure there is nothing before t 
            } else if (currentInput.Contains("t "))
            {
                return parseCompletion(ref builder, currentInput, out match, token: "t", CompletionData.Topics, offset: 2);
            } else if (currentInput.Contains("rr "))
            {
                return parseCompletion(ref builder, currentInput, out match, token: "rr", CompletionData.Regions);
            } else if (currentInput.Contains("ru "))
            {
                return parseCompletion(ref builder, currentInput, out match, token: "ru", CompletionData.Usage);
            }
            */

            return builder;

        } // end contains El

        private static StringBuilder parseCompletion(ref StringBuilder builder, string currentInput, out string match, 
            string token, string[] completionList, int offset = 3)
        {
            try {
            string relevantInput = currentInput.Substring(currentInput.IndexOf(token) + offset);
            if (string.IsNullOrWhiteSpace(relevantInput))
            {
                match = completionList.FirstOrDefault();
                builder = displayMatch(builder, match, token, offset);
                return builder;
            }
            else if (completionList.Contains(relevantInput.Trim()))
            {
                var nextMatch = completionList.SkipWhile(item => item != relevantInput).Skip(1).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(nextMatch))
                {
                    nextMatch = completionList.FirstOrDefault();
                    builder = displayMatch(builder, nextMatch, token, offset);
                    match = nextMatch;
                    return builder;

                }
                builder = displayMatch(builder, nextMatch, token, offset);
                match = nextMatch;
                return builder;
            }
            else
            {
                match = completionList.FirstOrDefault(item => item != relevantInput && item.StartsWith(relevantInput, true, CultureInfo.InvariantCulture));
                builder = displayMatch(builder, match, token, offset);
                return builder;
            }
            }
            catch (Exception e)
            {
                xConsole.WriteLine("Problem with auto-complete parser.");
                xConsole.WriteLine(e.Message);
                match = "";
                return null;
            }
        }

        public static StringBuilder displayMatch(StringBuilder builder, string match, string token, int offset)
        {
            // TODO fix backspace
            // @TODO fix next match
            ClearCurrentLine();
            token = token + " ";
            string cleanedInput = builder.ToString().Substring(0, builder.ToString().IndexOf(token) + offset);
            builder.Clear();
            Console.Write(cleanedInput + match);
            builder.Append(cleanedInput + match);
            return builder;
        }

        public static StringBuilder HandleKeyInput(StringBuilder builder, ConsoleKeyInfo input)
        {
            var currentInput = builder.ToString();
            if (input.Key == ConsoleKey.Backspace && currentInput.Length > 0)
            {
                builder.Remove(builder.Length - 1, 1);
                ClearCurrentLine();

                currentInput = currentInput.Remove(currentInput.Length - 1);
                Console.Write(currentInput);
                return builder;
            }
            else
            {
                var key = input.KeyChar;
                builder.Append(key);
                Console.Write(key);
                return builder;
            }
        }

    }
}