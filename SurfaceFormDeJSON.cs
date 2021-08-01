using System;
using System.Collections.Generic;

namespace oed {
        public class Meta
        {
            public int total { get; set; }
            public int count { get; set; }
            public int offset { get; set; }
            public int limit { get; set; }
            public string provider { get; set; }
            public string cite { get; set; }
        }

        public class Links
        {
            public string self { get; set; }
            public object prev { get; set; }
            public object next { get; set; }
            public string first { get; set; }
            public string last { get; set; }
        }

        public class Daterange
        {
            public int? end { get; set; }
            public int start { get; set; }
            public bool obsolete { get; set; }
            public string rangestring { get; set; }
        }

        public class Datum
        {
            public string id { get; set; }
            public int band { get; set; }
            public string form { get; set; }
            public string lemma { get; set; }
            public string region { get; set; }
            public string word_id { get; set; }
            public bool computed { get; set; }
            public Daterange daterange { get; set; }
            public bool irregular { get; set; }
            public string normalized { get; set; }
            public string part_of_speech { get; set; }
            public bool standard_us_form { get; set; }
            public bool standard_british_form { get; set; }
            
        }

        public class SurfaceFormDeJSON
        {
            public Meta meta { get; set; }
            public Links links { get; set; }
            public List<Datum> data { get; set; }
        }
} 
