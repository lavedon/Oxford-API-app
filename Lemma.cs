using System.Collections.Generic;
using System;

namespace OxfordV2
{
    public class Lemma
    {
        // 'token' is the word the user entered
        // which is then looked up
        public string Token { get; set; } 
        // Put the returned Lemma here
        public string WordLemma { get; set; }
        public string FirstUse { get; set; }
        public string Definition { get; set; }
    }

}