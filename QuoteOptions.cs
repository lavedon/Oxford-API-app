#nullable enable
using System;

namespace OxfordV2
{
        public class QuoteOptions
        {
                private string? _sourceTitle;
                public string? AuthorGender { get; set; }
                public string? SourceTitle {
                    get => _sourceTitle;
                    set => _sourceTitle = _sourceTitle = value!.Trim().Replace(" ","%20");
                }
                public bool FirstWord { get; set; }
                public bool FirstSense { get; set; }

                public QuoteOptions(string? authorGender, string? sourceTitle, bool firstWord, bool firstSense)
                {
                    this.AuthorGender = authorGender;
                    this.SourceTitle = sourceTitle;
                    this.FirstWord = firstWord;
                    this.FirstSense = firstSense;
                }
        }
}