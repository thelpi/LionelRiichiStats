using System;
using System.Collections.Generic;
using System.Linq;

namespace MahjongDll.Pivot
{
    /// <summary>
    /// Represents a hand of 14 tiles, regardless of the context.
    /// </summary>
    public class FullHandPivot
    {
        // number of tiles, in a given family, which prevents to use every tiles in valid sets.
        private static readonly int[] _forbiddenCounts = { 1, 4, 7, 10, 13 };
        // number of tiles, in a given family, which implies one of the sets is a pair.
        private static readonly int[] _withPairCounts = _forbiddenCounts.Select(x => x + 1).ToArray();

        #region Embedded properties

        /// <summary>
        /// List of concealed <see cref="TilePivot"/>.
        /// </summary>
        public IReadOnlyCollection<TilePivot> ConcealedTiles { get; private set; }
        /// <summary>
        /// Dominant <see cref="WindPivot"/>.
        /// </summary>
        public WindPivot DominantWind { get; private set; }
        /// <summary>
        /// Turn <see cref="WindPivot"/> for the player of the hand.
        /// </summary>
        public WindPivot TurnWind { get; private set; }
        /// <summary>
        /// List of opened <see cref="SetPivot"/>.
        /// </summary>
        public IReadOnlyCollection<SetPivot> OpenedSets { get; private set; }
        /// <summary>
        /// Last <see cref="TilePivot"/> picked. Not included in <see cref="ConcealedTiles"/>.
        /// </summary>
        public TilePivot LastTile { get; private set; }
        /// <summary>
        /// Closed kans.
        /// </summary>
        public IReadOnlyCollection<SetPivot> ConcealedKans { get; private set; }
        /// <summary>
        /// Indicates if the context is riichi.
        /// </summary>
        /// <remarks><c>True</c> if <see cref="IsDoubleRiichi"/>.</remarks>
        public bool IsRiichi { get; private set; }
        /// <summary>
        /// Indicates if the context is riichi at first turn.
        /// </summary>
        public bool IsDoubleRiichi { get; private set; }
        /// <summary>
        /// Indicates if the context is an ippatsu win.
        /// </summary>
        public bool IsIppatsu { get; private set; }
        /// <summary>
        /// Indicates if the context is a chankan win.
        /// </summary>
        public bool IsChankan { get; private set; }
        /// <summary>
        /// Indicates if the context is a haitei win.
        /// </summary>
        public bool IsHaitei { get; private set; }
        /// <summary>
        /// Indicates if the context is a rinshankaihou win.
        /// </summary>
        public bool IsRinshankaihou { get; private set; }
        /// <summary>
        /// Indicates if the context is a first draw win.
        /// </summary>
        public bool IsInitialDraw { get; private set; }
        /// <summary>
        /// Indicates if the context is a ron win.
        /// </summary>
        public bool IsRon { get; private set; }

        #endregion Embedded properties

        #region Inferred properties

        // List of tiles which are not in declared sets.
        private List<TilePivot> _unsetTiles;

        /// <summary>
        /// Indicates if the hand is opened.
        /// </summary>
        public bool IsOpen => OpenedSets.Count > 0;
        /// <summary>
        /// List of tiles which are not in declared sets.
        /// </summary>
        public IReadOnlyCollection<TilePivot> UnsetTiles
        {
            get
            {
                if (_unsetTiles == null)
                {
                    List<TilePivot> tilesToAdd = new List<TilePivot>();
                    if (LastTile != null)
                    {
                        tilesToAdd.Add(LastTile);
                    }
                    _unsetTiles = ConcealedTiles.Concat(tilesToAdd).ToList();
                    _unsetTiles.Sort();
                }
                return _unsetTiles;
            }
        }
        /// <summary>
        /// List of every tiles of the hand.
        /// </summary>
        /// <remarks>Doesn't include fourth tile of each kan.</remarks>
        public IReadOnlyCollection<TilePivot> AllTiles
        {
            get
            {
                List<TilePivot> tmpList = new List<TilePivot>(UnsetTiles);
                tmpList.AddRange(OpenedSets.SelectMany(x => x.Tiles.Take(3)));
                return tmpList;
            }
        }

        #endregion Inferred properties

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tiles"><see cref="ConcealedTiles"/>.</param>
        /// <param name="dominantWind"><see cref="DominantWind"/>.</param>
        /// <param name="turnWind"><see cref="TurnWind"/>.</param>
        /// <param name="lastTile"><see cref="LastTile"/>.</param>
        /// <param name="isRon">Optionnal ; <see cref="IsRon"/>.</param>
        /// <param name="openedSets">Optionnal ; <see cref="OpenedSets"/>.</param>
        /// <param name="concealedKans">Optionnal ; <see cref="ConcealedKans"/>.</param>
        /// <param name="isRiichi">Optionnal ; <see cref="IsRiichi"/>.</param>
        /// <param name="isDoubleRiichi">Optionnal ; <see cref="IsDoubleRiichi"/>.</param>
        /// <param name="isIppatsu">Optionnal ; <see cref="IsIppatsu"/>.</param>
        /// <param name="isHaitei">Optionnal ; <see cref="IsHaitei"/>.</param>
        /// <param name="isRinshankaihou">Optionnal ; <see cref="IsRinshankaihou"/>.</param>
        /// <param name="isChankan">Optionnal ; <see cref="IsChankan"/>.</param>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidTilesCountInHandError"/>.</exception>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidHandArgumentsConsistencyError"/>.</exception>
        public FullHandPivot(List<TilePivot> tiles, WindPivot dominantWind, WindPivot turnWind, TilePivot lastTile, bool isRon = false,
            List<SetPivot> openedSets = null, List<SetPivot> concealedKans = null,
            bool isRiichi = false, bool isDoubleRiichi = false, bool isIppatsu = false, bool isHaitei = false,
            bool isRinshankaihou = false, bool isChankan = false)
        {
            tiles = tiles ?? new List<TilePivot>();
            openedSets = openedSets ?? new List<SetPivot>();
            concealedKans = concealedKans ?? new List<SetPivot>();

            if ((openedSets.Count * 3 + concealedKans.Count * 3 + 1 + tiles.Count) != 14)
            {
                throw new ArgumentException(Messages.InvalidTilesCountInHandError);
            }

            if (openedSets.Count > 0 && (isRiichi || isDoubleRiichi || isIppatsu || openedSets.Any(set => set.IsPair)))
            {
                throw new ArgumentException(Messages.InvalidHandArgumentsConsistencyError, nameof(openedSets));
            }

            if (concealedKans.Count > 0 && concealedKans.Any(set => !set.IsKan))
            {
                throw new ArgumentException(Messages.InvalidHandArgumentsConsistencyError, nameof(concealedKans));
            }

            if (concealedKans.Count == 0 && !openedSets.Any(set => set.IsKan) && isRinshankaihou)
            {
                throw new ArgumentException(Messages.InvalidHandArgumentsConsistencyError, nameof(isRinshankaihou));
            }

            if (IsHaitei && isRinshankaihou)
            {
                throw new ArgumentException(Messages.InvalidHandArgumentsConsistencyError, nameof(IsHaitei));
            }

            if (isChankan && (isRinshankaihou || !isRon))
            {
                throw new ArgumentException(Messages.InvalidHandArgumentsConsistencyError, nameof(isChankan));
            }

            if (isIppatsu && (isRinshankaihou || (IsRon && isHaitei)))
            {
                throw new ArgumentException(Messages.InvalidHandArgumentsConsistencyError, nameof(isIppatsu));
            }

            ConcealedTiles = tiles.OrderBy(t => t).ToList();
            DominantWind = dominantWind;
            TurnWind = turnWind;
            LastTile = lastTile ?? throw new ArgumentNullException(nameof(lastTile));
            IsRon = isRon;
            OpenedSets = openedSets;
            ConcealedKans = concealedKans;
            IsRiichi = isRiichi || isDoubleRiichi;
            IsIppatsu = isIppatsu;
            IsDoubleRiichi = isDoubleRiichi;
            IsChankan = isChankan;
            IsHaitei = isHaitei;
            IsRinshankaihou = isRinshankaihou;
        }

        /// <summary>
        /// Constructor specific to tenhou / chiihou.
        /// </summary>
        /// <param name="tiles"><see cref="ConcealedTiles"/>.</param>
        /// <param name="dominantWind"><see cref="DominantWind"/>.</param>
        /// <param name="turnWind"><see cref="TurnWind"/>.</param>
        /// <param name="isInitialDraw"><see cref="IsInitialDraw"/>.</param>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidTilesCountInHandError"/>.</exception>
        public FullHandPivot(List<TilePivot> tiles, WindPivot dominantWind, WindPivot turnWind, bool isInitialDraw = true)
        {
            tiles = tiles ?? new List<TilePivot>();

            if (tiles.Count != 14)
            {
                throw new ArgumentException(Messages.InvalidTilesCountInHandError);
            }

            ConcealedTiles = tiles.OrderBy(t => t).ToList();
            DominantWind = dominantWind;
            TurnWind = turnWind;
            OpenedSets = new List<SetPivot>();
            ConcealedKans = new List<SetPivot>();
            IsInitialDraw = isInitialDraw;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="substitutions"></param>
        /// <returns></returns>
        public static FullHandPivot FromHand(FullHandPivot hand, Tuple<TilePivot, TilePivot>[] substitutions)
        {
            List<TilePivot> tiles = hand.AllTiles.ToList();
            // subs
            return new FullHandPivot(tiles, hand.DominantWind, hand.TurnWind, hand.IsInitialDraw);
        }

        /// <summary>
        /// Computes every yakus extractible from the current hand.
        /// </summary>
        /// <returns>
        /// List of list of yakus (one for each valid combination of tiles).
        /// Sorted by descending fans count.
        /// </returns>
        public List<List<YakuPivot>> ComputeHandYakus()
        {
            List<List<YakuPivot>> groupsOfYakus = new List<List<YakuPivot>>();
            bool checkCircumstantialYakus = false;

            if (IsKokushiMusou())
            {
                groupsOfYakus.Add(new List<YakuPivot> { YakuPivot.Get(YakuPivot.KokushiMusou) });
                checkCircumstantialYakus = true;
            }
            else
            {
                List<SetPivot> chiToiSets = GetChiitoitsuCombination();

                List<List<SetPivot>> combinationlist = ComputeValidCombinations();
                if (chiToiSets.Count == 7)
                {
                    combinationlist.Add(chiToiSets);
                }

                if (combinationlist.Count > 0)
                {
                    checkCircumstantialYakus = true;
                    foreach (List<SetPivot> currentCombo in combinationlist)
                    {
                        List<YakuPivot> yakusList = new List<YakuPivot>();
                        foreach (YakuPivot yakuToCheck in YakuPivot.Yakus.OrderByDescending(y => (IsOpen ? y.FansOpen : y.FansConcealed)))
                        {
                            if (IsOpen && yakuToCheck.FansOpen == 0)
                            {
                                continue;
                            }

                            bool validCombination = false;
                            // For yakuhai only.
                            int yakuCumul = 1;
                            switch (yakuToCheck.Name)
                            {
                                case YakuPivot.Chantaiyao:
                                    validCombination = currentCombo.All(c => c.IsHonorOrTerminal || c.IsTerminalChi);
                                    break;
                                case YakuPivot.Chinitsu:
                                    validCombination = currentCombo.All(c => !c.IsHonor && c.Family == currentCombo.First().Family);
                                    break;
                                case YakuPivot.Chinroutou:
                                    validCombination = currentCombo.All(c => c.IsTerminal);
                                    break;
                                case YakuPivot.ChuurenPoutou:
                                    validCombination = IsChuurenPoutou();
                                    break;
                                case YakuPivot.Daisangen:
                                    validCombination = currentCombo.Count(c => c.IsPonOrKan && c.IsDragon) == 3;
                                    break;
                                case YakuPivot.Daisuushi:
                                    validCombination = currentCombo.Count(c => c.IsWind && c.IsPonOrKan) == 4;
                                    break;
                                case YakuPivot.Honitsu:
                                    validCombination = currentCombo.All(c => c.IsHonor || (!c.IsHonor && c.Family == currentCombo.First(cBis => !cBis.IsHonor).Family));
                                    break;
                                case YakuPivot.Honroutou:
                                    validCombination = currentCombo.All(c => c.IsHonorOrTerminal);
                                    break;
                                case YakuPivot.Iipeikou:
                                    validCombination = currentCombo.Where(c => c.IsChi).GroupBy(c => new { c.FirstNumber, c.Family }).Any(c => c.Count() >= 2);
                                    break;
                                case YakuPivot.Ittsuu:
                                    validCombination = currentCombo.Where(c => c.IsChi).GroupBy(c => c.Family).Any(c =>
                                        new int[] { 1, 4, 7 }.All(index => c.Any(set => set.FirstNumber == index)));
                                    break;
                                case YakuPivot.Junchantaiyao:
                                    validCombination = currentCombo.All(c => c.IsTerminal || c.IsTerminalChi);
                                    break;
                                case YakuPivot.Pinfu:
                                    validCombination = currentCombo.All(c => c.IsChi || (c.IsPair && !c.IsDragon && c.Wind != TurnWind && c.Wind != DominantWind))
                                        && currentCombo.Any(c => c.IsChi && (c.Tiles.ElementAt(0).Equals(LastTile) || c.Tiles.ElementAt(2).Equals(LastTile)));
                                    break;
                                case YakuPivot.Ryanpeikou:
                                    validCombination = currentCombo.Where(c => c.IsChi).GroupBy(c => new { c.FirstNumber, c.Family }).Count(c => c.Count() >= 2) > 1
                                        || currentCombo.Where(c => c.IsChi).GroupBy(c => new { c.FirstNumber, c.Family }).Any(c => c.Count() == 4);
                                    break;
                                case YakuPivot.Ryuuiisou:
                                    validCombination = currentCombo.All(c => c.Dragon == DragonPivot.Green
                                        || (c.Family == FamilyPivot.Bamboo && c.Tiles.All(t => t.Number == 3 || t.Number % 2 == 0)));
                                    break;
                                case YakuPivot.Sanankou:
                                    validCombination = CountConealedPons(currentCombo, 3);
                                    break;
                                case YakuPivot.Sankantsu:
                                    validCombination = currentCombo.Count(c => c.IsKan) == 3;
                                    break;
                                case YakuPivot.SanshokuDoujun:
                                    validCombination = currentCombo.Where(c => c.IsChi).GroupBy(c => c.FirstNumber).Any(c => c.Count() >= 3
                                        && new FamilyPivot[] { FamilyPivot.Bamboo, FamilyPivot.Character, FamilyPivot.Circle }.All(f =>
                                            c.Any(set => set.Family == f)));
                                    break;
                                case YakuPivot.SanshokuDoukou:
                                    validCombination = currentCombo.Where(c => c.IsPonOrKan && !c.IsHonor).GroupBy(c => c.FirstNumber).Any(c => c.Count() == 3);
                                    break;
                                case YakuPivot.Shousangen:
                                    validCombination = currentCombo.Count(c => c.IsDragon && c.IsPonOrKan) == 2
                                        && currentCombo.Any(c => c.IsPair && c.IsDragon);
                                    break;
                                case YakuPivot.Shousuushi:
                                    validCombination = currentCombo.Count(c => c.IsWind && c.IsPonOrKan) == 3
                                        && currentCombo.Any(c => c.IsPair && c.IsWind);
                                    break;
                                case YakuPivot.Suuankou:
                                    validCombination = CountConealedPons(currentCombo, 4);
                                    break;
                                case YakuPivot.Suukantsu:
                                    validCombination = currentCombo.Count(c => c.IsKan) == 4;
                                    break;
                                case YakuPivot.Tanyao:
                                    validCombination = currentCombo.All(c => !c.IsHonorOrTerminal && !c.IsTerminalChi);
                                    break;
                                case YakuPivot.Toitoi:
                                    validCombination = currentCombo.Count(c => c.IsPonOrKan) == 4;
                                    break;
                                case YakuPivot.Tsuuiisou:
                                    validCombination = currentCombo.All(c => c.IsHonor);
                                    break;
                                case YakuPivot.Yakuhai:
                                    yakuCumul = currentCombo.Count(c => c.IsPonOrKan && c.IsDragon);
                                    yakuCumul += currentCombo.Count(c => c.IsPonOrKan && c.Wind == TurnWind);
                                    yakuCumul += currentCombo.Count(c => c.IsPonOrKan && c.Wind == DominantWind);
                                    validCombination = yakuCumul > 0;
                                    break;
                                case YakuPivot.Chiitoitsu:
                                    validCombination = currentCombo.All(x => x.IsPair);
                                    break;
                            }
                            // 1 - the combination is valid.
                            // 2 - the combination is a yakuman or "yakusList" doesn't contain a yakuman.
                            // 3 - there's no upgrade of the combination in "yakusList".
                            // 4 - the hand is concealed or the yaku is valid when open.
                            if (validCombination
                                && (yakuToCheck.Yakuman || !yakusList.Any(y => y.Yakuman))
                                && !yakusList.Any(y => yakuToCheck.Upgrades.Contains(y))
                                && (!IsOpen || yakuToCheck.FansOpen > 0))
                            {
                                for (int i = 1; i <= yakuCumul; i++)
                                {
                                    yakusList.Add(yakuToCheck);
                                }
                            }
                        }
                        groupsOfYakus.Add(yakusList);
                    }
                }
            }

            if (checkCircumstantialYakus)
            {
                List<YakuPivot> circumstantialYakus = CheckCircumstantialYakus();
                foreach (List<YakuPivot> yakuGroup in groupsOfYakus)
                {
                    yakuGroup.AddRange(circumstantialYakus.Where(y => y.Yakuman || !yakuGroup.Any(y2 => y2.Yakuman)));
                }
            }

            return groupsOfYakus
                    .OrderByDescending(ys =>
                        ys.Sum(y =>
                            (IsOpen ? y.FansOpen : y.FansConcealed)))
                    .ToList();
        }

        #region Hands validation

        // Checks the count of concealed pons or kans in the hand.
        private bool CountConealedPons(List<SetPivot> combo, int expectedCount)
        {
            return combo.Count(c => c.IsPonOrKan
                && !OpenedSets.Contains(c)
                && (!IsRon || !c.Tiles.Contains(LastTile))) == expectedCount;
        }

        // Checks yakus that don't depend on tiles (assuming the hand is valid).
        private List<YakuPivot> CheckCircumstantialYakus()
        {
            List<YakuPivot> yakus = new List<YakuPivot>();

            foreach (YakuPivot yakuToCheck in YakuPivot.Yakus)
            {
                bool isIn = false;
                switch (yakuToCheck.Name)
                {
                    case YakuPivot.MenzenTsumo:
                        isIn = !IsRon && !IsOpen;
                        break;
                    case YakuPivot.Chankan:
                        isIn = IsChankan;
                        break;
                    case YakuPivot.Chiihou:
                        isIn = IsInitialDraw && TurnWind != WindPivot.East;
                        break;
                    case YakuPivot.DoubleRiichi:
                        isIn = IsDoubleRiichi;
                        break;
                    case YakuPivot.Haitei:
                        isIn = IsHaitei;
                        break;
                    case YakuPivot.Ippatsu:
                        isIn = IsIppatsu;
                        break;
                    case YakuPivot.Riichi:
                        isIn = IsRiichi && !IsDoubleRiichi;
                        break;
                    case YakuPivot.RinshanKaihou:
                        isIn = IsRinshankaihou;
                        break;
                    case YakuPivot.Tenhou:
                        isIn = IsInitialDraw && TurnWind == WindPivot.East;
                        break;
                }
                if (isIn)
                {
                    yakus.Add(yakuToCheck);
                }
            }

            return yakus;
        }

        // Checks if the hand is chuuren poutou
        private bool IsChuurenPoutou()
        {
            // TODO : check than you can't have this hand with a kan of 9 or 1.
            if (IsOpen || ConcealedKans.Count > 0)
            {
                return false;
            }

            if (!UnsetTiles.All(t => t.Family == UnsetTiles.First().Family)
                || UnsetTiles.Any(t => t.IsHonor))
            {
                return false;
            }

            if (UnsetTiles.Count(t => t.Number == 1) < 3
                || UnsetTiles.Count(t => t.Number == 9) < 3)
            {
                return false;
            }

            for (int i = 2; i <= 8; i++)
            {
                if (!UnsetTiles.Any(t => t.Number == i))
                {
                    return false;
                }
            }

            return true;
        }

        // Checks if the hand is Kokushi Musou.
        private bool IsKokushiMusou()
        {
            if (ConcealedKans.Count > 0 || IsOpen || ConcealedTiles.Count < 14)
            {
                return false;
            }

            return ConcealedTiles.All(t => t.IsTerminalOrHonor)
                // Doesn't contain a pon.
                && !ConcealedTiles
                    .GroupBy(t => t)
                    .Select(grp => new KeyValuePair<TilePivot, int>(grp.Key, grp.Count()))
                    .Any(kvp => kvp.Value > 2)
                // Contains a single pair.
                && ConcealedTiles
                    .GroupBy(t => t)
                    .Select(grp => new KeyValuePair<TilePivot, int>(grp.Key, grp.Count()))
                    .Count(kvp => kvp.Value == 2) == 1;
        }

        // Checks if the hand is Chiitoitsu, and returns the list of sets.
        private List<SetPivot> GetChiitoitsuCombination()
        {
            List<SetPivot> sets = new List<SetPivot>();

            if (IsOpen || ConcealedKans.Count > 0)
            {
                return sets;
            }
            
            List<TilePivot> passed = new List<TilePivot>();
            for (int i = 0; i < UnsetTiles.Count; i++)
            {
                if (i % 2 == 1 && UnsetTiles.ElementAt(i).Equals(UnsetTiles.ElementAt(i - 1))
                    && !passed.Contains(UnsetTiles.ElementAt(i)))
                {
                    sets.Add(new SetPivot(UnsetTiles.ElementAt(i - 1), UnsetTiles.ElementAt(i)));
                    passed.Add(UnsetTiles.ElementAt(i - 1));
                }
            }
            return sets;
        }

        // Computes the list of valid combinations for this hand (regardless of concealed or not). Doesn't include Kokushi Musou and Chiitoiitsu.
        private List<List<SetPivot>> ComputeValidCombinations()
        {
            List<List<SetPivot>> listOf_FourthSetsAndAPair = new List<List<SetPivot>>();

            // Treats each family of tiles (dragon, wind, bamboo...) and begins with dragons and winds (do not change the OrderBy).
            foreach (var family in UnsetTiles.GroupBy(t => t.Family).OrderByDescending(t => (int)t.Key))
            {
                switch (family.Key)
                {
                    case FamilyPivot.Dragon:
                        List<SetPivot> setsOfDragons = new List<SetPivot>();
                        foreach (var dragon in family.GroupBy(t => t.Dragon.Value))
                        {
                            if (!TryExtractSetsFromWindOrDragonFamily(setsOfDragons, dragon.ToList()))
                            {
                                goto exitloop;
                            }
                        }
                        AddWindOrDragonSetsToListOfSetsList(listOf_FourthSetsAndAPair, setsOfDragons);
                        break;
                    case FamilyPivot.Wind:
                        List<SetPivot> setsOfWinds = new List<SetPivot>();
                        foreach (var wind in family.GroupBy(t => t.Wind.Value))
                        {
                            if (!TryExtractSetsFromWindOrDragonFamily(setsOfWinds, wind.ToList()))
                            {
                                goto exitloop;
                            }
                        }
                        AddWindOrDragonSetsToListOfSetsList(listOf_FourthSetsAndAPair, setsOfWinds);
                        break;
                    default:
                        if (_forbiddenCounts.Contains(family.Count()))
                        {
                            goto exitloop;
                        }

                        // Make sure to keep the OrderBy !
                        List<TilePivot> baseTilesOfFamily = family.OrderBy(t => t).ToList();

                        // If we already have partial sets list and the current family contains a pair.
                        if (listOf_FourthSetsAndAPair.Count > 0 && _withPairCounts.Contains(baseTilesOfFamily.Count))
                        {
                            // We need to remove all the sets list which already contains a pair.
                            listOf_FourthSetsAndAPair.RemoveAll(tsl => tsl.Any(tl => tl.IsPair));
                            // If, by doing so, we remove all the sets list, then we have to stop completely.
                            if (listOf_FourthSetsAndAPair.Count == 0)
                            {
                                goto exitloop;
                            }
                        }

                        // Computes every possible sets for each tile (but no reverse loop on tiles already processed).
                        List<List<List<int>>> setsForEachTile = new List<List<List<int>>>();
                        for (int i = 0; i < baseTilesOfFamily.Count - 1; i++)
                        {
                            List<List<int>> possibleSetsForAtile = GetPossibleSetsForATile(baseTilesOfFamily, i);
                            if (possibleSetsForAtile.Count > 0)
                            {
                                setsForEachTile.Add(possibleSetsForAtile);
                            }
                        }

                        if (setsForEachTile.Count == 0)
                        {
                            goto exitloop;
                        }

                        List<List<List<int>>> listOfPossibleSetIndexes = FilterPossibleSetsCombinationFromBaseSetsList(baseTilesOfFamily, setsForEachTile);

                        AddCartesianListOfSetsToListOfSetsList(ref listOf_FourthSetsAndAPair,
                            listOfPossibleSetIndexes
                                .Select(indexList => FromListIndexToSetList(indexList, baseTilesOfFamily))
                                .ToList());

                        break;
                }
            }
            exitloop:

            listOf_FourthSetsAndAPair.ForEach(sets =>
            {
                sets.AddRange(ConcealedKans);
                sets.AddRange(OpenedSets);
            });

            return listOf_FourthSetsAndAPair.Distinct(new SetListComparerPivot()).ToList();
        }

        #endregion Hands validation

        #region Helper static methods

        // Filters, from a list of sets (by index) for each tile, the list of sets which uses every tiles.
        private static List<List<List<int>>> FilterPossibleSetsCombinationFromBaseSetsList(List<TilePivot> tiles, List<List<List<int>>> setsIndexes)
        {
            int expectedCountSets = tiles.Count / 3 + (tiles.Count % 3 == 0 ? 0 : 1);

            // Makes flat the list of possible sets for each tiles.
            List<List<int>> setsList = setsIndexes.SelectMany(ts => ts).ToList();

            HashSet<HashSet<List<int>>>[] globals = new HashSet<HashSet<List<int>>>[expectedCountSets];
            for (int i = 0; i < expectedCountSets; i++)
            {
                globals[i] = null;
            }
            HashSet<HashSet<List<int>>> baseListOfSets = Combinations(new HashSet<List<int>>(setsList), expectedCountSets, globals);

            List<List<List<int>>> wellFormedSets = new List<List<List<int>>>(
                baseListOfSets
                    .Where(sets => sets.Count(set => set.Count == 2) < 2)
                    .Where(sets => sets.Sum(set => set.Count) == tiles.Count)
                    .Select(sets => new List<List<int>>(sets)));

            return wellFormedSets;
        }

        // Computes every combinations of k elements taken from alphabet.
        private static HashSet<HashSet<List<int>>> Combinations(HashSet<List<int>> alphabet, int k, HashSet<HashSet<List<int>>>[] globals)
        {
            if (k == 0)
            {
                var result = new HashSet<HashSet<List<int>>>
                {
                    new HashSet<List<int>>()
                };
                if (globals[0] == null)
                {
                    globals[0] = result;
                }
                return result;
            }
            else
            {
                var result = new HashSet<HashSet<List<int>>>();
                foreach (var e in alphabet)
                {
                    var newAlphabet = new HashSet<List<int>>(alphabet);
                    newAlphabet.Remove(e);
                    var smallerCombinations = globals[k - 1] ?? Combinations(newAlphabet, k - 1, globals);
                    var useFullCombination = smallerCombinations.Where(sets => !sets.SelectMany(x => x).Any(x => e.Contains(x))).ToList();
                    foreach (var smallerCombination in useFullCombination)
                    {
                        var combination = new HashSet<List<int>>(smallerCombination) { e };
                        result.Add(combination);
                    }
                }

                //result.RemoveAll(xx => xx.SelectMany(x => x).Distinct().Count() != xx.SelectMany(x => x).Count());
                if (k < globals.Length && globals[k] == null)
                {
                    globals[k] = result;
                }
                return result;
            }
        }

        // Transforms a list of indexes to a list of sets.
        private static List<SetPivot> FromListIndexToSetList(List<List<int>> setIndexesList, List<TilePivot> tiles)
        {
            List<SetPivot> sets = new List<SetPivot>();

            foreach (List<int> setIndex in setIndexesList)
            {
                sets.Add(new SetPivot(setIndex.Select(i => tiles[i]).ToArray()));
            }

            return sets;
        }

        // Computes, for the tile at the specified index of the list, every possible sets in the list. Result as list of indexes list, not tiles list.
        private static List<List<int>> GetPossibleSetsForATile(List<TilePivot> baseTilesOfFamily, int i)
        {
            TilePivot currentTile = baseTilesOfFamily[i];

            // Indexes of other tiles of the list which can be used for pairs or pons.
            List<int> indexOfDoublons = new List<int>();
            // Indexes of other tiles of the list which can be used for second tile of a chi.
            List<int> indexOfNext1Num = new List<int>();
            // Indexes of other tiles of the list which can be used for third tile of a chi.
            List<int> indexOfNext2Num = new List<int>();

            for (int j = (i + 1); j < baseTilesOfFamily.Count; j++)
            {
                if (currentTile.Equals(baseTilesOfFamily[j]))
                {
                    indexOfDoublons.Add(j);
                }
                else if (currentTile.Number == (baseTilesOfFamily[j].Number - 1))
                {
                    indexOfNext1Num.Add(j);
                }
                else if (currentTile.Number == (baseTilesOfFamily[j].Number - 2))
                {
                    indexOfNext2Num.Add(j);
                }
            }

            // Creates list of pairs.
            List<List<int>> listOfPairs = new List<List<int>>();
            foreach (int doublonIndex in indexOfDoublons)
            {
                listOfPairs.Add(new List<int> { i, doublonIndex });
            }

            // Creates list of pons.
            List<List<int>> listOfPons = new List<List<int>>();
            foreach (List<int> pair in listOfPairs)
            {
                foreach (int doublonIndex in indexOfDoublons.Where(ind => !pair.Contains(ind)))
                {
                    listOfPons.Add(new List<int>(pair) { doublonIndex });
                }
            }

            // Creates list of chis.
            List<List<int>> listOfChis = indexOfNext1Num.SelectMany(num1 =>
                indexOfNext2Num, (num1, num2) => new List<int> { i, num1, num2 }).ToList();

            return listOfPairs.Concat(listOfPons).Concat(listOfChis).ToList();
        }

        // Creates a cartesian product of list of sets list 
        private static void AddCartesianListOfSetsToListOfSetsList(ref List<List<SetPivot>> listOf_FourthSetsAndAPair, List<List<SetPivot>> setsListFromFamily)
        {
            if (listOf_FourthSetsAndAPair.Count == 0)
            {
                listOf_FourthSetsAndAPair.AddRange(setsListFromFamily);
            }
            else
            {
                listOf_FourthSetsAndAPair = listOf_FourthSetsAndAPair
                    .SelectMany(x => setsListFromFamily, (x, y) => x.Concat(y).ToList())
                    .ToList();
            }
        }

        // Adds a list of dragon or wind sets to a pre-existent list of sets list.
        private static void AddWindOrDragonSetsToListOfSetsList(List<List<SetPivot>> setsListList, List<SetPivot> setsList)
        {
            if (setsListList.Count > 0)
            {
                setsListList.ForEach(x => x.AddRange(setsList));
            }
            else
            {
                setsListList.Add(setsList);
            }
        }

        // Tries to extract each set from the lsit of tiles and adds it to the sets list if success.
        private static bool TryExtractSetsFromWindOrDragonFamily(List<SetPivot> setsList, List<TilePivot> tilesList)
        {
            if (_forbiddenCounts.Contains(tilesList.Count))
            {
                return false;
            }
            
            if (_withPairCounts.Contains(tilesList.Count))
            {
                // Extracts pair, then removes both tiles from the list.
                TilePivot pairTile = tilesList.GroupBy(t => t).Single(kvp => kvp.Count() == 2).Key;
                setsList.Add(new SetPivot(tilesList.Where(t => t.Equals(pairTile)).ToArray()));
                tilesList.RemoveAll(t => t == pairTile);

                CreateSetOfThreeTiles(setsList, tilesList);
            }
            else
            {
                CreateSetOfThreeTiles(setsList, tilesList);
            }

            return true;
        }

        // Creates sets of 3 tiles from the tiles list and adds them to the sets list.
        private static void CreateSetOfThreeTiles(List<SetPivot> setsList, List<TilePivot> tilesList)
        {
            for (int i = 0; i < tilesList.Count / 3; i++)
            {
                setsList.Add(new SetPivot(tilesList.Skip(i * 3).Take(3).ToArray()));
            }
        }

        #endregion Helper static methods
    }
}
