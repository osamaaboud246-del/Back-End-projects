using System;
using System.Collections.Generic;
using System.Text;

namespace StudentPortalConsole
{
    public static class StringExtenstion
    {
        public static string RemoveWhiteSpaces(this string text)
        {
            var result = text.Replace(" ", "");
            return result;
        }
    }
}
