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
            Console.SetCursorPosition(0, currentLine);
        }
        public static void HandleTabInput(StringBuilder builder, IEnumerable<string> data)
        {
            var currentInput = builder.ToString();
            var match = data.FirstOrDefault(item => item != currentInput && item.StartsWith(currentInput, true, CultureInfo.InvariantCulture));
            if (data.Contains(currentInput.Trim()))
            {
                var nextMatch = data.SkipWhile(item => item != currentInput).Skip(1).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(nextMatch))
                {
                    nextMatch = data.FirstOrDefault();
                }
                ClearCurrentLine();
                builder.Clear();
                Console.Write(nextMatch);
                builder.Append(nextMatch);
            }
            if (string.IsNullOrEmpty(match))
            {
                return;
            }
            // if currentInput equals a match, then move to the next match

            ClearCurrentLine();
            builder.Clear();

            Console.Write(match);
            builder.Append(match);
        }

        public static void HandleKeyInput(StringBuilder builder, IEnumerable<string> data, ConsoleKeyInfo input)
        {
            var currentInput = builder.ToString();
            if (input.Key == ConsoleKey.Backspace && currentInput.Length > 0)
            {
                builder.Remove(builder.Length - 1, 1);
                ClearCurrentLine();

                currentInput = currentInput.Remove(currentInput.Length - 1);
                Console.Write(currentInput);
            }
            else
            {
                var key = input.KeyChar;
                builder.Append(key);
                Console.Write(key);
            }
        }

    }
}