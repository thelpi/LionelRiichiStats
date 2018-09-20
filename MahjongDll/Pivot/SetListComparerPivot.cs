using System.Collections.Generic;
using System.Linq;

namespace MahjongDll.Pivot
{
    /// <summary>
    /// Compares two list of <see cref="SetPivot"/>.
    /// </summary>
    /// <seealso cref="IEqualityComparer"/>.
    public class SetListComparerPivot : IEqualityComparer<List<SetPivot>>
    {
        /// <summary>
        /// Checks if two lists of <see cref="SetPivot"/> are equals.
        /// </summary>
        /// <param name="x">First list of <see cref="SetPivot"/>.</param>
        /// <param name="y">Second list of <see cref="SetPivot"/>.</param>
        /// <returns><c>True</c> if lists are equals, <c>False</c> otherwise.</returns>
        public bool Equals(List<SetPivot> x, List<SetPivot> y)
        {
            if (x == null)
            {
                return y == null;
            }

            if (x.Count != y.Count || x.Contains(null) || y.Contains(null))
            {
                return false;
            }

            return x.All(xSet => y.Any(ySet => ySet.IsSame(xSet)));
        }

        /// <summary>
        /// Computes an hashcode for a list of <see cref="SetPivot"/>.
        /// </summary>
        /// <remarks>This implementation might be not reliable.</remarks>
        /// <param name="setsList">List of <see cref="SetPivot"/>.</param>
        /// <returns>The hashcode.</returns>
        public int GetHashCode(List<SetPivot> setsList) =>
            base.GetHashCode(); // TODO : finds a better implementation.
    }
}
