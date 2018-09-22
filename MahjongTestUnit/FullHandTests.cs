using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MahjongDll.Pivot;
using System.Linq;

namespace MahjongTestUnit
{
    [TestClass]
    public class FullHandTests
    {
        private Random _randomizer = new Random(DateTime.Now.Millisecond);

        [TestMethod]
        public void TestBasicValidHand()
        {
            List<TilePivot> tiles = new List<TilePivot>
            {
                new TilePivot(DragonPivot.red),
                new TilePivot(DragonPivot.red),
                new TilePivot(FamilyPivot.bamboo, 4),
                new TilePivot(FamilyPivot.bamboo, 5),
                new TilePivot(FamilyPivot.bamboo, 6),
                new TilePivot(FamilyPivot.bamboo, 5),
                new TilePivot(FamilyPivot.bamboo, 6),
                new TilePivot(FamilyPivot.bamboo, 7),
                new TilePivot(FamilyPivot.circle, 3),
                new TilePivot(FamilyPivot.circle, 3),
                new TilePivot(FamilyPivot.circle, 1),
                new TilePivot(FamilyPivot.circle, 2),
                new TilePivot(FamilyPivot.circle, 3)
            };
            tiles = tiles.OrderBy(x => _randomizer.Next()).ToList();

            FullHandPivot handPivot = new FullHandPivot(tiles, WindPivot.east, WindPivot.east, new TilePivot(DragonPivot.red));

            List<List<YakuPivot>> groupsOfYakus = handPivot.ComputeHandYakus();

            Assert.IsNotNull(groupsOfYakus);
            // Assert.IsTrue(yakus.Contains(YakuPivot.Get(YakuPivot.Yakuhai)));
        }

        [TestMethod]
        public void TestChitoiTanYaoTsumoHand()
        {
            List<TilePivot> tiles = new List<TilePivot>
            {
                new TilePivot(FamilyPivot.character, 2),
                new TilePivot(FamilyPivot.character, 2),
                new TilePivot(FamilyPivot.character, 5),
                new TilePivot(FamilyPivot.character, 5),
                new TilePivot(FamilyPivot.character, 6),
                new TilePivot(FamilyPivot.character, 6),
                new TilePivot(FamilyPivot.bamboo, 4),
                new TilePivot(FamilyPivot.bamboo, 4),
                new TilePivot(FamilyPivot.bamboo, 8),
                new TilePivot(FamilyPivot.bamboo, 8),
                new TilePivot(FamilyPivot.circle, 5),
                new TilePivot(FamilyPivot.circle, 5),
                new TilePivot(FamilyPivot.circle, 8)
            };
            tiles = tiles.OrderBy(x => _randomizer.Next()).ToList();

            FullHandPivot handPivot = new FullHandPivot(tiles, WindPivot.east, WindPivot.east, new TilePivot(FamilyPivot.circle, 8));

            List<List<YakuPivot>> groupsOfYakus = handPivot.ComputeHandYakus();

            Assert.IsNotNull(groupsOfYakus);
            /*Assert.IsTrue(yakus.Contains(YakuPivot.Get(YakuPivot.Chiitoitsu)));
            Assert.IsTrue(yakus.Contains(YakuPivot.Get(YakuPivot.Tanyao)));
            Assert.IsTrue(yakus.Contains(YakuPivot.Get(YakuPivot.MenzenTsumo)));*/
        }

        [TestMethod]
        public void TestFullHonnorsAndFullPons()
        {
            List<TilePivot> tiles = new List<TilePivot>
            {
                new TilePivot(DragonPivot.green),
                new TilePivot(DragonPivot.green),
                new TilePivot(DragonPivot.green),
                new TilePivot(DragonPivot.red),
                new TilePivot(DragonPivot.red),
                new TilePivot(WindPivot.east),
                new TilePivot(WindPivot.east),
                new TilePivot(WindPivot.east),
                new TilePivot(WindPivot.north),
                new TilePivot(WindPivot.north),
                new TilePivot(WindPivot.north),
                new TilePivot(DragonPivot.white),
                new TilePivot(DragonPivot.white)
            };
            tiles = tiles.OrderBy(x => _randomizer.Next()).ToList();

            FullHandPivot handPivot = new FullHandPivot(tiles, WindPivot.east, WindPivot.north, new TilePivot(DragonPivot.red));

            List<List<YakuPivot>> groupsOfYakus = handPivot.ComputeHandYakus();

            Assert.IsNotNull(groupsOfYakus);
            /*Assert.AreEqual(4, yakus.Count(y => y.Name == YakuPivot.Yakuhai));
            Assert.IsTrue(yakus.Contains(YakuPivot.Get(YakuPivot.Toitoi)));
            Assert.IsTrue(yakus.Contains(YakuPivot.Get(YakuPivot.Sanankou)));
            Assert.IsTrue(yakus.Contains(YakuPivot.Get(YakuPivot.Tsuuiisou)));
            Assert.IsTrue(yakus.Contains(YakuPivot.Get(YakuPivot.Suuankou)));
            Assert.IsTrue(yakus.Contains(YakuPivot.Get(YakuPivot.Honitsu)));
            Assert.IsTrue(yakus.Contains(YakuPivot.Get(YakuPivot.Shousangen)));
            Assert.IsTrue(yakus.Contains(YakuPivot.Get(YakuPivot.Honroutou)));
            Assert.IsTrue(yakus.Contains(YakuPivot.Get(YakuPivot.Chantaiyao)));*/
        }

        [TestMethod]
        public void Test_Ryanpeikou_Pinfu_Chinitsu_Junchantaiyao_NoChiitoitsu()
        {
            List<TilePivot> tiles = new List<TilePivot>
            {
                new TilePivot(FamilyPivot.character, 1),
                new TilePivot(FamilyPivot.character, 2),
                new TilePivot(FamilyPivot.character, 2),
                new TilePivot(FamilyPivot.character, 3),
                new TilePivot(FamilyPivot.character, 3),
                new TilePivot(FamilyPivot.character, 7),
                new TilePivot(FamilyPivot.character, 7),
                new TilePivot(FamilyPivot.character, 8),
                new TilePivot(FamilyPivot.character, 8),
                new TilePivot(FamilyPivot.character, 9),
                new TilePivot(FamilyPivot.character, 9),
                new TilePivot(FamilyPivot.character, 9),
                new TilePivot(FamilyPivot.character, 9),
            };
            tiles = tiles.OrderBy(x => _randomizer.Next()).ToList();

            FullHandPivot handPivot = new FullHandPivot(tiles, WindPivot.east, WindPivot.north, new TilePivot(FamilyPivot.character, 1));

            List<List<YakuPivot>> groupsOfYakus = handPivot.ComputeHandYakus();

            Assert.IsNotNull(groupsOfYakus);
            /*Assert.IsTrue(yakus.Contains(YakuPivot.Get(YakuPivot.Chinitsu)));
            Assert.IsTrue(yakus.Contains(YakuPivot.Get(YakuPivot.Honitsu)));
            Assert.IsTrue(yakus.Contains(YakuPivot.Get(YakuPivot.Pinfu)));
            Assert.IsTrue(yakus.Contains(YakuPivot.Get(YakuPivot.Ryanpeikou)));
            Assert.IsTrue(yakus.Contains(YakuPivot.Get(YakuPivot.Iipeikou)));
            Assert.IsFalse(yakus.Contains(YakuPivot.Get(YakuPivot.Chiitoitsu))); // nug
            Assert.IsTrue(yakus.Contains(YakuPivot.Get(YakuPivot.Chantaiyao)));
            Assert.IsTrue(yakus.Contains(YakuPivot.Get(YakuPivot.Junchantaiyao)));*/
        }
    }
}
