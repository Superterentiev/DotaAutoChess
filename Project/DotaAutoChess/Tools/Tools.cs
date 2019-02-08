using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DotaAutoChess.Tools
{
    public static class Tools
    {

        public static int CleanSymbols(string inString)
        {
            Regex digitsOnly = new Regex(@"[^\d]");
            return int.Parse(digitsOnly.Replace(inString, ""));
        }

        public static int GetRowIndex(object obj)
        {
            string name = obj.GetType().GetProperty("Name").GetValue(obj, null).ToString().ToLower().Replace("textblock", "");
            int result = CleanSymbols(name.Replace(name.Substring(name.IndexOf("c")), ""));
            return result;
        }
        public static int GetColumnIndex(object obj)
        {
            string name = obj.GetType().GetProperty("Name").GetValue(obj, null).ToString().ToLower().Replace("textblock", "");
            int result = CleanSymbols(name.Substring(name.IndexOf("c")));
            return result;
        }


    }
}
