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
            Probability = SubSource.Count(t => t.Equals(Subber)) / (double)SubSource.Count;
        }

        public override string ToString()
        {
            return $"[ {Subbed} / {Subber} ]";
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

        public override string ToString()
        {
            return $"( {string.Join(" ; ", Substitutions)} )";
        }
    }

    internal class SubstitutionGroup
    {
        private List<SubstitutionSequence> _substitutionSequences;

        public IReadOnlyCollection<SubstitutionSequence> SubstitutionSequences { get { return _substitutionSequences; } }
        public HandYakuListPivot Yakus { get; private set; }
        public double Probability
        {
            get
            {
                return _substitutionSequences.Count == 0 ? 1 :
                    _substitutionSequences.Sum(subSeq => subSeq.Probability) / _substitutionSequences.First().Substitutions.Count;
            }
        }

        internal SubstitutionGroup(HandYakuListPivot handYakuListPivot)
        {
            _substitutionSequences = new List<SubstitutionSequence>();
            Yakus = handYakuListPivot;
        }

        internal void AddSubstitutionSequence(SubstitutionSequence substitutionSequence)
        {
            if (_substitutionSequences.Any(subSeq => subSeq.Substitutions.Count != substitutionSequence.Substitutions.Count))
            {
                throw new System.InvalidOperationException("Every sequence should have the same number of substitutions.");
            }

            _substitutionSequences.Add(substitutionSequence);
        }

        internal void AddSubstitutionToEachSequences(Substitution substitution)
        {
            _substitutionSequences.ForEach(substitutionSequence => substitutionSequence.AddSubstitution(substitution));
        }

        public override string ToString()
        {
            return string.Join(" | ", SubstitutionSequences);
        }
    }
}
