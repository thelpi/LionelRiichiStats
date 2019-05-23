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
        public double Probability { get; protected set; }

        internal Substitution(TilePivot subbed, TilePivot subber, IEnumerable<TilePivot> subSource)
        {
            Subbed = subbed;
            Subber = subber;
            SubSource = subSource.ToList();
            Probability = 1 / (double)SubSource.Count; // Si distinct : SubSource.Count(t => t.Equals(Subber))
        }
    }

    internal class SubstitutionSequence
    {
        private List<Substitution> _substitutions;

        public IReadOnlyCollection<Substitution> Substitutions { get { return _substitutions; } }
        public double Probability { get; private set; }

        internal SubstitutionSequence()
        {
            _substitutions = new List<Substitution>();
            Probability = 1;
        }

        internal void AddSubstitution(Substitution substitution)
        {
            _substitutions.Add(substitution);
            Probability *= substitution.Probability;
        }
    }

    internal class SubstitutionGroup
    {
        private List<SubstitutionSequence> _substitutionSequences;
        private readonly bool _noSubAdded;

        public IReadOnlyCollection<SubstitutionSequence> SubstitutionSequences { get { return _substitutionSequences; } }
        public double Probability { get; private set; }
        public HandYakuListPivot Yakus { get; private set; }

        internal SubstitutionGroup(HandYakuListPivot handYakuListPivot, bool noSubAdded = false)
        {
            _substitutionSequences = new List<SubstitutionSequence>();
            Yakus = handYakuListPivot;
            Probability = noSubAdded ? 1 : 0;
            _noSubAdded = noSubAdded;
        }

        internal void AddSubstitutionSequence(SubstitutionSequence substitutionSequence)
        {
            if (_noSubAdded)
            {
                throw new System.InvalidOperationException("No 'SubstitutionSequence' can't be added in this context.");
            }

            _substitutionSequences.Add(substitutionSequence);
            Probability += substitutionSequence.Probability;
        }
    }
}
