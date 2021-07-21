using System;
using System.Collections.Generic;

namespace oed
{
    public class Etymology
    {
        public List<string> Etymons { get; set; }
        public string EtymologyType { get; set; }
        public List<string> EtymonLanguage { get; set; }
        public List<string> SourceLanguage { get; set; }
        public string EtymologySummary { get; set; }

        public Etymology() {
            this.Etymons = new();
            this.EtymonLanguage = new();
            this.SourceLanguage = new();
        }
    }
}