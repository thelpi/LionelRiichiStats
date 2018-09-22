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
        public void Test_DoubleYakuPai()
        {
            SetPivot openedSets = new SetPivot(new TilePivot(WindPivot.east),
                new TilePivot(WindPivot.east),
                new TilePivot(WindPivot.east));

            List<TilePivot> tiles = new List<TilePivot>
            {
                new TilePivot(FamilyPivot.bamboo, 4),
                new TilePivot(FamilyPivot.bamboo, 5),
                new TilePivot(FamilyPivot.bamboo, 6),
                new TilePivot(FamilyPivot.bamboo, 5),
                new TilePivot(FamilyPivot.bamboo, 6),
                new TilePivot(FamilyPivot.bamboo, 7),
                new TilePivot(FamilyPivot.circle, 3),
                new TilePivot(FamilyPivot.circle, 3),
                new TilePivot(FamilyPivot.circle, 1),
                new TilePivot(FamilyPivot.circle, 2)
            };
            tiles = tiles.OrderBy(x => _randomizer.Next()).ToList();

            FullHandPivot handPivot = new FullHandPivot(tiles, WindPivot.east, WindPivot.east,
                new TilePivot(FamilyPivot.circle, 3),
                openedSets: new List<SetPivot> { openedSets });

            List<List<YakuPivot>> groupsOfYakus = handPivot.ComputeHandYakus();

            Assert.IsNotNull(groupsOfYakus);
            Assert.AreEqual(1, groupsOfYakus.Count);
            Assert.IsNotNull(groupsOfYakus[0]);
            Assert.AreEqual(2, groupsOfYakus[0].Count);
            Assert.IsTrue(groupsOfYakus[0].All(y => y.Name == YakuPivot.Yakuhai));
        }

        [TestMethod]
        public void Test_Chitoi_TanYao_Tsumo_DoubleRiichi_Ippatsu()
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

            FullHandPivot handPivot = new FullHandPivot(tiles, WindPivot.east, WindPivot.east,
                new TilePivot(FamilyPivot.circle, 8), isRon: false, isDoubleRiichi: true, isIppatsu: true);

            List<List<YakuPivot>> groupsOfYakus = handPivot.ComputeHandYakus();

            Assert.IsNotNull(groupsOfYakus);
            Assert.AreEqual(1, groupsOfYakus.Count);
            Assert.IsNotNull(groupsOfYakus[0]);
            Assert.AreEqual(5, groupsOfYakus[0].Count);
            Assert.IsTrue(groupsOfYakus[0].Contains(YakuPivot.Get(YakuPivot.Chiitoitsu)));
            Assert.IsTrue(groupsOfYakus[0].Contains(YakuPivot.Get(YakuPivot.Tanyao)));
            Assert.IsTrue(groupsOfYakus[0].Contains(YakuPivot.Get(YakuPivot.MenzenTsumo)));
            Assert.IsTrue(groupsOfYakus[0].Contains(YakuPivot.Get(YakuPivot.DoubleRiichi)));
            Assert.IsTrue(groupsOfYakus[0].Contains(YakuPivot.Get(YakuPivot.Ippatsu)));
        }

        [TestMethod]
        public void Test_Sankantsu_Chankan_Honroutou_Shousangen_Honitsu_Sanankou_Toitoi_Yakuhai()
        {
            List<SetPivot> concealedKans = new List<SetPivot>
            {
                new SetPivot(new TilePivot(DragonPivot.white),
                    new TilePivot(DragonPivot.white),
                    new TilePivot(DragonPivot.white),
                    new TilePivot(DragonPivot.white)),
                new SetPivot(new TilePivot(DragonPivot.green),
                    new TilePivot(DragonPivot.green),
                    new TilePivot(DragonPivot.green),
                    new TilePivot(DragonPivot.green)),
                new SetPivot(new TilePivot(WindPivot.north),
                    new TilePivot(WindPivot.north),
                    new TilePivot(WindPivot.north),
                    new TilePivot(WindPivot.north))
            };
            List<TilePivot> tiles = new List<TilePivot>
            {
                new TilePivot(DragonPivot.red),
                new TilePivot(DragonPivot.red),
                new TilePivot(FamilyPivot.circle, 9),
                new TilePivot(FamilyPivot.circle, 9)
            };
            tiles = tiles.OrderBy(x => _randomizer.Next()).ToList();

            // Note : this is an impossible hand, as Chankan can be made on something else than a chi.
            FullHandPivot handPivot = new FullHandPivot(tiles, WindPivot.east, WindPivot.north,
                new TilePivot(FamilyPivot.circle, 9), concealedKans: concealedKans,
                isChankan: true, isRon: true);

            List<List<YakuPivot>> groupsOfYakus = handPivot.ComputeHandYakus();

            Assert.IsNotNull(groupsOfYakus);
            Assert.AreEqual(1, groupsOfYakus.Count);
            Assert.IsNotNull(groupsOfYakus[0]);
            Assert.AreEqual(10, groupsOfYakus[0].Count);
            Assert.AreEqual(3, groupsOfYakus[0].Count(y => y.Name == YakuPivot.Yakuhai));
            Assert.IsTrue(groupsOfYakus[0].Contains(YakuPivot.Get(YakuPivot.Toitoi)));
            Assert.IsTrue(groupsOfYakus[0].Contains(YakuPivot.Get(YakuPivot.Sanankou)));
            Assert.IsTrue(groupsOfYakus[0].Contains(YakuPivot.Get(YakuPivot.Honitsu)));
            Assert.IsTrue(groupsOfYakus[0].Contains(YakuPivot.Get(YakuPivot.Shousangen)));
            Assert.IsTrue(groupsOfYakus[0].Contains(YakuPivot.Get(YakuPivot.Honroutou)));
            Assert.IsTrue(groupsOfYakus[0].Contains(YakuPivot.Get(YakuPivot.Chankan)));
            Assert.IsTrue(groupsOfYakus[0].Contains(YakuPivot.Get(YakuPivot.Sankantsu)));
        }

        [TestMethod]
        public void Test_Ryanpeikou_Pinfu_Chinitsu_Junchantaiyao()
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

            FullHandPivot handPivot = new FullHandPivot(tiles, WindPivot.east, WindPivot.north,
                new TilePivot(FamilyPivot.character, 1), isRon: true);

            List<List<YakuPivot>> groupsOfYakus = handPivot.ComputeHandYakus();

            Assert.IsNotNull(groupsOfYakus);
            Assert.AreEqual(1, groupsOfYakus.Count);
            Assert.IsNotNull(groupsOfYakus[0]);
            Assert.AreEqual(4, groupsOfYakus[0].Count);
            Assert.IsTrue(groupsOfYakus[0].Contains(YakuPivot.Get(YakuPivot.Chinitsu)));
            Assert.IsTrue(groupsOfYakus[0].Contains(YakuPivot.Get(YakuPivot.Ryanpeikou)));
            Assert.IsTrue(groupsOfYakus[0].Contains(YakuPivot.Get(YakuPivot.Junchantaiyao)));
            Assert.IsTrue(groupsOfYakus[0].Contains(YakuPivot.Get(YakuPivot.Pinfu)));
            Assert.IsFalse(groupsOfYakus[0].Contains(YakuPivot.Get(YakuPivot.Chiitoitsu)));
            Assert.IsFalse(groupsOfYakus[0].Contains(YakuPivot.Get(YakuPivot.Honitsu)));
            Assert.IsFalse(groupsOfYakus[0].Contains(YakuPivot.Get(YakuPivot.Chantaiyao)));
        }

        [TestMethod]
        public void Test_KokushiMusou_Tenhou()
        {
            List<TilePivot> tiles = new List<TilePivot>
            {
                new TilePivot(FamilyPivot.character, 1),
                new TilePivot(FamilyPivot.character, 9),
                new TilePivot(FamilyPivot.circle, 1),
                new TilePivot(FamilyPivot.circle, 9),
                new TilePivot(FamilyPivot.bamboo, 1),
                new TilePivot(FamilyPivot.bamboo, 9),
                new TilePivot(WindPivot.east),
                new TilePivot(WindPivot.south),
                new TilePivot(WindPivot.north),
                new TilePivot(WindPivot.west),
                new TilePivot(DragonPivot.red),
                new TilePivot(DragonPivot.green),
                new TilePivot(DragonPivot.white),
                new TilePivot(DragonPivot.white)
            };
            tiles = tiles.OrderBy(x => _randomizer.Next()).ToList();

            FullHandPivot handPivot = new FullHandPivot(tiles, WindPivot.east, WindPivot.east);

            List<List<YakuPivot>> groupsOfYakus = handPivot.ComputeHandYakus();

            Assert.IsNotNull(groupsOfYakus);
            Assert.AreEqual(1, groupsOfYakus.Count);
            Assert.IsNotNull(groupsOfYakus[0]);
            Assert.AreEqual(2, groupsOfYakus[0].Count);
            Assert.IsTrue(groupsOfYakus[0].Contains(YakuPivot.Get(YakuPivot.Tenhou)));
            Assert.IsTrue(groupsOfYakus[0].Contains(YakuPivot.Get(YakuPivot.KokushiMusou)));
        }

        [TestMethod]
        public void Test_ChuurenPoutou_Chiihou()
        {
            List<TilePivot> tiles = new List<TilePivot>
            {
                new TilePivot(FamilyPivot.bamboo, 1),
                new TilePivot(FamilyPivot.bamboo, 1),
                new TilePivot(FamilyPivot.bamboo, 1),
                new TilePivot(FamilyPivot.bamboo, 2),
                new TilePivot(FamilyPivot.bamboo, 3),
                new TilePivot(FamilyPivot.bamboo, 4),
                new TilePivot(FamilyPivot.bamboo, 5),
                new TilePivot(FamilyPivot.bamboo, 6),
                new TilePivot(FamilyPivot.bamboo, 7),
                new TilePivot(FamilyPivot.bamboo, 8),
                new TilePivot(FamilyPivot.bamboo, 9),
                new TilePivot(FamilyPivot.bamboo, 9),
                new TilePivot(FamilyPivot.bamboo, 9),
                new TilePivot(FamilyPivot.bamboo, 3)
            };
            tiles = tiles.OrderBy(x => _randomizer.Next()).ToList();

            FullHandPivot handPivot = new FullHandPivot(tiles, WindPivot.east, WindPivot.north);

            List<List<YakuPivot>> groupsOfYakus = handPivot.ComputeHandYakus();

            Assert.IsNotNull(groupsOfYakus);
            Assert.AreEqual(1, groupsOfYakus.Count);
            Assert.IsNotNull(groupsOfYakus[0]);
            Assert.AreEqual(2, groupsOfYakus[0].Count);
            Assert.IsTrue(groupsOfYakus[0].Contains(YakuPivot.Get(YakuPivot.Chiihou)));
            Assert.IsTrue(groupsOfYakus[0].Contains(YakuPivot.Get(YakuPivot.ChuurenPoutou)));
        }

        [TestMethod]
        public void Test_Chiitoi_Or_Ryanpeikou()
        {
            List<TilePivot> tiles = new List<TilePivot>
            {
                new TilePivot(FamilyPivot.character, 1),
                new TilePivot(FamilyPivot.character, 1),
                new TilePivot(FamilyPivot.character, 2),
                new TilePivot(FamilyPivot.character, 3),
                new TilePivot(FamilyPivot.character, 3),
                new TilePivot(FamilyPivot.character, 7),
                new TilePivot(FamilyPivot.character, 7),
                new TilePivot(FamilyPivot.character, 8),
                new TilePivot(FamilyPivot.character, 8),
                new TilePivot(FamilyPivot.character, 9),
                new TilePivot(FamilyPivot.character, 9),
                new TilePivot(FamilyPivot.circle, 4),
                new TilePivot(FamilyPivot.circle, 4),
            };
            tiles = tiles.OrderBy(x => _randomizer.Next()).ToList();

            FullHandPivot handPivot = new FullHandPivot(tiles, WindPivot.east, WindPivot.north,
                new TilePivot(FamilyPivot.character, 2), isRon: true, isRiichi: true);

            List<List<YakuPivot>> groupsOfYakus = handPivot.ComputeHandYakus();

            Assert.IsNotNull(groupsOfYakus);
            Assert.AreEqual(2, groupsOfYakus.Count);
            Assert.IsNotNull(groupsOfYakus[0]);
            Assert.IsNotNull(groupsOfYakus[1]);
            Assert.AreEqual(2, groupsOfYakus[0].Count);
            Assert.AreEqual(2, groupsOfYakus[1].Count);
            Assert.IsTrue(groupsOfYakus[0].Contains(YakuPivot.Get(YakuPivot.Ryanpeikou)));
            Assert.IsTrue(groupsOfYakus[0].Contains(YakuPivot.Get(YakuPivot.Riichi)));
            Assert.IsTrue(groupsOfYakus[1].Contains(YakuPivot.Get(YakuPivot.Chiitoitsu)));
            Assert.IsTrue(groupsOfYakus[1].Contains(YakuPivot.Get(YakuPivot.Riichi)));
        }
    }
}
