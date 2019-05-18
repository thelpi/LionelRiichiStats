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
            SetPivot openedSets = new SetPivot(new TilePivot(WindPivot.East),
                new TilePivot(WindPivot.East),
                new TilePivot(WindPivot.East));

            List<TilePivot> tiles = new List<TilePivot>
            {
                new TilePivot(FamilyPivot.Bamboo, 4),
                new TilePivot(FamilyPivot.Bamboo, 5),
                new TilePivot(FamilyPivot.Bamboo, 6),
                new TilePivot(FamilyPivot.Bamboo, 5),
                new TilePivot(FamilyPivot.Bamboo, 6),
                new TilePivot(FamilyPivot.Bamboo, 7),
                new TilePivot(FamilyPivot.Circle, 3),
                new TilePivot(FamilyPivot.Circle, 3),
                new TilePivot(FamilyPivot.Circle, 1),
                new TilePivot(FamilyPivot.Circle, 2)
            };
            tiles = tiles.OrderBy(x => _randomizer.Next()).ToList();

            FullHandPivot handPivot = new FullHandPivot(tiles, WindPivot.East, WindPivot.East,
                new TilePivot(FamilyPivot.Circle, 3),
                openedSets: new List<SetPivot> { openedSets });

            List<HandYakuListPivot> groupsOfYakus = handPivot.ComputeHandYakus();

            Assert.IsNotNull(groupsOfYakus);
            Assert.AreEqual(1, groupsOfYakus.Count);
            Assert.IsNotNull(groupsOfYakus[0]);
            Assert.AreEqual(2, groupsOfYakus[0].Yakus.Count);
            Assert.IsTrue(groupsOfYakus[0].Yakus.All(y => y.Name == YakuPivot.Yakuhai));
        }

        [TestMethod]
        public void Test_Chitoi_TanYao_Tsumo_DoubleRiichi_Ippatsu()
        {
            List<TilePivot> tiles = new List<TilePivot>
            {
                new TilePivot(FamilyPivot.Character, 2),
                new TilePivot(FamilyPivot.Character, 2),
                new TilePivot(FamilyPivot.Character, 5),
                new TilePivot(FamilyPivot.Character, 5),
                new TilePivot(FamilyPivot.Character, 6),
                new TilePivot(FamilyPivot.Character, 6),
                new TilePivot(FamilyPivot.Bamboo, 4),
                new TilePivot(FamilyPivot.Bamboo, 4),
                new TilePivot(FamilyPivot.Bamboo, 8),
                new TilePivot(FamilyPivot.Bamboo, 8),
                new TilePivot(FamilyPivot.Circle, 5),
                new TilePivot(FamilyPivot.Circle, 5),
                new TilePivot(FamilyPivot.Circle, 8)
            };
            tiles = tiles.OrderBy(x => _randomizer.Next()).ToList();

            FullHandPivot handPivot = new FullHandPivot(tiles, WindPivot.East, WindPivot.East,
                new TilePivot(FamilyPivot.Circle, 8), isRon: false, isDoubleRiichi: true, isIppatsu: true);

            List<HandYakuListPivot> groupsOfYakus = handPivot.ComputeHandYakus();

            Assert.IsNotNull(groupsOfYakus);
            Assert.AreEqual(1, groupsOfYakus.Count);
            Assert.IsNotNull(groupsOfYakus[0]);
            Assert.AreEqual(5, groupsOfYakus[0].Yakus.Count);
            Assert.IsTrue(groupsOfYakus[0].Yakus.Contains(YakuPivot.Get(YakuPivot.Chiitoitsu)));
            Assert.IsTrue(groupsOfYakus[0].Yakus.Contains(YakuPivot.Get(YakuPivot.Tanyao)));
            Assert.IsTrue(groupsOfYakus[0].Yakus.Contains(YakuPivot.Get(YakuPivot.MenzenTsumo)));
            Assert.IsTrue(groupsOfYakus[0].Yakus.Contains(YakuPivot.Get(YakuPivot.DoubleRiichi)));
            Assert.IsTrue(groupsOfYakus[0].Yakus.Contains(YakuPivot.Get(YakuPivot.Ippatsu)));
        }

        [TestMethod]
        public void Test_Sankantsu_Chankan_Honroutou_Shousangen_Honitsu_Sanankou_Toitoi_Yakuhai()
        {
            List<SetPivot> concealedKans = new List<SetPivot>
            {
                new SetPivot(new TilePivot(DragonPivot.White),
                    new TilePivot(DragonPivot.White),
                    new TilePivot(DragonPivot.White),
                    new TilePivot(DragonPivot.White)),
                new SetPivot(new TilePivot(DragonPivot.Green),
                    new TilePivot(DragonPivot.Green),
                    new TilePivot(DragonPivot.Green),
                    new TilePivot(DragonPivot.Green)),
                new SetPivot(new TilePivot(WindPivot.North),
                    new TilePivot(WindPivot.North),
                    new TilePivot(WindPivot.North),
                    new TilePivot(WindPivot.North))
            };
            List<TilePivot> tiles = new List<TilePivot>
            {
                new TilePivot(DragonPivot.Red),
                new TilePivot(DragonPivot.Red),
                new TilePivot(FamilyPivot.Circle, 9),
                new TilePivot(FamilyPivot.Circle, 9)
            };
            tiles = tiles.OrderBy(x => _randomizer.Next()).ToList();

            // Note : this is an impossible hand, as Chankan can be made on something else than a chi.
            FullHandPivot handPivot = new FullHandPivot(tiles, WindPivot.East, WindPivot.North,
                new TilePivot(FamilyPivot.Circle, 9), concealedKans: concealedKans,
                isChankan: true, isRon: true);

            List<HandYakuListPivot> groupsOfYakus = handPivot.ComputeHandYakus();

            Assert.IsNotNull(groupsOfYakus);
            Assert.AreEqual(1, groupsOfYakus.Count);
            Assert.IsNotNull(groupsOfYakus[0]);
            Assert.AreEqual(10, groupsOfYakus[0].Yakus.Count);
            Assert.AreEqual(3, groupsOfYakus[0].Yakus.Count(y => y.Name == YakuPivot.Yakuhai));
            Assert.IsTrue(groupsOfYakus[0].Yakus.Contains(YakuPivot.Get(YakuPivot.Toitoi)));
            Assert.IsTrue(groupsOfYakus[0].Yakus.Contains(YakuPivot.Get(YakuPivot.Sanankou)));
            Assert.IsTrue(groupsOfYakus[0].Yakus.Contains(YakuPivot.Get(YakuPivot.Honitsu)));
            Assert.IsTrue(groupsOfYakus[0].Yakus.Contains(YakuPivot.Get(YakuPivot.Shousangen)));
            Assert.IsTrue(groupsOfYakus[0].Yakus.Contains(YakuPivot.Get(YakuPivot.Honroutou)));
            Assert.IsTrue(groupsOfYakus[0].Yakus.Contains(YakuPivot.Get(YakuPivot.Chankan)));
            Assert.IsTrue(groupsOfYakus[0].Yakus.Contains(YakuPivot.Get(YakuPivot.Sankantsu)));
        }

        [TestMethod]
        public void Test_Ryanpeikou_Pinfu_Chinitsu_Junchantaiyao()
        {
            List<TilePivot> tiles = new List<TilePivot>
            {
                new TilePivot(FamilyPivot.Character, 1),
                new TilePivot(FamilyPivot.Character, 2),
                new TilePivot(FamilyPivot.Character, 2),
                new TilePivot(FamilyPivot.Character, 3),
                new TilePivot(FamilyPivot.Character, 3),
                new TilePivot(FamilyPivot.Character, 7),
                new TilePivot(FamilyPivot.Character, 7),
                new TilePivot(FamilyPivot.Character, 8),
                new TilePivot(FamilyPivot.Character, 8),
                new TilePivot(FamilyPivot.Character, 9),
                new TilePivot(FamilyPivot.Character, 9),
                new TilePivot(FamilyPivot.Character, 9),
                new TilePivot(FamilyPivot.Character, 9),
            };
            tiles = tiles.OrderBy(x => _randomizer.Next()).ToList();

            FullHandPivot handPivot = new FullHandPivot(tiles, WindPivot.East, WindPivot.North,
                new TilePivot(FamilyPivot.Character, 1), isRon: true);

            List<HandYakuListPivot> groupsOfYakus = handPivot.ComputeHandYakus();

            Assert.IsNotNull(groupsOfYakus);
            Assert.AreEqual(1, groupsOfYakus.Count);
            Assert.IsNotNull(groupsOfYakus[0]);
            Assert.AreEqual(4, groupsOfYakus[0].Yakus.Count);
            Assert.IsTrue(groupsOfYakus[0].Yakus.Contains(YakuPivot.Get(YakuPivot.Chinitsu)));
            Assert.IsTrue(groupsOfYakus[0].Yakus.Contains(YakuPivot.Get(YakuPivot.Ryanpeikou)));
            Assert.IsTrue(groupsOfYakus[0].Yakus.Contains(YakuPivot.Get(YakuPivot.Junchantaiyao)));
            Assert.IsTrue(groupsOfYakus[0].Yakus.Contains(YakuPivot.Get(YakuPivot.Pinfu)));
            Assert.IsFalse(groupsOfYakus[0].Yakus.Contains(YakuPivot.Get(YakuPivot.Chiitoitsu)));
            Assert.IsFalse(groupsOfYakus[0].Yakus.Contains(YakuPivot.Get(YakuPivot.Honitsu)));
            Assert.IsFalse(groupsOfYakus[0].Yakus.Contains(YakuPivot.Get(YakuPivot.Chantaiyao)));
        }

        [TestMethod]
        public void Test_KokushiMusou_Tenhou()
        {
            List<TilePivot> tiles = new List<TilePivot>
            {
                new TilePivot(FamilyPivot.Character, 1),
                new TilePivot(FamilyPivot.Character, 9),
                new TilePivot(FamilyPivot.Circle, 1),
                new TilePivot(FamilyPivot.Circle, 9),
                new TilePivot(FamilyPivot.Bamboo, 1),
                new TilePivot(FamilyPivot.Bamboo, 9),
                new TilePivot(WindPivot.East),
                new TilePivot(WindPivot.South),
                new TilePivot(WindPivot.North),
                new TilePivot(WindPivot.West),
                new TilePivot(DragonPivot.Red),
                new TilePivot(DragonPivot.Green),
                new TilePivot(DragonPivot.White)
            };
            tiles = tiles.OrderBy(x => _randomizer.Next()).ToList();

            FullHandPivot handPivot = new FullHandPivot(tiles, WindPivot.East, WindPivot.East, new TilePivot(DragonPivot.White));

            List<HandYakuListPivot> groupsOfYakus = handPivot.ComputeHandYakus();

            Assert.IsNotNull(groupsOfYakus);
            Assert.AreEqual(1, groupsOfYakus.Count);
            Assert.IsNotNull(groupsOfYakus[0]);
            Assert.AreEqual(1, groupsOfYakus[0].Yakus.Count);
            Assert.IsTrue(groupsOfYakus[0].Yakus.Contains(YakuPivot.Get(YakuPivot.KokushiMusou)));
        }

        [TestMethod]
        public void Test_ChuurenPoutou_Chiihou()
        {
            List<TilePivot> tiles = new List<TilePivot>
            {
                new TilePivot(FamilyPivot.Bamboo, 1),
                new TilePivot(FamilyPivot.Bamboo, 1),
                new TilePivot(FamilyPivot.Bamboo, 1),
                new TilePivot(FamilyPivot.Bamboo, 2),
                new TilePivot(FamilyPivot.Bamboo, 3),
                new TilePivot(FamilyPivot.Bamboo, 4),
                new TilePivot(FamilyPivot.Bamboo, 5),
                new TilePivot(FamilyPivot.Bamboo, 6),
                new TilePivot(FamilyPivot.Bamboo, 7),
                new TilePivot(FamilyPivot.Bamboo, 8),
                new TilePivot(FamilyPivot.Bamboo, 9),
                new TilePivot(FamilyPivot.Bamboo, 9),
                new TilePivot(FamilyPivot.Bamboo, 9)
            };
            tiles = tiles.OrderBy(x => _randomizer.Next()).ToList();

            FullHandPivot handPivot = new FullHandPivot(tiles, WindPivot.East, WindPivot.North, new TilePivot(FamilyPivot.Bamboo, 3));

            List<HandYakuListPivot> groupsOfYakus = handPivot.ComputeHandYakus();

            Assert.IsNotNull(groupsOfYakus);
            Assert.AreEqual(1, groupsOfYakus.Count);
            Assert.IsNotNull(groupsOfYakus[0]);
            Assert.AreEqual(1, groupsOfYakus[0].Yakus.Count);
            Assert.IsTrue(groupsOfYakus[0].Yakus.Contains(YakuPivot.Get(YakuPivot.ChuurenPoutou)));
        }

        [TestMethod]
        public void Test_Chiitoi_Or_Ryanpeikou()
        {
            List<TilePivot> tiles = new List<TilePivot>
            {
                new TilePivot(FamilyPivot.Character, 1),
                new TilePivot(FamilyPivot.Character, 1),
                new TilePivot(FamilyPivot.Character, 2),
                new TilePivot(FamilyPivot.Character, 3),
                new TilePivot(FamilyPivot.Character, 3),
                new TilePivot(FamilyPivot.Character, 7),
                new TilePivot(FamilyPivot.Character, 7),
                new TilePivot(FamilyPivot.Character, 8),
                new TilePivot(FamilyPivot.Character, 8),
                new TilePivot(FamilyPivot.Character, 9),
                new TilePivot(FamilyPivot.Character, 9),
                new TilePivot(FamilyPivot.Circle, 4),
                new TilePivot(FamilyPivot.Circle, 4),
            };
            tiles = tiles.OrderBy(x => _randomizer.Next()).ToList();

            FullHandPivot handPivot = new FullHandPivot(tiles, WindPivot.East, WindPivot.North,
                new TilePivot(FamilyPivot.Character, 2), isRon: true, isRiichi: true);

            List<HandYakuListPivot> groupsOfYakus = handPivot.ComputeHandYakus();

            Assert.IsNotNull(groupsOfYakus);
            Assert.AreEqual(2, groupsOfYakus.Count);
            Assert.IsNotNull(groupsOfYakus[0]);
            Assert.IsNotNull(groupsOfYakus[1]);
            Assert.AreEqual(2, groupsOfYakus[0].Yakus.Count);
            Assert.AreEqual(2, groupsOfYakus[1].Yakus.Count);
            Assert.IsTrue(groupsOfYakus[0].Yakus.Contains(YakuPivot.Get(YakuPivot.Ryanpeikou)));
            Assert.IsTrue(groupsOfYakus[0].Yakus.Contains(YakuPivot.Get(YakuPivot.Riichi)));
            Assert.IsTrue(groupsOfYakus[1].Yakus.Contains(YakuPivot.Get(YakuPivot.Chiitoitsu)));
            Assert.IsTrue(groupsOfYakus[1].Yakus.Contains(YakuPivot.Get(YakuPivot.Riichi)));
        }

        [TestMethod]
        public void Test_NotAValidHand()
        {
            List<TilePivot> tiles = new List<TilePivot>
            {
                new TilePivot(FamilyPivot.Character, 1),
                new TilePivot(FamilyPivot.Character, 1),
                new TilePivot(FamilyPivot.Bamboo, 1),
                new TilePivot(FamilyPivot.Bamboo, 2),
                new TilePivot(FamilyPivot.Bamboo, 3),
                new TilePivot(FamilyPivot.Bamboo, 7),
                new TilePivot(FamilyPivot.Bamboo, 7),
                new TilePivot(FamilyPivot.Character, 7),
                new TilePivot(FamilyPivot.Character, 8),
                new TilePivot(FamilyPivot.Character, 9),
                new TilePivot(WindPivot.North),
                new TilePivot(DragonPivot.Red),
                new TilePivot(DragonPivot.Red)
            };
            tiles = tiles.OrderBy(x => _randomizer.Next()).ToList();

            FullHandPivot handPivot = new FullHandPivot(tiles, WindPivot.East, WindPivot.North, new TilePivot(FamilyPivot.Character, 1));

            List<HandYakuListPivot> groupsOfYakus = handPivot.ComputeHandYakus();

            Assert.IsNotNull(groupsOfYakus);
            Assert.AreEqual(0, groupsOfYakus.Count);
        }
    }
}
