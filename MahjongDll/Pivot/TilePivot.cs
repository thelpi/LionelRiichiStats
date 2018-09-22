using System;

namespace MahjongDll.Pivot
{
    /// <summary>
    /// Represents a tile.
    /// </summary>
    /// <seealso cref="IComparable{T}"/>.
    /// <seealso cref="IEquatable{T}"/>.
    public class TilePivot : IComparable<TilePivot>, IEquatable<TilePivot>
    {
        #region Embedded properties

        /// <summary>
        /// The <see cref="FamilyPivot"/> type.
        /// </summary>
        public FamilyPivot Family { get; private set; }
        /// <summary>
        /// The <see cref="DragonPivot"/> type, if applicable.
        /// </summary>
        public DragonPivot? Dragon { get; private set; }
        /// <summary>
        /// The <see cref="WindPivot"/> type, if applicable.
        /// </summary>
        public WindPivot? Wind { get; private set; }
        /// <summary>
        /// The number between 1 and 9. 0 for dragons and winds.
        /// </summary>
        public int Number { get; private set; }

        #endregion

        #region Inferred properties

        /// <summary>
        /// Gtes if the tile is a dragon.
        /// </summary>
        public bool IsDragon =>
            Family == FamilyPivot.dragon;
        /// <summary>
        /// Gets if the tile is a wind.
        /// </summary>
        public bool IsWind =>
            Family == FamilyPivot.wind;
        /// <summary>
        /// Gets if the tile is a wind or a dragon.
        /// </summary>
        public bool IsHonor =>
            IsDragon || IsWind;
        /// <summary>
        /// Gets if the tile is a terminal (1 or 9 <see cref="Number"/>).
        /// </summary>
        public bool IsTerminal =>
            Number == 1 || Number == 9;
        /// <summary>
        /// Gets if the tile is an honor or a terminal.
        /// </summary>
        public bool IsTerminalOrHonor =>
            IsHonor || IsTerminal;

        #endregion Inferred properties

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="family"><see cref="FamilyPivot"/>.</param>
        /// <param name="number">Number between 1 and 9.</param>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidTileFamilyError"/></exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidTileFamilyError"/></exception>
        public TilePivot(FamilyPivot family, int number)
        {
            if (family == FamilyPivot.dragon || family == FamilyPivot.wind)
            {
                throw new ArgumentException(Messages.InvalidTileFamilyError, nameof(family));
            }
            if (number < 1 || number > 9)
            {
                throw new ArgumentException(Messages.InvalidTileFamilyError, nameof(number));
            }

            Number = number;
            Family = family;
        }

        /// <summary>
        /// Dragon family constructor.
        /// </summary>
        /// <param name="dragon"><see cref="DragonPivot"/>.</param>
        public TilePivot(DragonPivot dragon)
        {
            Dragon = dragon;
            Family = FamilyPivot.dragon;
        }

        /// <summary>
        /// Wind family constructor.
        /// </summary>
        /// <param name="wind"><see cref="WindPivot"/>.</param>
        public TilePivot(WindPivot wind)
        {
            Wind = wind;
            Family = FamilyPivot.wind;
        }

        /// <summary>
        /// <see cref="IComparable"/> implementation.
        /// </summary>
        /// <param name="other"><see cref="TilePivot"/> to compare with the current instance.</param>
        /// <returns>-1 if the current tile is before <paramref name="other"/>, 1 if after, 0 if same.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <c>Null</c>.</exception>
        public int CompareTo(TilePivot other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (Family != other.Family)
            {
                return Family.CompareTo(other.Family);
            }

            if (Dragon.HasValue)
            {
                return Dragon.Value.CompareTo(other.Dragon.Value);
            }

            if (Wind.HasValue)
            {
                return Wind.Value.CompareTo(other.Wind.Value);
            }

            return Number.CompareTo(other.Number);
        }

        /// <summary>
        /// Checks the equality between this instance and a second <see cref="TilePivot"/>.
        /// </summary>
        /// <param name="other">The second <see cref="TilePivot"/>.</param>
        /// <returns><c>True</c> if instances are equals, <c>False</c> otherwise.</returns>
        public bool Equals(TilePivot other) =>
            other != null
            && other.Family == Family
            && other.Number == Number
            && other.Wind == Wind
            && other.Dragon == Dragon;

        /// <summary>
        /// Computes an hashcode for the tile.
        /// </summary>
        /// <returns>The tile's hashcode.</returns>
        public override int GetHashCode() =>
            (int)Family * (11 + Number) * (17 + (Wind.HasValue ? (int)Wind : -1)) * (23 + (Dragon.HasValue ? (int)Dragon : -1));

        /// <summary>
        /// Gets a <see cref="string"/> representation of the tile.
        /// </summary>
        /// <returns><see cref="String"/> which represents the tile.</returns>
        public override string ToString() =>
            string.Format("{0} - {1}", Family,
                IsWind ? Wind.Value.ToString() :
                    (IsDragon ? Dragon.Value.ToString() :
                        Number.ToString()));
    }
}
