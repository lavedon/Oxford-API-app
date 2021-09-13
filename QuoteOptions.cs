#nullable enable
using System;
using System.Collections.Generic;

namespace oed
{
        public class QuoteOptions
        {
                private string? _sourceTitle;
                private string? _author;
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
                public string? Author {
                    get => _author;
                    set {
                     _author = _author = value!.Trim().Replace(" ","%20").Replace("'","");
                    }
                }
                public bool Male { get; set; } = false;
                public bool Female { get; set; } = false;
                public bool FirstWord { get; set; } = false;
                public bool FirstSense { get; set; } = false;

                public string? FromDefinition { get; set; } = null;
                public List<string> WordIDsToUse { get; set; }

                public string? FromSense { get; set; } = null;
                public List<string> SenseIDsToUse { get; set; }

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
                        xConsole.WriteLine("Invalid gender entered");
                        Console.ReadLine();
                        return (false, "");
                    }
                }
                */
                public QuoteOptions(bool male, bool female, string? sourceTitle, string? author, bool firstWord, bool firstSense, string? fromDefinition, string? fromSense, bool useWords, bool useSenses)
                {
                    // this.AuthorGender = authorGender;
                    this.Male = male;
                    this.Female = female;
                    this.SourceTitle = sourceTitle;
                    this.Author = author;
                    this.FirstWord = firstWord;
                    this.FirstSense = firstSense;
                    this.FromDefinition = fromDefinition;
                    this.WordIDsToUse = new List<string>();
                    this.FromSense = fromSense;
                    this.SenseIDsToUse = new List<string>();
                    this.UseWords = useWords;

                    this.UseSenses = useSenses;


                }
        }
}
