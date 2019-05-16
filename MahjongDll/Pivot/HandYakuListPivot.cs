using System;
using System.Collections.Generic;
using System.Linq;

namespace MahjongDll.Pivot
{
    /// <summary>
    /// Represents a list of <see cref="YakuPivot"/> in a hand.
    /// </summary>
    public class HandYakuListPivot
    {
        #region Embedded properties

        private readonly List<YakuPivot> _yakus;

        /// <summary>
        /// List of <see cref="YakuPivot"/>.
        /// </summary>
        public IReadOnlyCollection<YakuPivot> Yakus
        {
            get { return _yakus; }
        }

        /// <summary>
        /// Indicates if the hand is concealed or not.
        /// </summary>
        public bool ConcealedHand { get; private set; }

        #endregion Embedded properties

        #region Inferred properties

        /// <summary>
        /// Official fans count.
        /// </summary>
        /// <remarks>No kazoe yakuman.</remarks>
        public int OfficialFansCount { get { return Yakuman ? 13 : (TotalFansCount > 12 ? 12 : TotalFansCount); } }

        /// <summary>
        /// Total fans count.
        /// </summary>
        public int TotalFansCount { get { return _yakus.Sum(y => ConcealedHand ? y.FansConcealed : y.FansOpen); } }

        /// <summary>
        /// Indicates if the hand is yakuman.
        /// </summary>
        public bool Yakuman { get { return _yakus.Any(y => y.Yakuman); } }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="yakus"><see cref="Yakus"/></param>
        /// <param name="concealedHand"><see cref="ConcealedHand"/></param>
        /// <exception cref="ArgumentNullException"><paramref name="yakus"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidYakusCountError"/></exception>
        public HandYakuListPivot(List<YakuPivot> yakus, bool concealedHand)
        {
            if (yakus == null)
            {
                throw new ArgumentNullException(nameof(yakus));
            }

            if (yakus.Count == 0 || yakus.Any(y => y == null))
            {
                throw new ArgumentException(Messages.InvalidYakusCountError, nameof(yakus));
            }

            _yakus = new List<YakuPivot>(yakus);
            ConcealedHand = concealedHand;
        }
    }
}
