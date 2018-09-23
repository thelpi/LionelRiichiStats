using System;
using System.Collections.Generic;
using System.Linq;

namespace MahjongDll.Pivot
{
    /*
    /// <summary>
    /// Represents the points of a complete hand.
    /// </summary>
    public class HandPointPivot
    {
        #region Embedded properties

        // The east hand.
        private FullHandPivot _handEast;
        // The south hand.
        private FullHandPivot _handSouth;
        // The west hand.
        private FullHandPivot _handWest;
        // The north hand.
        private FullHandPivot _handNorth;
        // List of dora indicators.
        private List<TilePivot> _doraMarks;
        // List of ura-dora indicators.
        private List<TilePivot> _uradoraMarks;
        // Indicates the wind of the player who gaves the ron (if ron).
        private WindPivot? _ronOn;

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
        /// 
        /// </summary>
        /// <param name="eastHand"></param>
        /// <param name="southHand"></param>
        /// <param name="westHand"></param>
        /// <param name="northHand"></param>
        /// <param name="doraMarks">List of dora indicators.</param>
        /// <param name="uradoraMarks">List of ura-dora indicators.</param>
        /// <param name="_ronOn">Indicates the wind of the player who gaves the ron (if ron).</param>
        public HandPointPivot(FullHandPivot eastHand, FullHandPivot southHand,
            FullHandPivot westHand, FullHandPivot northHand,
            List<TilePivot> doraMarks, List<TilePivot> uradoraMarks, WindPivot? _ronOn = null)
        {
            _handEast = eastHand ?? throw new ArgumentNullException(nameof(eastHand));
            _handSouth = southHand ?? throw new ArgumentNullException(nameof(southHand));
            _handWest = westHand ?? throw new ArgumentNullException(nameof(westHand));
            _handNorth = northHand ?? throw new ArgumentNullException(nameof(northHand));
            _doraMarks = doraMarks ?? new List<TilePivot>();
            _uradoraMarks = _uradoraMarks ?? new List<TilePivot>();

            if (_doraMarks.Count > 4 || _uradoraMarks.Count > 4)
            {
                throw new ArgumentException(Messages.InvalidDorasCountError);
            }

            // List<List<YakuPivot>> yakus = _hand.ComputeHandYakus();
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

        /// <summary>
        /// Gets a <see cref="string"/> representation of the instance.
        /// </summary>
        /// <returns><see cref="string"/> representation.</returns>
        public override string ToString()
        {
            return base.ToString();
        }
    }
    */
}
