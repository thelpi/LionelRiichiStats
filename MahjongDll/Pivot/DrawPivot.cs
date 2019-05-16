using System;
using System.Collections.Generic;
using System.Linq;

namespace MahjongDll.Pivot
{
    /// <summary>
    /// Represents every <see cref="TilePivot"/> of the game.
    /// </summary>
    public class DrawPivot
    {
        #region Embedded properties

        // Randomizer.
        private Random _randomizer = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// List of <see cref="TilePivot"/>.
        /// </summary>
        public IReadOnlyCollection<TilePivot> Tiles { get; private set; }

        #endregion Embedded properties

        #region Inferred properties

        /// <summary>
        /// Inferred; List of unique <see cref="TilePivot"/>.
        /// </summary>
        public IReadOnlyCollection<TilePivot> UniqueTiles { get { return Tiles.Distinct().ToList(); } }

        #endregion Inferred properties

        /// <summary>
        /// Constructor.
        /// </summary>
        public DrawPivot()
        {
            List<TilePivot> tiles = new List<TilePivot>();

            for (int i = 0; i < 4; i++)
            {
                tiles.Add(new TilePivot(DragonPivot.Red));
                tiles.Add(new TilePivot(DragonPivot.White));
                tiles.Add(new TilePivot(DragonPivot.Green));
                tiles.Add(new TilePivot(WindPivot.East));
                tiles.Add(new TilePivot(WindPivot.South));
                tiles.Add(new TilePivot(WindPivot.West));
                tiles.Add(new TilePivot(WindPivot.North));
                for (int j = 1; j <= 9; j++)
                {
                    tiles.Add(new TilePivot(FamilyPivot.Character, j));
                    tiles.Add(new TilePivot(FamilyPivot.Circle, j));
                    tiles.Add(new TilePivot(FamilyPivot.Bamboo, j));
                }
            }

            tiles.Sort();

            Tiles = tiles;
        }

        /// <summary>
        /// Picks an hand of 14 tiles.
        /// </summary>
        /// <returns>List of <see cref="TilePivot"/>.</returns>
        public IReadOnlyCollection<TilePivot> PickHandOfTiles()
        {
            List<TilePivot> picks = new List<TilePivot>();
            List<int> indexes = new List<int>();
            for (int i = 0; i < 14; i++)
            {
                bool added = false;
                do
                {
                    int nextIndex = _randomizer.Next(0, Tiles.Count);
                    if (!indexes.Contains(nextIndex))
                    {
                        picks.Add(Tiles.ElementAt(nextIndex));
                        indexes.Add(nextIndex);
                        added = true;
                    }
                }
                while (!added);
            }
            picks.Sort();
            return picks;
        }

        /// <summary>
        /// Computes tiles of the instance not included in the specified hand.
        /// </summary>
        /// <param name="handTiles">List of <see cref="TilePivot"/> in the hand.</param>
        /// <param name="unique"><c>True</c> to remove duplicates from the output.</param>
        /// <returns>List of remaining <see cref="TilePivot"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="handTiles"/></exception>
        public List<TilePivot> ComputeRemainingTiles(List<TilePivot> handTiles, bool unique)
        {
            if (handTiles == null)
            {
                throw new ArgumentNullException(nameof(handTiles));
            }

            List<TilePivot> tiles = new List<TilePivot>(Tiles);
            foreach (TilePivot tile in handTiles)
            {
                tiles.RemoveAt(tiles.LastIndexOf(tile));
            }

            return unique ? tiles.Distinct().ToList() : tiles;
        }
    }
}
