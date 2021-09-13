using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BackRoll.Services.Models;

namespace BackRoll.Services.Comparers
{
    public class FuzzyTrackEqualityComparer : IEqualityComparer<Track>
    {
        private readonly IEqualityComparer<string> _fuzzyStringEqualityComparer;

        public FuzzyTrackEqualityComparer()
        {
            _fuzzyStringEqualityComparer = new FuzzyStringEqualityComparer();
        }

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

            return _fuzzyStringEqualityComparer.Equals(x.Name, y.Name);
        }

        public int GetHashCode([DisallowNull] Track obj)
        {
            return obj.GetHashCode();
        }
    }
}
