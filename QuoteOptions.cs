#nullable enable
using System;

namespace oed
{
        public class QuoteOptions
        {
                private string? _sourceTitle;
                // private string? _authorGender;
                /*
                public string? AuthorGender { 
                    get => _authorGender; 
                    set { 
                        if (!string.IsNullOrWhiteSpace(value)) {
                            var tuple = validateGender(value!);
                                if (tuple.Item1) { 
                                _authorGender = tuple.Item2;
                                }
                        }
                        else {
                            _authorGender = value!;
                        }
                    }
                }
                */
                public string? SourceTitle {
                    get => _sourceTitle;
                    set => _sourceTitle = _sourceTitle = value!.Trim().Replace(" ","%20");
                }
                public bool Male { get; set; } = false;
                public bool Female { get; set; } = false;
                public bool FirstWord { get; set; } = false;
                public bool FirstSense { get; set; } = false;

                public bool UseWords { get; set; } = false;
                public bool UseSenses { get; set; } = false;
		public bool UseNonIdEndpoint { get; set; } = false;

/*
                private (bool, string) validateGender(string input)
                {
                    if (input.Trim().ToLower() == "m" || input.Trim().ToLower() == "male")
                    {
                        return (true, "male");
                    }
                    else if (input.Trim().ToLower() == "f" || input.Trim().ToLower() == "female")
                    {
                        return (true, "female");
                    }
                    else 
                    {
                        Console.WriteLine("Invalid gender entered");
                        Console.ReadLine();
                        return (false, "");
                    }
                }
                */
                public QuoteOptions(bool male, bool female, string? sourceTitle, bool firstWord, bool firstSense, bool useWords, bool useSenses)
                {
                    // this.AuthorGender = authorGender;
                    this.Male = male;
                    this.Female = female;
                    this.SourceTitle = sourceTitle;
                    this.FirstWord = firstWord;
                    this.FirstSense = firstSense;
                    this.UseWords = useWords;
                    this.UseSenses = useSenses;
                }
        }
}
