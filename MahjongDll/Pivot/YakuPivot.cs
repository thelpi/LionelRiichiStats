using System.Collections.Generic;
using System.Linq;

namespace MahjongDll.Pivot
{
    /// <summary>
    /// Represents a yaku.
    /// </summary>
    public class YakuPivot
    {
        #region Embedded properties

        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Short english description.
        /// </summary>
        public string Description { get; private set; }
        /// <summary>
        /// Fan count when concealed.
        /// </summary>
        public int FansConcealed { get; private set; }
        /// <summary>
        /// Fan count when open.
        /// </summary>
        /// <remarks>0 when the yaku can't be made opened.</remarks>
        public int FansOpen { get; private set; }

        #endregion

        #region Inferred properties

        /// <summary>
        /// Gets if yakuman or not.
        /// </summary>
        public bool Yakuman { get { return FansConcealed == 13; } }

        #endregion

        #region Static properties

        // Static list of yakus.
        private static List<YakuPivot> _yakus;

        /// <summary>
        /// Static read-only list of yakus.
        /// </summary>
        public static IReadOnlyCollection<YakuPivot> Yakus
        {
            get
            {
                if (_yakus == null)
                {
                    _yakus = new List<YakuPivot>();
                    GenerateYakusList();
                    _yakus = _yakus.OrderByDescending(y => y.FansConcealed).ToList();
                }
                return _yakus;
            }
        }

        #endregion Static properties

        // Private constructor.
        private YakuPivot() { }

        // Generates the static list of yakus.
        private static void GenerateYakusList()
        {
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 1,
                FansOpen = 0,
                Name = Riichi,
                Description = "Declaration of tenpai (hand must be closed and becomes immutable)."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 1,
                FansOpen = 0,
                Name = Ippatsu,
                Description = "Win during the turn which follows riichi (no call made)."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 1,
                FansOpen = 0,
                Name = MenzenTsumo,
                Description = "All tiles concealed."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 1,
                FansOpen = 0,
                Name = Pinfu,
                Description = "Only Chis and one pair (hand must be closed, pair without value, wait on both sides)."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 1,
                FansOpen = 0,
                Name = Iipeikou,
                Description = "Twice the same chi (hand must be closed)."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 1,
                FansOpen = 1,
                Name = Haitei,
                Description = "Win on the last tile (ron or tsumo)."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 1,
                FansOpen = 1,
                Name = RinshanKaihou,
                Description = "Win on the kan's compensation tile."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 1,
                FansOpen = 1,
                Name = Chankan,
                Description = "Win on 4th tile of an opponent calling a opened kan."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 1,
                FansOpen = 1,
                Name = Tanyao,
                Description = "No honors or terminals."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 1,
                FansOpen = 1,
                Name = Yakuhai,
                Description = "Pon or kan of dragons, turn wind or dominant wind."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 2,
                FansOpen = 0,
                Name = DoubleRiichi,
                Description = "Riichi at first turn (no call made)."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 2,
                FansOpen = 1,
                Name = Chantaiyao,
                Description = "Terminals or honors in each set."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 2,
                FansOpen = 1,
                Name = SanshokuDoujun,
                Description = "Same chi in three family."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 3,
                FansOpen = 2,
                Name = Ittsuu,
                Description = "123456789 in a single family."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 2,
                FansOpen = 2,
                Name = Toitoi,
                Description = "Pons (or kans) only."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 2,
                FansOpen = 2,
                Name = Sanankou,
                Description = "Three concelead pons."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 2,
                FansOpen = 2,
                Name = SanshokuDoukou,
                Description = "Same pon in three family."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 2,
                FansOpen = 2,
                Name = Sankantsu,
                Description = "Three kans."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 2,
                FansOpen = 0,
                Name = Chiitoitsu,
                Description = "Seven pairs (must be concelead)."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 2,
                FansOpen = 2,
                Name = Honroutou,
                Description = "Only terminals and honors."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 2,
                FansOpen = 2,
                Name = Shousangen,
                Description = "Two pons or kans or dragons and a pair of dragons."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 3,
                FansOpen = 2,
                Name = Honitsu,
                Description = "A single family and honors."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 3,
                FansOpen = 2,
                Name = Junchantaiyao,
                Description = "Terminals in each set."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 3,
                FansOpen = 0,
                Name = Ryanpeikou,
                Description = "Double iipeikou (must be concealed)."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 6,
                FansOpen = 5,
                Name = Chinitsu,
                Description = "A single family."
            });

            // Yakumans below this line.
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 13,
                FansOpen = 0,
                Name = KokushiMusou,
                Description = "One tile of each terminal and honor, and a duplicate."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 13,
                FansOpen = 13,
                Name = Daisangen,
                Description = "Pons or kans of each dragon."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 13,
                FansOpen = 0,
                Name = Suuankou,
                Description = "Four concealed pons or kans."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 13,
                FansOpen = 13,
                Name = Shousuushi,
                Description = "Three pons or kans of winds, and a pair of winds."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 13,
                FansOpen = 13,
                Name = Daisuushi,
                Description = "Pons or kans of each wind."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 13,
                FansOpen = 13,
                Name = Tsuuiisou,
                Description = "Sets made of honors only."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 13,
                FansOpen = 13,
                Name = Ryuuiisou,
                Description = "Sets made of green bamboos (2, 3, 4, 6, 8) and green dragons only."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 13,
                FansOpen = 13,
                Name = Chinroutou,
                Description = "Sets made of terminals only."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 13,
                FansOpen = 0,
                Name = ChuurenPoutou,
                Description = "1112345678999 in a single family, and on tile of the family."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 13,
                FansOpen = 13,
                Name = Suukantsu,
                Description = "Four kans."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 13,
                FansOpen = 0,
                Name = Tenhou,
                Description = "Dealer win with initial tiles."
            });
            _yakus.Add(new YakuPivot
            {
                FansConcealed = 13,
                FansOpen = 0,
                Name = Chiihou,
                Description = "Non-dealer win at first pick."
            });
        }

        /// <summary>
        /// Gets a <see cref="YakuPivot"/> by his name.
        /// </summary>
        /// <remarks>Case insensitive.</remarks>
        /// <param name="name">Name of the <see cref="YakuPivot"/>.</param>
        /// <returns>The <see cref="YakuPivot"/>, <c>Null</c> if not found.</returns>
        public static YakuPivot Get(string name)
        {
            return Yakus.FirstOrDefault(y => y.Name.ToLowerInvariant().Equals((name ?? string.Empty).Trim().ToLowerInvariant()));
        }

        #region Yaku's name constants

        /// <summary>
        /// Double riichi.
        /// </summary>
        public const string DoubleRiichi = "Double riichi";
        /// <summary>
        /// Riichi.
        /// </summary>
        public const string Riichi = "Riichi";
        /// <summary>
        /// Ippatsu.
        /// </summary>
        public const string Ippatsu = "Ippatsu";
        /// <summary>
        /// Menzen tsumo.
        /// </summary>
        public const string MenzenTsumo = "Menzen tsumo";
        /// <summary>
        /// Pinfu.
        /// </summary>
        public const string Pinfu = "Pinfu";
        /// <summary>
        /// Iipeikou.
        /// </summary>
        public const string Iipeikou = "Iipeikou";
        /// <summary>
        /// Haitei.
        /// </summary>
        public const string Haitei = "Haitei";
        /// <summary>
        /// Rinshan kaihou.
        /// </summary>
        public const string RinshanKaihou = "Rinshan kaihou";
        /// <summary>
        /// Chankan.
        /// </summary>
        public const string Chankan = "Chankan";
        /// <summary>
        /// Tanyao.
        /// </summary>
        public const string Tanyao = "Tanyao";
        /// <summary>
        /// Yakuhai.
        /// </summary>
        public const string Yakuhai = "Yakuhai";
        /// <summary>
        /// Chantaiyao.
        /// </summary>
        public const string Chantaiyao = "Chantaiyao";
        /// <summary>
        /// Sanshoku doujun.
        /// </summary>
        public const string SanshokuDoujun = "Sanshoku doujun";
        /// <summary>
        /// Ittsuu.
        /// </summary>
        public const string Ittsuu = "Ittsuu";
        /// <summary>
        /// Toitoi.
        /// </summary>
        public const string Toitoi = "Toitoi";
        /// <summary>
        /// Sanankou.
        /// </summary>
        public const string Sanankou = "Sanankou";
        /// <summary>
        /// Sanshoku doukou.
        /// </summary>
        public const string SanshokuDoukou = "Sanshoku doukou";
        /// <summary>
        /// Sankantsu.
        /// </summary>
        public const string Sankantsu = "Sankantsu";
        /// <summary>
        /// Chiitoitsu.
        /// </summary>
        public const string Chiitoitsu = "Chiitoitsu";
        /// <summary>
        /// Honroutou.
        /// </summary>
        public const string Honroutou = "Honroutou";
        /// <summary>
        /// Shousangen.
        /// </summary>
        public const string Shousangen = "Shousangen";
        /// <summary>
        /// Honitsu.
        /// </summary>
        public const string Honitsu = "Honitsu";
        /// <summary>
        /// Junchantaiyao.
        /// </summary>
        public const string Junchantaiyao = "Junchantaiyao";
        /// <summary>
        /// Ryanpeikou.
        /// </summary>
        public const string Ryanpeikou = "Ryanpeikou";
        /// <summary>
        /// Chinitsu.
        /// </summary>
        public const string Chinitsu = "Chinitsu";
        /// <summary>
        /// Kokushi musou.
        /// </summary>
        public const string KokushiMusou = "Kokushi musou";
        /// <summary>
        /// Daisangen.
        /// </summary>
        public const string Daisangen = "Daisangen";
        /// <summary>
        /// Suuankou.
        /// </summary>
        public const string Suuankou = "Suuankou";
        /// <summary>
        /// Shousuushi.
        /// </summary>
        public const string Shousuushi = "Shousuushi";
        /// <summary>
        /// Daisuushi.
        /// </summary>
        public const string Daisuushi = "Daisuushi";
        /// <summary>
        /// Tsuuiisou.
        /// </summary>
        public const string Tsuuiisou = "Tsuuiisou";
        /// <summary>
        /// Ryuuiisou.
        /// </summary>
        public const string Ryuuiisou = "Ryuuiisou";
        /// <summary>
        /// Chinroutou.
        /// </summary>
        public const string Chinroutou = "Chinroutou";
        /// <summary>
        /// Chuuren poutou.
        /// </summary>
        public const string ChuurenPoutou = "Chuuren poutou";
        /// <summary>
        /// Suukantsu.
        /// </summary>
        public const string Suukantsu = "Suukantsu";
        /// <summary>
        /// Tenhou.
        /// </summary>
        public const string Tenhou = "Tenhou";
        /// <summary>
        /// Chiihou.
        /// </summary>
        public const string Chiihou = "Chiihou";

        #endregion
    }
}
