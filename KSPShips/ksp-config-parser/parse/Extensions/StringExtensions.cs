using System.Collections.Generic;
using KSPCfgParser.Models;

namespace KSPCfgParser.Extensions
{
    public static class StringExtensions
    {
        public static InputLine ParseLine(this string line, int rawLineNumber)
        {
            const string commentMarker = "//";

            var result = new InputLine(rawLineNumber);
            if (line == null) return result;

            // Comments trump everything else
            var commentStartIndex = line.IndexOf(commentMarker);
            if (commentStartIndex >= 0)
            {
                result.Comment = line.Substring(commentStartIndex + commentMarker.Length).Trim();
                line = line.Substring(0, commentStartIndex);
            }

            result.Data = line.Trim();

            return result;
        }
    }
}