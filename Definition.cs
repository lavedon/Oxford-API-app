using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oed
{
    public class Definition
    {
        public string WordDefinition { get; set; }
        public string WordID { get; set; }
        public bool IsWordMainDefinition { get; set; }
        public bool IsWordObsolete { get; set; }
        public string RecordedFirstUseSource { get; set; }
        public int RecordedFirstUseYear { get; set; }
        public List<string> PartsOfSpeech { get; set;}
        public List<string> FormattedPartsOfSpeech { get; set; }
    	public string FormattedVerboseOutput { get; set; }

        public Etymology DefinitionEtymology { get; set; }

        public string BritishIPA { get; set; }
        public string USIPA { get; set; }

    public Definition() {
        this.PartsOfSpeech = new List<string>();
        this.FormattedPartsOfSpeech = new List<string>();
        this.FormattedVerboseOutput = "";
        this.DefinitionEtymology = new();

    }

    public void FormatPartsOfSpeech() {
        foreach (string p in this.PartsOfSpeech)
            switch (p) {
                case("VBN"):
                FormattedPartsOfSpeech.Add("Verb, past participle");
                break;

                case("NN"):
                FormattedPartsOfSpeech.Add("Noun");
                break;

                case("NNS"):
                FormattedPartsOfSpeech.Add("Plural noun");
                break;

                case("PRP"):
                FormattedPartsOfSpeech.Add("Personal pronoun");
                break;

                case("PRP$"):
                FormattedPartsOfSpeech.Add("Possessive pronoun");
                break;

                case("WP"):
                FormattedPartsOfSpeech.Add("wh-pronoun");
                break;

                case("WP$"):
                FormattedPartsOfSpeech.Add("Possessive wh-pronoun");
                break;

                case("JJ"):
                FormattedPartsOfSpeech.Add("Adjective");
                break;

                case("JJR"):
                FormattedPartsOfSpeech.Add("Comparative adjective");
                break;

                case("JJS"):
                FormattedPartsOfSpeech.Add("Superlative adjective");
                break;

                case("MD"):
                FormattedPartsOfSpeech.Add("Modal");
                break;

                case("VB"):
                FormattedPartsOfSpeech.Add("Verb");
                break;

                case("VBD"):
                FormattedPartsOfSpeech.Add("Verb, past tense");
                break;

                case("VBG"):
                FormattedPartsOfSpeech.Add("Verb, present participle");
                break;

                case("VBZ"):
                FormattedPartsOfSpeech.Add("Verb, 3rd person singular present");
                break;

                case("RB"):
                FormattedPartsOfSpeech.Add("Comparative adverb");
                break;

                case("RBR"):
                FormattedPartsOfSpeech.Add("Comparative adverb");
                break;

                case("RBS"):
                FormattedPartsOfSpeech.Add("Superlative adverb");
                break;

                case("WRB"):
                FormattedPartsOfSpeech.Add("Wh-adverb");
                break;

                case("IN"):
                FormattedPartsOfSpeech.Add("Preposition");
                break;

                case("CC"):
                FormattedPartsOfSpeech.Add("Coordinating conjunction");
                break;

                case("UH"):
                FormattedPartsOfSpeech.Add("Interjection");
                break;

                case("NNP"):
                FormattedPartsOfSpeech.Add("Proper noun");
                break;

                case("CD"):
                FormattedPartsOfSpeech.Add("Cardinal number");
                break;

                case("DT"):
                FormattedPartsOfSpeech.Add("Determiner");
                break;

                case("EX"):
                FormattedPartsOfSpeech.Add("Existential there");
                break;

                case("PDT"):
                FormattedPartsOfSpeech.Add("Predeterminer");
                break;

                case("WDT"):
                FormattedPartsOfSpeech.Add("Wh-determiner");
                break;

                case("FW"):
                FormattedPartsOfSpeech.Add("Foreign word");
                break;

                case("POS"):
                FormattedPartsOfSpeech.Add("Possessive ending");
                break;

                case("RP"):
                FormattedPartsOfSpeech.Add("Particle");
                break;

                case("SYM"):
                FormattedPartsOfSpeech.Add("Symbol");
                break;

                case("TO"):
                FormattedPartsOfSpeech.Add("Infinitive to");
                break;

                default:
                    FormattedPartsOfSpeech.Add(p.ToString());
                break;
                
              
            }

    }
        
    }
}
