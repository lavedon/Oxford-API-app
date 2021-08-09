using System;
using System.Collections.Generic;

namespace oed {
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    /*
    public class Meta
    {
        public string provider { get; set; }
        public string cite { get; set; }
        public int created { get; set; }
        public bool revised { get; set; }
        public int updated { get; set; }
        public string sense_group { get; set; }
        public int position_in_entry { get; set; }
    }
    */

/*
    public class Links
    {
        public string self { get; set; }
    }
    */
/*
    public class Daterange
    {
        public object end { get; set; }
        public int start { get; set; }
        public bool obsolete { get; set; }
        public string rangestring { get; set; }
    }

    public class Etymon
    {
        public string word { get; set; }
        public string language { get; set; }
        public string target_id { get; set; }
        public string part_of_speech { get; set; }
    }
    */

// 
    public class SQ_Etymology
    {
        public List<Etymon> etymons { get; set; }
        public string etymology_type { get; set; }
        public List<List<string>> etymon_language { get; set; }
        public List<List<string>> source_language { get; set; }
        public string etymology_summary { get; set; }
    }

    public class Inflection2
    {
        public string form { get; set; }
        public string part_of_speech { get; set; }
        public string region { get; set; }
        public List<Inflection> inflections { get; set; }
    }
/*
    public class Pronunciation
    {
        public List<string> ipa { get; set; }
        public string region { get; set; }
    }
    */

    public class Categories
    {
        public List<List<string>> topic { get; set; }
        public List<List<string>> usage { get; set; }
        public List<List<string>> region { get; set; }
    }

    public class Transitivity
    {
        public bool reflexive { get; set; }
        public bool transitive { get; set; }
        public bool intransitive { get; set; }
    }

    public class Text
    {
        public string keyword { get; set; }
        public string full_text { get; set; }
        public int keyword_offset { get; set; }
    }

    public class Source
    {
        public string title { get; set; }
        public string author { get; set; }
        public string gender { get; set; }
    }

    public class Quotation
    {
        public string id { get; set; }
        public Text text { get; set; }
        public int year { get; set; }
        public string lemma { get; set; }
        public Source source { get; set; }
        public string oed_url { get; set; }
        public string word_id { get; set; }
        public string sense_id { get; set; }
        public string datestring { get; set; }
        public bool first_in_word { get; set; }
        public string oed_reference { get; set; }
        public bool first_in_sense { get; set; }
    }

    public class Sens
    {
        public string id { get; set; }
        public Meta meta { get; set; }
        public string lemma { get; set; }
        public List<string> notes { get; set; }
        public string oed_url { get; set; }
        public string word_id { get; set; }
        public Daterange daterange { get; set; }
        public string first_use { get; set; }
        public Categories categories { get; set; }
        public string definition { get; set; }
        public Transitivity transitivity { get; set; }
        public string oed_reference { get; set; }
        public string part_of_speech { get; set; }
        public bool main_current_sense { get; set; }
        public List<List<string>> semantic_class_ids { get; set; }
        public List<Quotation> quotations { get; set; }
    }

    public class SQ_Data
    {
        public string id { get; set; }
        public int band { get; set; }
        public Meta meta { get; set; }
        public string lemma { get; set; }
        public string oed_url { get; set; }
        public Daterange daterange { get; set; }
        public Etymology etymology { get; set; }
        public string first_use { get; set; }
        public List<List<double>> frequency { get; set; }
        public string definition { get; set; }
        public bool main_entry { get; set; }
        public List<Inflection> inflections { get; set; }
        public string frequency_id { get; set; }
        public string oed_reference { get; set; }
        public List<Pronunciation> pronunciations { get; set; }
        public List<string> parts_of_speech { get; set; }
        public List<Sens> senses { get; set; }
    }

    public class SQ_Root
    {
        public Meta meta { get; set; }
        public Links links { get; set; }
        public Data data { get; set; }
    }
}