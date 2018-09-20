using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahjongDll.Pivot
{
    public class DrawPivot
    {
        private List<TilePivot> _tiles = new List<TilePivot>();
        private Random _randomizer = new Random(DateTime.Now.Millisecond);

        public DrawPivot()
        {
            for (int i = 0; i < 4; i++)
            {
                _tiles.Add(new TilePivot(DragonPivot.red));
                _tiles.Add(new TilePivot(DragonPivot.white));
                _tiles.Add(new TilePivot(DragonPivot.green));
                _tiles.Add(new TilePivot(WindPivot.east));
                _tiles.Add(new TilePivot(WindPivot.south));
                _tiles.Add(new TilePivot(WindPivot.west));
                _tiles.Add(new TilePivot(WindPivot.north));
                for (int j = 1; j <= 9; j++)
                {
                    _tiles.Add(new TilePivot(FamilyPivot.character, j));
                    _tiles.Add(new TilePivot(FamilyPivot.circle, j));
                    _tiles.Add(new TilePivot(FamilyPivot.bamboo, j));
                }
            }
            _tiles = _tiles.OrderBy(x => _randomizer.NextDouble()).ToList();
        }
    }
}
