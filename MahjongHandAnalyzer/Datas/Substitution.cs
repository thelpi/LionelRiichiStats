using System.Collections.Generic;
using System.Linq;
using MahjongDll.Pivot;

namespace MahjongHandAnalyzer.Datas
{
    internal class Substitution
    {
        public TilePivot Subbed { get; protected set; }
        public TilePivot Subber { get; protected set; }
        public IReadOnlyCollection<TilePivot> SubSource { get; protected set; }

        internal Substitution(TilePivot subbed, TilePivot subber, IEnumerable<TilePivot> subSource)
        {
            Subbed = subbed;
            Subber = subber;
            SubSource = subSource.ToList();
        }
    }
}
