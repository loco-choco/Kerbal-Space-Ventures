using System;

namespace KSPCfgParser.Models
{
    public class InputLine
    {
        public InputLine(int rawLineNumber)
        {
            RawLineNumber = rawLineNumber;
        }

        public int RawLineNumber { get; }

        public int BlockId { get; set; }
        public int BlockDepth { get; set; }

        ///<summary>Anything up to the first "//" on a line</summary>
        public string Data { get; set; }

        ///<summary>Anything after the first "//" (comment) on a line</summary>
        public string Comment { get; set; }
    }
}