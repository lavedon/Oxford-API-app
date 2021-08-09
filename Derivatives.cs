using System;
using System.Collections.Generic;

namespace oed {
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
/*
    public class Links
    {
        public string self { get; set; }
        public object prev { get; set; }
        public object next { get; set; }
        public string first { get; set; }
        public string last { get; set; }
    }
*/

/*
    public class Daterange
    {
        public int? end { get; set; }
        public int start { get; set; }
        public bool obsolete { get; set; }
        public string rangestring { get; set; }
    }
    */

    public class Etymon
    {
        public string word { get; set; }
        public string language { get; set; }
        public string target_id { get; set; }
        public string part_of_speech { get; set; }
    }

    public class DerivativeEtymology
    {
        public List<Etymon> etymons { get; set; }
        public string etymology_type { get; set; }
        public List<List<string>> etymon_language { get; set; }
        public List<List<string>> source_language { get; set; }
        public string etymology_summary { get; set; }
    }

    public class Inflection
    {
        public string form { get; set; }
        public string part_of_speech { get; set; }
        public string region { get; set; }
        public List<Inflection> inflections { get; set; }
    }

    public class Pronunciation
    {
        public List<string> ipa { get; set; }
        public string region { get; set; }
    }

    public class Derivatives
    {
        public string id { get; set; }
        public int band { get; set; }
        public Meta meta { get; set; }
        public string lemma { get; set; }
        public string oed_url { get; set; }
        public Daterange daterange { get; set; }
        public DerivativeEtymology etymology { get; set; }
        public string first_use { get; set; }
        public List<List<double>> frequency { get; set; }
        public List<string> sense_ids { get; set; }
        public string definition { get; set; }
        public bool main_entry { get; set; }
        public List<Inflection> inflections { get; set; }
        public string frequency_id { get; set; }
        public string oed_reference { get; set; }
        public List<Pronunciation> pronunciations { get; set; }
        public List<string> parts_of_speech { get; set; }
        public string primary_sense_id { get; set; }
    }

    public class DerivativesRoot
    {
        public Meta meta { get; set; }
        public Links links { get; set; }
        public List<Derivatives> data { get; set; }
    }

}
