using System;

namespace oed {
    public static class xConsole {
        public static int PageBreak { get; set; } = Console.WindowHeight - 2;
        public static int LinesWritten { get; set; } = 0;

        public static void WriteLine(string text)
        {
            checkPageBreak();
            Console.WriteLine(text);
            LinesWritten++;
        }
        public static void WriteLine()
        {
            Console.WriteLine();
            checkPageBreak();
            LinesWritten++;
        }
        public static void WriteLine(string text1, string text2)
        {
            checkPageBreak();
            Console.WriteLine(text1 + text2);
            LinesWritten++;
        }
        public static void WriteLine(string text1, string text2, string text3)
        {
            checkPageBreak();
            Console.WriteLine(text1 + text2 + text3);
            LinesWritten++;
        }
        public static void WriteLine(string text1, object obj2)
        {
            checkPageBreak();
            Console.WriteLine(text1, obj2);
            LinesWritten++;
        }

        public static void WriteLine(Object obj1)
        {
            checkPageBreak();
            Console.WriteLine(obj1);
            LinesWritten++;
        }
        public static void WriteLine(string text1, object obj2, object obj3)
        {
            checkPageBreak();
            Console.WriteLine(text1, obj2, obj3);
            LinesWritten++;
        }
        public static void WriteLine(string text1, object obj2, object obj3, object obj4)
        {
            checkPageBreak();
            Console.WriteLine(text1, obj2, obj3, obj4);
            LinesWritten++;
        }
        public static void WriteLine(string text1, object obj2, object obj3, object obj4, object obj5)
        {
            checkPageBreak();
            Console.WriteLine(text1, obj2, obj3, obj4, obj5);
            LinesWritten++;
        }

        private static void checkPageBreak()
        {
            if (LinesWritten >= PageBreak)
            {
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                PageBreak = PageBreak + Console.WindowHeight - 2;
            }
        }
    }
}