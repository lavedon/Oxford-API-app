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
            var currentInput = builder.ToString();
            if (string.IsNullOrWhiteSpace(currentInput))
            {
                return builder;
            }
            // @TODO: trim the input string to only auto-complete relevant sections
            string match;
            if (currentInput.Contains("el "))
            {
                return parseCompletion(ref builder, currentInput, out match, token: "el", CompletionData.ELanguageData);
            } else if (currentInput.Contains("ps ")) 
            {
                return parseCompletion(ref builder, currentInput, out match, token: "ps", CompletionData.PartsOfSpeech);
            }
            return builder;

        } // end contains El

        private static StringBuilder parseCompletion(ref StringBuilder builder, string currentInput, out string match, 
            string token, string[] completionList)
        {
            string relevantInput = currentInput.Substring(currentInput.IndexOf(token) + 3);
            if (string.IsNullOrWhiteSpace(relevantInput))
            {
                match = completionList.FirstOrDefault();
                builder = displayMatch(builder, match, token);
                return builder;
            }
            else if (completionList.Contains(relevantInput.Trim()))
            {
                var nextMatch = completionList.SkipWhile(item => item != relevantInput).Skip(1).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(nextMatch))
                {
                    nextMatch = completionList.FirstOrDefault();
                    builder = displayMatch(builder, nextMatch, token);
                    match = nextMatch;
                    return builder;

                }
                builder = displayMatch(builder, nextMatch, token);
                match = nextMatch;
                return builder;
            }
            else
            {
                match = completionList.FirstOrDefault(item => item != relevantInput && item.StartsWith(relevantInput, true, CultureInfo.InvariantCulture));
                builder = displayMatch(builder, match, token);
                return builder;
            }
        }

        public static StringBuilder displayMatch(StringBuilder builder, string match, string token)
        {
            // TODO fix backspace
            // @TODO fix next match
            ClearCurrentLine();
            token = token + " ";
            string cleanedInput = builder.ToString().Substring(0, builder.ToString().IndexOf(token) + 3);
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