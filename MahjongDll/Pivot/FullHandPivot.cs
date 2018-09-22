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
            if (lastTile == null)
            {
                throw new ArgumentNullException(nameof(lastTile));
            }

            tiles = tiles ?? new List<TilePivot>();
            openedSets = openedSets ?? new List<SetPivot>();
            concealedKans = concealedKans ?? new List<SetPivot>();

            if ((openedSets.Count * 3 + concealedKans.Count * 3 + 1 + tiles.Count) != 14)
            {
                throw new ArgumentException(Messages.InvalidTilesCountInHandError);
            }

            if (isRon && isRinshankaihou)
            {
                throw new ArgumentException(Messages.InvalidHandArgumentsConsistencyError, nameof(isRon));
            }

            if (openedSets.Count > 0 && (isRiichi || isDoubleRiichi || isIppatsu || openedSets.Any(set => set.IsPair)))
            {
                throw new ArgumentException(Messages.InvalidHandArgumentsConsistencyError, nameof(openedSets));
            }

            if (concealedKans.Count > 0 && concealedKans.Any(set => !set.IsKan))
            {
                throw new ArgumentException(Messages.InvalidHandArgumentsConsistencyError, nameof(concealedKans));
            }

            if (isRiichi && isDoubleRiichi)
            {
                throw new ArgumentException(Messages.InvalidHandArgumentsConsistencyError, nameof(isRiichi));
            }

            // TODO : checks this rule.
            if (isIppatsu && (isRinshankaihou || isChankan))
            {
                throw new ArgumentException(Messages.InvalidHandArgumentsConsistencyError, nameof(isIppatsu));
            }

            // TODO : checks this rule.
            // TODO : checks isChankan.
            if (isHaitei && isRinshankaihou)
            {
                throw new ArgumentException(Messages.InvalidHandArgumentsConsistencyError, nameof(isHaitei));
            }

            if (isRinshankaihou && isChankan)
            {
                throw new ArgumentException(Messages.InvalidHandArgumentsConsistencyError, nameof(isRinshankaihou));
            }

            ConcealedTiles = tiles.OrderBy(t => t).ToList();
            DominantWind = dominantWind;
            TurnWind = turnWind;
            LastTile = lastTile;
            IsRon = isRon;
            OpenedSets = openedSets;
            ConcealedKans = concealedKans;
            IsRiichi = isRiichi;
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
        /// <param name="turnWind"><see cref="TurnWind"/>.</param>
        /// <exception cref="ArgumentException"><see cref="Messages.InvalidTilesCountInHandError"/>.</exception>
        public FullHandPivot(List<TilePivot> tiles, WindPivot turnWind)
        {
            tiles = tiles ?? new List<TilePivot>();

            if (tiles.Count != 14)
            {
                throw new ArgumentException(Messages.InvalidTilesCountInHandError);
            }

            ConcealedTiles = tiles.OrderBy(t => t).ToList();
            DominantWind = turnWind; // whatever
            TurnWind = turnWind;
            OpenedSets = new List<SetPivot>();
            ConcealedKans = new List<SetPivot>();
            IsInitialDraw = true;
        }

        /// <summary>
        /// Computes the list of <see cref="YakuPivot"/> which can be made with this hand.
        /// </summary>
        /// <param name="getFlat">If <c>True</c>, every yakus are returned regardless of upgrades and incompatibilities.</param>
        /// <returns>List of <see cref="YakuPivot"/>.</returns>
        public List<YakuPivot> ComputeHandYakus(bool getFlat)
        {
            List<YakuPivot> yakus = new List<YakuPivot>();

            if (IsKokushiMusou())
            {
                yakus.Add(YakuPivot.Get(YakuPivot.KokushiMusou));
            }
            else
            {
                List<SetPivot> chiToiSets = GetChiitoitsuCombination();

                List<List<SetPivot>> combinationlist = ComputeValidCombinations();
                if (chiToiSets.Count == 7)
                {
                    yakus.Add(YakuPivot.Get(YakuPivot.Chiitoitsu));
                    combinationlist.Add(chiToiSets);
                }

                if (combinationlist.Count > 0)
                {
                    foreach (List<SetPivot> currentCombo in combinationlist)
                    {
                        foreach (YakuPivot yakuToCheck in YakuPivot.Yakus.OrderByDescending(c => c.FansConcealed))
                        {
                            bool isIn = false;
                            // For yakuhai only.
                            int yakuCumul = 1;
                            switch (yakuToCheck.Name)
                            {
                                case YakuPivot.Chantaiyao:
                                    isIn = currentCombo.All(c => c.IsHonorOrTerminal || c.IsTerminalChi);
                                    break;
                                case YakuPivot.Chinitsu:
                                    isIn = currentCombo.All(c => !c.IsHonor && c.Family == currentCombo.First().Family);
                                    break;
                                case YakuPivot.Chinroutou:
                                    isIn = currentCombo.All(c => c.IsTerminal);
                                    break;
                                case YakuPivot.ChuurenPoutou:
                                    isIn = IsChuurenPoutou(currentCombo);
                                    break;
                                case YakuPivot.Daisangen:
                                    isIn = currentCombo.Count(c => c.IsPonOrKan && c.IsDragon) == 3;
                                    break;
                                case YakuPivot.Daisuushi:
                                    isIn = currentCombo.Count(c => c.IsWind && c.IsPonOrKan) == 4;
                                    break;
                                case YakuPivot.Honitsu:
                                    isIn = currentCombo.All(c => c.IsHonor || (!c.IsHonor && c.Family == currentCombo.First(cBis => !cBis.IsHonor).Family));
                                    break;
                                case YakuPivot.Honroutou:
                                    isIn = currentCombo.All(c => c.IsHonorOrTerminal);
                                    break;
                                case YakuPivot.Iipeikou:
                                    isIn = currentCombo.Where(c => c.IsChi).GroupBy(c => new { c.FirstNumber, c.Family }).Any(c => c.Count() >= 2);
                                    break;
                                case YakuPivot.Ittsuu:
                                    isIn = currentCombo.Where(c => c.IsChi).GroupBy(c => c.Family).Any(c =>
                                        new int[] { 1, 4, 7 }.All(index => c.Any(set => set.FirstNumber == index)));
                                    break;
                                case YakuPivot.Junchantaiyao:
                                    isIn = currentCombo.All(c => c.IsTerminal || c.IsTerminalChi);
                                    break;
                                case YakuPivot.MenzenTsumo:
                                    isIn = !IsRon && OpenedSets.Count == 0;
                                    break;
                                case YakuPivot.Pinfu:
                                    isIn = currentCombo.All(c => c.IsChi || (c.IsPair && !c.IsDragon && c.Wind != TurnWind && c.Wind != DominantWind))
                                        && currentCombo.Any(c => c.IsChi && (c.Tiles.ElementAt(0).Equals(LastTile) || c.Tiles.ElementAt(2).Equals(LastTile)));
                                    break;
                                case YakuPivot.Ryanpeikou:
                                    isIn = currentCombo.Where(c => c.IsChi).GroupBy(c => new { c.FirstNumber, c.Family }).Count(c => c.Count() >= 2) > 1
                                        || currentCombo.Where(c => c.IsChi).GroupBy(c => new { c.FirstNumber, c.Family }).Any(c => c.Count() == 4);
                                    break;
                                case YakuPivot.Ryuuiisou:
                                    isIn = currentCombo.All(c => c.Dragon == DragonPivot.green
                                        || (c.Family == FamilyPivot.bamboo && c.Tiles.All(t => t.Number == 3 || t.Number % 2 == 0)));
                                    break;
                                case YakuPivot.Sanankou:
                                    isIn = currentCombo.Count(c => c.IsPonOrKan) >= 3;
                                    break;
                                case YakuPivot.Sankantsu:
                                    isIn = currentCombo.Count(c => c.IsKan) == 3;
                                    break;
                                case YakuPivot.SanshokuDoujun:
                                    isIn = currentCombo.Where(c => c.IsChi).GroupBy(c => c.FirstNumber).Any(c => c.Count() >= 3
                                        && new FamilyPivot[] { FamilyPivot.bamboo, FamilyPivot.character, FamilyPivot.circle }.All(f =>
                                            c.Any(set => set.Family == f)));
                                    break;
                                case YakuPivot.SanshokuDoukou:
                                    isIn = currentCombo.Where(c => c.IsPonOrKan && !c.IsHonor).GroupBy(c => c.FirstNumber).Any(c => c.Count() == 3);
                                    break;
                                case YakuPivot.Shousangen:
                                    isIn = currentCombo.Count(c => c.IsDragon && c.IsPonOrKan) == 2
                                        && currentCombo.Any(c => c.IsPair && c.IsDragon);
                                    break;
                                case YakuPivot.Shousuushi:
                                    isIn = currentCombo.Count(c => c.IsWind && c.IsPonOrKan) == 3
                                        && currentCombo.Any(c => c.IsPair && c.IsWind);
                                    break;
                                case YakuPivot.Suuankou:
                                    isIn = currentCombo.Count(c => c.IsPonOrKan) == 4;
                                    break;
                                case YakuPivot.Suukantsu:
                                    isIn = currentCombo.Count(c => c.IsKan) == 4;
                                    break;
                                case YakuPivot.Tanyao:
                                    isIn = currentCombo.All(c => !c.IsHonorOrTerminal && !c.IsTerminalChi);
                                    break;
                                case YakuPivot.Toitoi:
                                    isIn = currentCombo.Count(c => c.IsPonOrKan) == 4;
                                    break;
                                case YakuPivot.Tsuuiisou:
                                    isIn = currentCombo.All(c => c.IsHonor);
                                    break;
                                case YakuPivot.Yakuhai:
                                    yakuCumul = currentCombo.Count(c =>
                                        c.IsPonOrKan && (c.IsDragon || c.Wind == TurnWind || c.Wind == DominantWind));
                                    isIn = yakuCumul > 0;
                                    break;
                                case YakuPivot.Chankan:
                                    isIn = IsChankan;
                                    break;
                                case YakuPivot.Chiihou:
                                    isIn = IsInitialDraw && TurnWind != WindPivot.east;
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
                                    isIn = IsInitialDraw && TurnWind == WindPivot.east;
                                    break;
                            }
                            if (isIn)
                            {
                                for (int i = 1; i <= yakuCumul; i++)
                                {
                                    yakus.Add(yakuToCheck);
                                }
                            }
                        }
                    }
                }
            }

            return yakus;
        }

        // Checks if the current sets list represents the hand chuuren poutou
        private static bool IsChuurenPoutou(List<SetPivot> currentCombo)
        {
            List<TilePivot> flatTilesList = GetFlatTilesList(currentCombo);

            if (!currentCombo.All(c => !c.IsHonor
                && c.Family == currentCombo.First().Family
                && flatTilesList.Count(t => t.Number == 1) == 3
                && flatTilesList.Count(t => t.Number == 9) == 3))
            {
                return false;
            }

            for (int i = 2; i <= 8; i++)
            {
                if (!flatTilesList.Any(t => t.Number == i))
                {
                    return false;
                }
            }

            return true;
        }

        // Checks if the hand is Kokushi Musou.
        private bool IsKokushiMusou()
        {
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
            List<TilePivot> passed = new List<TilePivot>();
            for (int i = 0; i < ConcealedTiles.Count; i++)
            {
                if (i % 2 == 1 && ConcealedTiles.ElementAt(i).Equals(ConcealedTiles.ElementAt(i - 1))
                    && !passed.Contains(ConcealedTiles.ElementAt(i)))
                {
                    sets.Add(new SetPivot(ConcealedTiles.ElementAt(i - 1), ConcealedTiles.ElementAt(i)));
                    passed.Add(ConcealedTiles.ElementAt(i - 1));
                }
            }
            return sets;
        }

        // Computes the list of valid combinations for this hand (regardless of concealed or not). Doesn't include Kokushi Musou and Chiitoiitsu.
        private List<List<SetPivot>> ComputeValidCombinations()
        {
            List<List<SetPivot>> listOf_FourthSetsAndAPair = new List<List<SetPivot>>();

            // Treats each family of tiles (dragon, wind, bamboo...) and begins with dragons and winds (do not change the OrderBy).
            foreach (var family in ConcealedTiles.GroupBy(t => t.Family).OrderByDescending(t => (int)t.Key))
            {
                switch (family.Key)
                {
                    case FamilyPivot.dragon:
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
                    case FamilyPivot.wind:
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

            return listOf_FourthSetsAndAPair.Distinct(new SetListComparerPivot()).ToList();
        }

        #region Helper methods
        
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
                    .Select(sets => new List<List<int>>(sets)));

            return wellFormedSets;
        }

        // Computes every combinations of k elements taken from alphabet.
        private static HashSet<HashSet<List<int>>> Combinations(HashSet<List<int>> alphabet,
            int k, HashSet<HashSet<List<int>>>[] globals)
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

        // Creats a flat list of tiles from the list of tilesets.
        private static List<TilePivot> GetFlatTilesList(List<SetPivot> tilsets)
        {
            return tilsets.SelectMany(ts => ts.Tiles).ToList();
        }

        #endregion Helper methods
    }
}
