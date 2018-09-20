using System;
using System.Collections.Generic;
using System.Linq;

namespace MahjongDll.Pivot
{
    /// <summary>
    /// Represents a valid set of <see cref="TilePivot"/>.
    /// </summary>
    public class SetPivot
    {
        #region Embedded properties

        /// <summary>
        /// Read-only list of <see cref="TilePivot"/> which form the set.
        /// </summary>
        public IReadOnlyCollection<TilePivot> Tiles { get; private set; }

        #endregion Embedded properties

        #region Inferred properties

        /// <summary>
        /// Indicates the family type of the set.
        /// </summary>
        public FamilyPivot Family => Tiles.First().Family;
        /// <summary>
        /// Indicates the first number of the set sequence, if applicable.
        /// </summary>
        public int FirstNumber => Tiles.First().Number;
        /// <summary>
        /// Indicates if the set is a kan.
        /// </summary>
        public bool IsKan => Tiles.Count == 4;
        /// <summary>
        /// Indicates if the set is a pair.
        /// </summary>
        public bool IsPair => Tiles.Count == 2;
        /// <summary>
        /// Indicates if the set is a pon.
        /// </summary>
        public bool IsPon => Tiles.Count == 3 && Tiles.All(t => t.Equals(Tiles.First()));
        /// <summary>
        /// Indicates if the set is a pon or a kan.
        /// </summary>
        public bool IsPonOrKan => IsKan || IsPon;
        /// <summary>
        /// Indicates if the set is a chi.
        /// </summary>
        public bool IsChi => !IsPonOrKan && !IsPair;
        /// <summary>
        /// Indicates if the set is made of dragons.
        /// </summary>
        public bool IsDragon => Tiles.First().Dragon.HasValue;
        /// <summary>
        /// Indicates if the set is made of winds.
        /// </summary>
        public bool IsWind => Tiles.First().Wind.HasValue;
        /// <summary>
        /// Indicates if the set is made of honors (dragons or winds).
        /// </summary>
        public bool IsHonor => IsDragon || IsWind;
        /// <summary>
        /// Indicates if the set is made of terminals (1, 9).
        /// </summary>
        public bool IsTerminal => !IsChi && (Tiles.First().Number == 1 || Tiles.First().Number == 9);
        /// <summary>
        /// Indicates if the set is made of honors or terminals.
        /// </summary>
        public bool IsHonorOrTerminal => IsTerminal || IsHonor;
        /// <summary>
        /// Indicates if the set is a chi and contains a terminal (1, 9).
        /// </summary>
        public bool IsTerminalChi => IsChi && (Tiles.First().Number == 1 || Tiles.Last().Number == 9);
        /// <summary>
        /// Indicates the <see cref="DragonPivot"/> of the set, if applicable.
        /// </summary>
        public DragonPivot? Dragon => Tiles.First().Dragon;
        /// <summary>
        /// Indicates the <see cref="WindPivot"/> of the set, if applicable.
        /// </summary>
        public WindPivot? Wind => Tiles.First().Wind;

        #endregion Inferred properties

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tilesArray">Array of <see cref="TilePivot"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="tilesArray"/> is <c>Null</c>.</exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidTilesCountError"/>.</exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidTwoOrFourTilesSetError"/>.</exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidTilesFamilyError"/>.</exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidTilesSequenceError"/>.</exception>
        public SetPivot(params TilePivot[] tilesArray)
        {
            if (tilesArray == null)
            {
                throw new ArgumentNullException(nameof(tilesArray));
            }

            int[] allowed = { 2, 3, 4 };
            if (!allowed.Contains(tilesArray.Length))
            {
                throw new ArgumentException(Messages.InvalidTilesCountError, nameof(tilesArray));
            }

            Tiles = tilesArray.OrderBy(t => t).ToList();

            bool allSame = Tiles.All(t => t.Equals(Tiles.First()));

            if ((Tiles.Count == 2 || Tiles.Count == 4) && !allSame)
            {
                throw new ArgumentException(Messages.InvalidTwoOrFourTilesSetError, nameof(tilesArray));
            }

            if (Tiles.Count == 3 && !allSame)
            {
                if (!Tiles.All(t => t.Family == Tiles.First().Family))
                {
                    throw new ArgumentException(Messages.InvalidTilesFamilyError, nameof(tilesArray));
                }
                else
                {
                    if (Tiles.First().Number != (Tiles.ElementAt(1).Number - 1)
                        || Tiles.ElementAt(1).Number != (Tiles.ElementAt(2).Number - 1))
                    {
                        throw new ArgumentException(Messages.InvalidTilesSequenceError, nameof(tilesArray));
                    }
                }
            }
        }

        /// <summary>
        /// Compares two sets for exact similtude.
        /// </summary>
        /// <param name="other">Other <see cref="SetPivot"/>.</param>
        /// <returns><c>True</c> if the two <see cref="SetPivot"/> are identical, <c>False</c> otherwise.</returns>
        internal bool IsSame(SetPivot other) =>
            other != null
            && Family == other.Family
            && FirstNumber == other.FirstNumber
            && IsKan == other.IsKan
            && IsChi == other.IsChi
            && IsPon == other.IsPon
            && IsPair == other.IsPair;

        /// <summary>
        /// Provides a <see cref="string"/> representation of this instance.
        /// </summary>
        /// <returns><see cref="String"/> representation of this instance.</returns>
        public override string ToString() =>
            string.Format("{0} - {1}", Family,
                (IsWind ? Tiles.First().Wind.Value.ToString() :
                    (IsDragon ? Tiles.First().Dragon.Value.ToString() :
                        FirstNumber.ToString())));

        /// <summary>
        /// Checks if a list of <see cref="TilePivot"/> can form a valid set.
        /// </summary>
        /// <param name="tilesArray">List of <see cref="TilePivot"/>.</param>
        /// <returns><c>True</c> if can form a valid set, <c>False</c> otherwise.</returns>
        public static bool IsValidSet(params TilePivot[] tilesArray)
        {
            if (tilesArray == null || tilesArray.Length < 2 || tilesArray.Length > 4)
            {
                return false;
            }

            bool allEqual = tilesArray.All(t => t.Equals(tilesArray.First()));
            if (tilesArray.Length == 2 || tilesArray.Length == 4)
            {
                return allEqual;
            }

            tilesArray = tilesArray.OrderBy(t => t.Number).ToArray();
            return allEqual || (
                tilesArray.All(t => t.Family == tilesArray.First().Family)
                && !tilesArray.First().IsHonor
                && tilesArray.First().Number == tilesArray[1].Number - 1
                && tilesArray.First().Number == tilesArray[2].Number - 2
            );
        }
    }
}
