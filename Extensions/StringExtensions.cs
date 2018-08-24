using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Extensions
{
    public static class StringExtensions
    {




        public static string Nl2Br(this string obj) => obj.Replace("\r\n", "<br />");

        public static string TrimEnd(this string input, string suffixToRemove, StringComparison comparisonType)
        {

            if (input != null && suffixToRemove != null
              && input.EndsWith(suffixToRemove, comparisonType))
            {
                return input.Substring(0, input.Length - suffixToRemove.Length);
            }
            else return input;
        }

        
    }
}
