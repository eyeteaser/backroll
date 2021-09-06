using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BackRoll.Services.Models;
using FuzzySharp;

namespace BackRoll.Services.Comparers
{
    public class FuzzyTrackEqualityComparer : IEqualityComparer<Track>
    {
        public bool Equals(Track x, Track y)
        {
            if (y == null && x == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            if (x.Name == y.Name)
            {
                return true;
            }

            var ratio = Fuzz.PartialTokenSetRatio(x.Name, y.Name);
            if (ratio > 90)
            {
                return true;
            }

            return false;
        }

        public int GetHashCode([DisallowNull] Track obj)
        {
            return obj.GetHashCode();
        }
    }
}
