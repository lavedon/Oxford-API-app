#nullable enable
#nullable disable warnings
using System;
using System.Collections.Generic;

namespace oed {


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

    public class Links
    {
        public string self { get; set; }
    }

    public class Daterange
    {
        public object end { get; set; }
        public int start { get; set; }
        public bool obsolete { get; set; }
        public string rangestring { get; set; }
    }

    public class Categories
    {
        public List<List<string>> topic { get; set; }
        public List<object> usage { get; set; }
        public List<object> region { get; set; }
    }

    public class Text
    {
        public string keyword { get; set; }
        public string full_text { get; set; }
        public int? keyword_offset { get; set; }
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

    */
    public class SQID_Data
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
        public object transitivity { get; set; }
        public string oed_reference { get; set; }
        public string part_of_speech { get; set; }
        public bool main_current_sense { get; set; }
        public List<List<string>> semantic_class_ids { get; set; }
        public List<Quotation> quotations { get; set; }
    }

    public class SQID_Root
    {
        public Meta meta { get; set; }
        public Links links { get; set; }
        public List<SQID_Data> data { get; set; }

    }
}
