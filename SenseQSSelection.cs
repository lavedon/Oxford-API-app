using System.Collections.Generic;

namespace oed {
    public class SenseQSSelection
    {
        public int? SenseNum { get; set; }
        public List<int>? QuotesToGet { get; set; }
        public List<int>? SensesToGetAllQuotes { get; set;}
        public bool AllQuotesFlag { get; set; } = false;



        public SenseQSSelection(int senseNum)
        {
            this.SenseNum = senseNum;
            this.QuotesToGet = new();
        }

        public SenseQSSelection(List<int> sensesToGetAllQuotes)
        {
            this.SensesToGetAllQuotes = sensesToGetAllQuotes;
            this.AllQuotesFlag = true;
            this.SenseNum = null;
        }

        public SenseQSSelection(int senseNum, List<int> quotesToGet)
        {
            this.SenseNum = senseNum;
            this.QuotesToGet = quotesToGet;
        }

        public SenseQSSelection()
        {

        }

        public SenseQSSelection(int senseNum, bool allQuotesFlag)
        {
            this.SenseNum = senseNum;
            this.AllQuotesFlag = allQuotesFlag;
            this.QuotesToGet = null;
        }

    } // end QS Selection class
}