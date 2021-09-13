using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FuzzySharp;

namespace BackRoll.Services.Comparers
{
    public class FuzzyStringEqualityComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            if (y == null && x == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            var ratio = Fuzz.PartialTokenSetRatio(x, y);
            if (ratio > 90)
            {
                return true;
            }

            return false;
        }

        public int GetHashCode([DisallowNull] string obj)
        {
            return obj.GetHashCode();
        }
    }
}
