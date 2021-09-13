using System;

namespace oed {
    public static class xConsole {
        public static int PageBreak { get; set; } = Console.WindowHeight - 2;
        public static int LinesWritten { get; set; } = 0;

        public static void WriteLine(string text) {
            if (LinesWritten >= PageBreak) {
                Console.WriteLine("Press enter to continue...");
                Console.ReadLine();
                PageBreak = PageBreak + Console.WindowHeight - 2;
            }
            Console.WriteLine(text);
            LinesWritten++;
        } 
    }
}