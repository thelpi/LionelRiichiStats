using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MahjongDll.Pivot;
using MahjongHandAnalyzer.Datas;

namespace MahjongHandAnalyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        private DrawPivot _draw;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            _draw = new DrawPivot();

            for (int i = 1; i <= 14; i++)
            {
                GetCombo(i).ItemsSource = _draw.UniqueTiles;
            }

            IEnumerable<WindPivot> winds = Enum.GetValues(typeof(WindPivot)).OfType<WindPivot>();
            CbbDominantWind.ItemsSource = winds.Take(2);
            CbbSeatWind.ItemsSource = winds;

            ResetForm();
        }

        #region Actions

        private void BtnAnalyze_Click(object sender, RoutedEventArgs e)
        {
            if (!ExtractFromForm(out List<TilePivot> handTiles, out TilePivot latestTile, out WindPivot dominantWind, out WindPivot seatWind))
            {
                return;
            }

            // HERE
            LoadWindow wdw = new LoadWindow(AsyncCompute, new object[]
            {
                 handTiles, dominantWind, seatWind, latestTile, _draw
            });
            wdw.ShowDialog();
            var resultsAr = wdw.ReturnValue as object[];
            
            var orderedRes = resultsAr[0] as Dictionary<int, List<SubstitutionGroup>>;
            var discardprobabilities = resultsAr[1] as Dictionary<TilePivot, double>;

            // display tile away0
            SetContentFromHandListYaku(Tbi0, orderedRes[0].First());
            // display tile away1
            DisplayTile1Away(orderedRes[1], discardprobabilities);
            // display tile away2
            DisplayTile2Away(orderedRes[2]);

            GrbResults.Visibility = Visibility.Visible;
        }

        private static void AsyncCompute(object sender, DoWorkEventArgs e)
        {
            var arguments = e.Argument as object[];
            var handTiles = (List<TilePivot>)arguments[0];
            var dominantWind = (WindPivot)arguments[1];
            var seatWind = (WindPivot)arguments[2];
            var latestTile = (TilePivot)arguments[3];
            var draw = (DrawPivot)arguments[4];

            Dictionary<int, List<SubstitutionGroup>> subsGroupByAwayIndex = new Dictionary<int, List<SubstitutionGroup>>();

            // compute tile0
            FullHandPivot hand0 = new FullHandPivot(handTiles, dominantWind, seatWind, latestTile);
            HandYakuListPivot handYakus = hand0.ComputeHandYakus()?.FirstOrDefault();
            subsGroupByAwayIndex.Add(0, new List<SubstitutionGroup>
            {
                new SubstitutionGroup(handYakus)
            });

            #region 1 tile away

            var handTileWithLast = new List<TilePivot>(handTiles)
            {
                latestTile
            };
            List<TilePivot> availableTiles = draw.ComputeRemainingTiles(handTileWithLast);

            // gets yakus for each substitution combination
            var rawResultsArray = BackgroundHandlerForOneTileAway(dominantWind, seatWind, handTileWithLast, availableTiles, new List<TilePivot>());
            var rawResults = rawResultsArray.Item1;

            subsGroupByAwayIndex.Add(1, rawResults.OrderByDescending(x => x.Probability).ToList());

            // per discard solutions
            var discardprobabilities = new Dictionary<TilePivot, double>();
            /* 
             TilePivot currentDiscard = null;
             double probability = 0;
             foreach (var r in rawResults.OrderBy(x => x.SubstitutionSequences.First().Substitutions.First().Subbed))
             {
                 if (currentDiscard != null && !r.Item1.Subbed.Equals(currentDiscard))
                 {
                     discardprobabilities.Add(currentDiscard, Math.Round(probability * 100, 3));
                     probability = 0;
                 }
                 currentDiscard = r.Item1.Subbed;
                 probability += availableTiles.Count(_ => _.Equals(currentDiscard)) / (double)availableTiles.Count;
             }
             if (currentDiscard != null)
             {
                 discardprobabilities.Add(currentDiscard, Math.Round(probability * 100, 3));
             }*/

            #endregion 1 tile away

            #region 2 tiles away

            // gets yakus for each substitution combination
            var rawResults2Away = new List<SubstitutionGroup>();
            var firstRoundSubstitutionsList = rawResultsArray.Item2;
            foreach (var firstSubstitution in firstRoundSubstitutionsList)
            {
                var usedTiles = new List<TilePivot>(handTiles)
                {
                    latestTile
                };
                usedTiles.Add(firstSubstitution.Subber);
                var availableTilesAway2 = draw.ComputeRemainingTiles(usedTiles);
                usedTiles.Remove(firstSubstitution.Subbed);

                rawResultsArray = BackgroundHandlerForOneTileAway(dominantWind, seatWind, usedTiles, availableTilesAway2, new List<TilePivot> { firstSubstitution.Subbed });
                var subGroupList = rawResultsArray.Item1;
                foreach (var subGroup in rawResultsArray.Item1)
                {
                    subGroup.AddSubstitutionToEachSequences(firstSubstitution);
                    rawResults2Away.Add(subGroup);
                }
            }

            var resultsAway2WithoutDuplicates = new List<SubstitutionGroup>();
            foreach (var r in rawResults2Away)
            {
                var match = resultsAway2WithoutDuplicates.FirstOrDefault(ra => ra.Yakus.Equals(r.Yakus));
                if (match == null)
                {
                    resultsAway2WithoutDuplicates.Add(r);
                }
                else
                {
                    foreach (var seq in r.SubstitutionSequences)
                    {
                        match.AddSubstitutionSequence(seq);
                    }
                }
            }

            subsGroupByAwayIndex.Add(2, resultsAway2WithoutDuplicates.OrderByDescending(x => x.Probability).ToList());

            #endregion 2 tiles away

            e.Result = new object[]
            {
                subsGroupByAwayIndex, discardprobabilities
            };
        }

        private static Dictionary<HandYakuListPivot, List<List<Substitution>>>
            Flat2(List<Tuple<HandYakuListPivot, List<Substitution>>> rawResults2Away)
        {
            // flats the raw results
            var groupResults2Away = new Dictionary<HandYakuListPivot, List<List<Substitution>>>();
            foreach (var rawResult2Away in rawResults2Away)
            {
                var key = groupResults2Away.Keys.FirstOrDefault(k => k.Equals(rawResult2Away.Item1));
                if (key == null)
                {
                    key = rawResult2Away.Item1;
                    groupResults2Away.Add(key, new List<List<Substitution>>());
                }
                groupResults2Away[key].Add(rawResult2Away.Item2);
            }

            return groupResults2Away;
        }

        private static Dictionary<HandYakuListPivot, List<Substitution>>
            Flat(List<Tuple<Substitution, HandYakuListPivot>> rawResults)
        {
            // flats the raw results
            var groupResults = new Dictionary<HandYakuListPivot, List<Substitution>>();
            foreach (var rawResult in rawResults)
            {
                var key = groupResults.Keys.FirstOrDefault(k => k.Equals(rawResult.Item2));
                if (key == null)
                {
                    key = rawResult.Item2;
                    groupResults.Add(key, new List<Substitution>());
                }
                groupResults[key].Add(rawResult.Item1);
            }

            return groupResults;
        }

        private void DisplayTile2Away(List<SubstitutionGroup> orderedResAway2)
        {
            int i = 0;
            StackPanel spSolutionsAway2 = new StackPanel { Orientation = Orientation.Horizontal };
            spSolutionsAway2.SetValue(DockPanel.DockProperty, Dock.Top);
            foreach (var group in orderedResAway2)
            {
                i++;
                GroupBox gbSingle = new GroupBox { Header = $"Solution {i}" };
                SetContentFromHandListYaku(gbSingle, group);
                spSolutionsAway2.Children.Add(gbSingle);
            }

            // per discard solutions
            StackPanel spPerDiscardAway2 = new StackPanel { Orientation = Orientation.Vertical };

            // TODO

            GroupBox gbPerDiscardAway2 = new GroupBox { Header = "Win prob. per discard", Content = spPerDiscardAway2 };
            gbPerDiscardAway2.SetValue(DockPanel.DockProperty, Dock.Bottom);

            DockPanel fullResultsAway2 = new DockPanel();
            fullResultsAway2.Children.Add(gbPerDiscardAway2);
            fullResultsAway2.Children.Add(spSolutionsAway2);
            Tbi2.Content = fullResultsAway2;
        }

        private void DisplayTile1Away(List<SubstitutionGroup> orderedRes,
            Dictionary<TilePivot, double>  discardprobabilities)
        {
            int j = 0;
            StackPanel spSolutions = new StackPanel { Orientation = Orientation.Horizontal };
            spSolutions.SetValue(DockPanel.DockProperty, Dock.Top);
            foreach (var subGroup in orderedRes)
            {
                j++;
                GroupBox gbSingle = new GroupBox { Header = $"Solution {j}" };
                SetContentFromHandListYaku(gbSingle, subGroup);
                spSolutions.Children.Add(gbSingle);
            }

            // tile1 "per discard" display
            StackPanel spPerDiscard = new StackPanel { Orientation = Orientation.Vertical };
            foreach (var discardprobability in discardprobabilities.OrderByDescending(x => x.Value))
            {
                spPerDiscard.Children.Add(new TextBlock
                {
                    Text = $"{discardprobability.Key} with a prob. of {discardprobability.Value} %"
                });
            }

            // tile1
            GroupBox gbPerDiscard = new GroupBox { Header = "Win prob. per discard", Content = spPerDiscard };
            gbPerDiscard.SetValue(DockPanel.DockProperty, Dock.Bottom);

            DockPanel fullResults = new DockPanel();
            fullResults.Children.Add(gbPerDiscard);
            fullResults.Children.Add(spSolutions);
            Tbi1.Content = fullResults;
        }

        private static Tuple<List<SubstitutionGroup>, List<Substitution>> BackgroundHandlerForOneTileAway(WindPivot dominantWind, WindPivot seatWind,
            List<TilePivot> handTileWithLast, List<TilePivot> availableTiles, List<TilePivot> forbiddenTiles)
        {
            var substitutionsAttemps = new List<Substitution>();
            var rawResults = new List<SubstitutionGroup>();

            foreach (TilePivot subbedTile in handTileWithLast.Distinct())
            {
                foreach (TilePivot subTile in availableTiles.Distinct())
                {
                    if (subbedTile.Equals(subTile) || forbiddenTiles.Contains(subTile))
                    {
                        continue;
                    }

                    substitutionsAttemps.Add(new Substitution(subbedTile, subTile, availableTiles));

                    List<TilePivot> handTilesWithSub = new List<TilePivot>(handTileWithLast);
                    handTilesWithSub.Remove(subbedTile);

                    var hand1 = new FullHandPivot(handTilesWithSub, dominantWind, seatWind, subTile);

                    HandYakuListPivot tmpResults = hand1.ComputeHandYakus()?.FirstOrDefault();
                    if (tmpResults != null)
                    {
                        var subSeq = new SubstitutionSequence();
                        subSeq.AddSubstitution(new Substitution(subbedTile, subTile, availableTiles));

                        var finded = rawResults.FirstOrDefault(x => x.Yakus.Equals(tmpResults));
                        if (finded == null)
                        {
                            rawResults.Add(new SubstitutionGroup(tmpResults));
                            finded = rawResults.Last();
                        }
                        finded.AddSubstitutionSequence(subSeq);
                    }
                }
            }

            return new Tuple<List<SubstitutionGroup>, List<Substitution>>(rawResults, substitutionsAttemps);
        }

        private void BtnRandomize_Click(object sender, RoutedEventArgs e)
        {
            List<TilePivot> handTiles = _draw.PickHandOfTiles().ToList();
            int indexOfLastPick = _draw.Randomizer.Next(1, 15);
            for (int i = 1; i <= 14; i++)
            {
                GetCombo(i).SelectedItem = handTiles[i - 1];
                GetRadio(i).IsChecked = indexOfLastPick == i;
            }
            CbbDominantWind.SelectedIndex = _draw.Randomizer.Next(0, 2);
            CbbSeatWind.SelectedIndex = _draw.Randomizer.Next(0, 4);
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        #endregion Actions

        private ComboBox GetCombo(int i)
        {
            return FindName(string.Format("CbbTile{0}", (i < 10 ? "0" : "") + i.ToString())) as ComboBox;
        }

        private RadioButton GetRadio(int i)
        {
            return FindName(string.Format("RdbLastPick{0}", (i < 10 ? "0" : "") + i.ToString())) as RadioButton;
        }

        private void ResetForm()
        {
            for (int i = 1; i <= 14; i++)
            {
                GetCombo(i).SelectedIndex = -1;
                GetRadio(i).IsChecked = i == 14;
            }
            CbbDominantWind.SelectedIndex = 0;
            CbbSeatWind.SelectedIndex = 0;
            GrbResults.Visibility = Visibility.Collapsed;
            Tbi0.IsSelected = true;
            Tbi0.Content = null;
            Tbi1.Content = null;
            Tbi2.Content = null;
        }

        private bool ExtractFromForm(out List<TilePivot> tiles, out TilePivot latestPickTile, out WindPivot dominantWind, out WindPivot seatWind)
        {
            tiles = new List<TilePivot>();
            latestPickTile = null;
            dominantWind = WindPivot.East;
            seatWind = WindPivot.East;

            for (int i = 1; i <= 14; i++)
            {
                if (GetCombo(i).SelectedItem == null)
                {
                    MessageBox.Show("Some slots are empty.", "LionelRiichiStats - Error");
                    return false;
                }
                TilePivot tile = GetCombo(i).SelectedItem as TilePivot;
                if (GetRadio(i).IsChecked == true && latestPickTile == null)
                {
                    latestPickTile = tile;
                }
                else
                {
                    tiles.Add(tile);
                }
            }
            if (latestPickTile == null)
            {
                MessageBox.Show("The latest tile has not been selected.", "LionelRiichiStats - Error");
                return false;
            }
            if (tiles.Concat(new List<TilePivot> { latestPickTile }).GroupBy(t => t).Any(tg => tg.Count() > 4))
            {
                MessageBox.Show("One tile is selected more than four times.", "LionelRiichiStats - Error");
                return false;
            }
            if (CbbDominantWind.SelectedIndex == -1)
            {
                MessageBox.Show("The dominant wind has not been selected.", "LionelRiichiStats - Error");
                return false;
            }
            if (CbbSeatWind.SelectedIndex == -1)
            {
                MessageBox.Show("The seat wind has not been selected.", "LionelRiichiStats - Error");
                return false;
            }
            dominantWind = (WindPivot)CbbDominantWind.SelectedItem;
            seatWind = (WindPivot)CbbSeatWind.SelectedItem;

            return true;
        }

        private void SetContentFromHandListYaku(ContentControl container, SubstitutionGroup subGroup)
        {
            if (subGroup?.Yakus == null)
            {
                container.Content = new TextBlock
                {
                    Text = "Shanten"
                };
            }
            else
            {
                StackPanel sp = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Margin = new Thickness(5, 0, 0, 0)
                };
                sp.Children.Add(new TextBlock
                {
                    Text = subGroup.Yakus.FansName,
                    FontSize = 12,
                    Foreground = System.Windows.Media.Brushes.Red
                });
                if (subGroup.Probability < 1)
                {
                    sp.Children.Add(new TextBlock
                    {
                        Text = $"Prob. of {Math.Round(subGroup.Probability * 100, 3)} %",
                        FontSize = 12,
                        Foreground = System.Windows.Media.Brushes.DarkSlateBlue
                    });
                }

                StackPanel spYakus = new StackPanel { Orientation = Orientation.Vertical };
                foreach (YakuPivot yaku in subGroup.Yakus.Yakus)
                {
                    spYakus.Children.Add(new TextBlock
                    {
                        Text = $"{yaku.Name} ({yaku.FansConcealed})",
                        ToolTip = yaku.Description
                    });
                }

                GroupBox gbYakus = new GroupBox { Header = "Yakus", Content = spYakus };
                sp.Children.Add(gbYakus);

                if (subGroup.SubstitutionSequences.Any())
                {
                    StackPanel spSubs = new StackPanel { Orientation = Orientation.Vertical };
                    foreach (var sub in subGroup.SubstitutionSequences)
                    {
                        spSubs.Children.Add(new TextBlock
                        {
                            Text = string.Join(", then ", sub.Substitutions.Select(x => $"discard/pick {x.Subbed}/{x.Subber}").ToArray())
                        });
                    }

                    GroupBox gbSubs = new GroupBox { Header = "Potential discards & picks", Content = spSubs };
                    sp.Children.Add(gbSubs);
                }

                container.Content = sp;
            }
        }

        private void BtnFillKokushiIichanten_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i <= 14; i++)
            {
                switch (i)
                {
                    case 1:
                        GetCombo(i).SelectedItem = _draw.Tiles.First(t => t.Family == FamilyPivot.Character && t.Number == 1);
                        break;
                    case 2:
                        GetCombo(i).SelectedItem = _draw.Tiles.First(t => t.Family == FamilyPivot.Character && t.Number == 9);
                        break;
                    case 3:
                        GetCombo(i).SelectedItem = _draw.Tiles.First(t => t.Family == FamilyPivot.Circle && t.Number == 1);
                        break;
                    case 4:
                        GetCombo(i).SelectedItem = _draw.Tiles.First(t => t.Family == FamilyPivot.Circle && t.Number == 9);
                        break;
                    case 5:
                        GetCombo(i).SelectedItem = _draw.Tiles.First(t => t.Family == FamilyPivot.Bamboo && t.Number == 1);
                        break;
                    case 6:
                        GetCombo(i).SelectedItem = _draw.Tiles.First(t => t.Family == FamilyPivot.Bamboo && t.Number == 9);
                        break;
                    case 7:
                        GetCombo(i).SelectedItem = _draw.Tiles.First(t => t.Family == FamilyPivot.Dragon && t.Dragon == DragonPivot.Green);
                        break;
                    case 8:
                        GetCombo(i).SelectedItem = _draw.Tiles.First(t => t.Family == FamilyPivot.Dragon && t.Dragon == DragonPivot.Red);
                        break;
                    case 9:
                        GetCombo(i).SelectedItem = _draw.Tiles.First(t => t.Family == FamilyPivot.Dragon && t.Dragon == DragonPivot.White);
                        break;
                    case 10:
                        GetCombo(i).SelectedItem = _draw.Tiles.First(t => t.Family == FamilyPivot.Wind && t.Wind == WindPivot.East);
                        break;
                    case 11:
                        GetCombo(i).SelectedItem = _draw.Tiles.First(t => t.Family == FamilyPivot.Wind && t.Wind == WindPivot.West);
                        break;
                    case 12:
                        GetCombo(i).SelectedItem = _draw.Tiles.First(t => t.Family == FamilyPivot.Wind && t.Wind == WindPivot.South);
                        break;
                    case 13:
                        GetCombo(i).SelectedItem = _draw.Tiles.First(t => t.Family == FamilyPivot.Character && t.Number == 4);
                        break;
                    case 14:
                        GetCombo(i).SelectedItem = _draw.Tiles.First(t => t.Family == FamilyPivot.Character && t.Number == 7);
                        break;
                }
            }
        }
    }
}
