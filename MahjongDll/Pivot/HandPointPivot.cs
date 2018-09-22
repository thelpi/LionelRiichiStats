using System;
using System.Collections.Generic;
using System.Linq;

namespace MahjongDll.Pivot
{
    /// <summary>
    /// Represents the points of a complete hand.
    /// </summary>
    public class HandPointPivot
    {
        #region Embedded properties

        /// <summary>
        /// List of yakus. Must be checked for errors by the caller.
        /// </summary>
        public IReadOnlyCollection<YakuPivot> Yakus { get; private set; }
        /// <summary>
        /// Indicates the current wind  of the winner.
        /// </summary>
        public WindPivot WinnerWind { get; private set; }
        /// <summary>
        /// Indicates the wind of the opponent which has got ron. <c>Null</c> for tsumo.
        /// </summary>
        public WindPivot? LoserWind { get; private set; }
        /// <summary>
        /// Indicates if the hand is open.
        /// </summary>
        public bool IsOpen { get; private set; }
        /// <summary>
        /// Number of doras (any type).
        /// </summary>
        public int Doras { get; private set; }
        /// <summary>
        /// Indicates if the hand contains a pair of dragons or winds (dominant or turn).
        /// </summary>
        public bool HasPairOfHonnor { get; private set; }
        /// <summary>
        /// Indicates if the hand has been complete on a chi double wait.
        /// </summary>
        public bool FinishOnChiDoubleWait { get; private set; }
        /// <summary>
        /// Number of concealed pons (not honor either terminal).
        /// </summary>
        public int ConcealedRegularPons { get; private set; }
        /// <summary>
        /// Number of concealed kans (not honor either terminal).
        /// </summary>
        public int ConcealedRegularKans { get; private set; }
        /// <summary>
        /// Number of concealed pons (honor or terminal).
        /// </summary>
        public int ConcealedTerminalOrHonorPons { get; private set; }
        /// <summary>
        /// Number of concealed kans (honor or terminal).
        /// </summary>
        public int ConcealedTerminalOrHonorKans { get; private set; }
        /// <summary>
        /// Number of opened pons (not honor either terminal).
        /// </summary>
        public int OpenedRegularPons { get; private set; }
        /// <summary>
        /// Number of opened kans (not honor either terminal).
        /// </summary>
        public int OpenedRegularKans { get; private set; }
        /// <summary>
        /// Number of opened pons (honor or terminal).
        /// </summary>
        public int OpenedTerminalOrHonorPons { get; private set; }
        /// <summary>
        /// Number of opened kans (honor or terminal).
        /// </summary>
        public int OpenedTerminalOrHonorKans { get; private set; }

        #endregion Embedded properties

        #region Computed properties

        private int? _fans;
        private int? _minipoints;

        /// <summary>
        /// Number of fans.
        /// </summary>
        public int Fans
        {
            get
            {
                if (!_fans.HasValue)
                {
                    _fans = Yakus.Sum(y => IsOpen ? y.FansOpen : y.FansConcealed) + Doras;
                    _fans = _fans > 13 ? 13 : _fans;
                }

                return _fans.Value;
            }
        }

        /// <summary>
        /// Number of minipoints.
        /// </summary>
        public int Minipoints
        {
            get
            {
                if (!_minipoints.HasValue)
                {
                    _minipoints = Yakus.Contains(YakuPivot.Get(YakuPivot.Chiitoitsu)) ? 25 : (!IsOpen && LoserWind.HasValue ? 30 : 20);

                    _minipoints += 32 * ConcealedTerminalOrHonorKans;
                    _minipoints += 16 * ConcealedRegularKans;
                    _minipoints += 16 * OpenedTerminalOrHonorKans;
                    _minipoints += 8 * OpenedRegularKans;
                    _minipoints += 8 * ConcealedTerminalOrHonorPons;
                    _minipoints += 4 * ConcealedRegularPons;
                    _minipoints += 4 * OpenedTerminalOrHonorPons;
                    _minipoints += 2 * OpenedRegularPons;
                    if (HasPairOfHonnor)
                    {
                        _minipoints += 2;
                    }
                    if (IsOpen || !FinishOnChiDoubleWait)
                    {
                        _minipoints += 2;
                    }
                }

                return _minipoints.Value;
            }
        }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="yakus">List of yakus. Must be checked for errors by the caller.</param>
        /// <exception cref="ArgumentNullException"><paramref name="yakus"/> is <c>Null</c>.</exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidYakusListError"/></exception>
        public HandPointPivot(List<YakuPivot> yakus)
        {
            if (yakus == null)
            {
                throw new ArgumentNullException(nameof(yakus));
            }

            if (yakus.Count == 0)
            {
                throw new ArgumentException(Messages.InvalidYakusListError, nameof(yakus));
            }

            Yakus = yakus;
        }

        /// <summary>
        /// Gets the number of points lost or won by the specified <see cref="WindPivot"/>.
        /// </summary>
        /// <param name="wind">The <see cref="WindPivot"/>.</param>
        /// <returns>Points imapct for the specified wind.</returns>
        public int GetPointsImpactFor(WindPivot wind)
        {
            bool pointsForWinner = WinnerWind == wind;
            bool eastWinner = WinnerWind == WindPivot.east;

            if (LoserWind.HasValue)
            {
                if (LoserWind != wind && !pointsForWinner)
                {
                    return 0;
                }
                int points = eastWinner ? PointRulePivot.GetPointsEastRon(Fans, Minipoints) :
                    PointRulePivot.GetPointsOtherRon(Fans, Minipoints);
                return pointsForWinner ? points : -points;
            }

            if (eastWinner)
            {
                int points = PointRulePivot.GetPointsEastTsumo(Fans, Minipoints);
                return pointsForWinner ? points * 3 : -points;
            }
            else
            {
                Tuple<int, int> pointsTuple = PointRulePivot.GetPointsOtherTsumo(Fans, Minipoints);
                return pointsForWinner ? pointsTuple.Item1 + (pointsTuple.Item2 * 2) :
                    -(wind == WindPivot.east ? pointsTuple.Item1 : pointsTuple.Item2);
            }
        }
    }
}
