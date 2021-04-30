using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace WindowsFormsApp1
{
    class WordCounting
    {
        public static int CountWords1(string s)
        {
            MatchCollection collection = Regex.Matches(s, @"[\S]+");
            return collection.Count;
        }

        
    }
}
