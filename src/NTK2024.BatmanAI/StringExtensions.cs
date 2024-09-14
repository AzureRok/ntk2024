using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTK2024.BatmanAI
{
    internal static class StringExtensions
    {
        public static string RemoveExplanation(this string value)
        {
            var posStart = value.IndexOf("-----", StringComparison.Ordinal);
            var result = (posStart > 0) ? value.Substring(0, posStart).Trim() : value;
            //var ai = result.IndexOf("AI:");
            //if (ai > -1)
            //{
            //    result = result.Substring(ai + 3).Trim();
            //}

            return result;
        }
    }
}
