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

        /// <summary>
        /// Draw every tiles.
        /// </summary>
        /// <param name="randomize">Set <c>True</c> to randomize the draw.</param>
        /// <param name="oneOfEach">Set <c>True</c> to get one of each instead of four.</param>
        public DrawPivot(bool randomize, bool oneOfEach)
        {
            List<TilePivot> tiles = new List<TilePivot>();

            for (int i = 0; i < (oneOfEach ? 1 : 4); i++)
            {
                tiles.Add(new TilePivot(DragonPivot.red));
                tiles.Add(new TilePivot(DragonPivot.white));
                tiles.Add(new TilePivot(DragonPivot.green));
                tiles.Add(new TilePivot(WindPivot.east));
                tiles.Add(new TilePivot(WindPivot.south));
                tiles.Add(new TilePivot(WindPivot.west));
                tiles.Add(new TilePivot(WindPivot.north));
                for (int j = 1; j <= 9; j++)
                {
                    tiles.Add(new TilePivot(FamilyPivot.character, j));
                    tiles.Add(new TilePivot(FamilyPivot.circle, j));
                    tiles.Add(new TilePivot(FamilyPivot.bamboo, j));
                }
            }

            if (randomize)
            {
                tiles = tiles.OrderBy(x => _randomizer.NextDouble()).ToList();
            }

            Tiles = tiles;
        }
    }
}
