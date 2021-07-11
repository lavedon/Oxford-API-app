using System.Collections.Generic;
using System;

namespace OxfordV2
{
    public class Lemmas
    {
        // 'token' is the word the user entered
        // which is then looked up
        public List<string> Tokens { get; set; } 
        // Put the returned Lemma here
        public List<string> WordLemmas { get; set; }

        public Dictionary<string, string> ZippedLemmas { get; set; }
        public string FirstUse { get; set; }
        public string Definition { get; set; }
    }

}