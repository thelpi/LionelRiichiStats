using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MahjongDll.Pivot;

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

            #region 0 tile

            FullHandPivot hand = new FullHandPivot(handTiles, dominantWind, seatWind, latestTile);
            HandYakuListPivot handYakus = hand.ComputeHandYakus()?.FirstOrDefault();
            SetContentFromHandListYaku(Tbi0, handYakus);

            #endregion 0 tile

            #region 1 tile

            // gets yakus for each substitution combination
            var rawResults = new List<Tuple<TilePivot, TilePivot, HandYakuListPivot>>();
            var alreadyDone = new List<Tuple<TilePivot, TilePivot>>();
            var handTileWithLast = new List<TilePivot>(handTiles)
            {
                latestTile
            };
            List<TilePivot> availableTiles = _draw.ComputeRemainingTiles(handTileWithLast, true);
            foreach (TilePivot subbedTile in handTileWithLast.Distinct())
            {
                foreach (TilePivot subTile in availableTiles)
                {
                    if (subbedTile.Equals(subTile)
                        && !alreadyDone.Any(_ => _.Item1 == subbedTile && _.Item2 == subTile))
                    {
                        continue;
                    }
                    alreadyDone.Add(new Tuple<TilePivot, TilePivot>(subbedTile, subTile));

                    List<TilePivot> handTilesWithSub = new List<TilePivot>(handTileWithLast);
                    handTilesWithSub.Remove(subbedTile);

                    hand = new FullHandPivot(handTilesWithSub, dominantWind, seatWind, subTile);

                    HandYakuListPivot tmpResults = hand.ComputeHandYakus()?.FirstOrDefault();
                    if (tmpResults != null)
                    {
                        rawResults.Add(new Tuple<TilePivot, TilePivot, HandYakuListPivot>(subbedTile, subTile, tmpResults));
                    }
                }
            }

            // flats the raw results
            var groupResults = new Dictionary<HandYakuListPivot, List<Tuple<TilePivot, TilePivot>>>();
            foreach (var rawResult in rawResults)
            {
                var key = groupResults.Keys.FirstOrDefault(k => k.Equals(rawResult.Item3));
                if (key == null)
                {
                    key = rawResult.Item3;
                    groupResults.Add(key, new List<Tuple<TilePivot, TilePivot>>());
                }
                groupResults[key].Add(new Tuple<TilePivot, TilePivot>(rawResult.Item1, rawResult.Item2));
            }

            // computes the probability of each HandYakuListPivot
            var resultsWithProb = new Dictionary<HandYakuListPivot, Tuple<double, List<Tuple<TilePivot, TilePivot>>>>();
            foreach (var key in groupResults.Keys)
            {
                var prob = groupResults[key].Sum(x => availableTiles.Count(_ => _.Equals(x.Item2)));
                resultsWithProb.Add(key, new Tuple<double, List<Tuple<TilePivot, TilePivot>>>(prob / (double)availableTiles.Count, groupResults[key]));
            }

            // displays ordered results
            var orderedRes = resultsWithProb
                                .OrderByDescending(x => x.Value.Item1)
                                .ToDictionary(x => x.Key, x => x.Value);
            int i = 0;
            StackPanel spSolutions = new StackPanel { Orientation = Orientation.Horizontal };
            spSolutions.SetValue(DockPanel.DockProperty, Dock.Top);
            foreach (var key in orderedRes.Keys)
            {
                i++;
                GroupBox gbSingle = new GroupBox { Header = $"Solution {i}" };
                SetContentFromHandListYaku(gbSingle, key, orderedRes[key].Item1, orderedRes[key].Item2);
                spSolutions.Children.Add(gbSingle);
            }

            // per discard solutions
            StackPanel spPerDiscard = new StackPanel { Orientation = Orientation.Vertical };

            var discardprobabilities = new Dictionary<TilePivot, double>();
            TilePivot currentDiscard = null;
            double probability = 0;
            foreach (var r in rawResults.OrderBy(x => x.Item1))
            {
                if (currentDiscard != null && !r.Item1.Equals(currentDiscard))
                {
                    discardprobabilities.Add(currentDiscard, Math.Round(probability * 100, 3));
                    probability = 0;
                }
                currentDiscard = r.Item1;
                probability += availableTiles.Count(_ => _.Equals(currentDiscard)) / (double)availableTiles.Count;
            }
            discardprobabilities.Add(currentDiscard, Math.Round(probability * 100, 3));

            foreach (var discardprobability in discardprobabilities.OrderByDescending(x => x.Value))
            {
                spPerDiscard.Children.Add(new TextBlock
                {
                    Text = $"{discardprobability.Key} with a prob. of {discardprobability.Value} %"
                });
            }

            GroupBox gbPerDiscard = new GroupBox { Header = "Win prob. per discard", Content = spPerDiscard };
            gbPerDiscard.SetValue(DockPanel.DockProperty, Dock.Bottom);

            DockPanel fullResults = new DockPanel();
            fullResults.Children.Add(gbPerDiscard);
            fullResults.Children.Add(spSolutions);
            Tbi1.Content = fullResults;

            #endregion 1 tile

            GrbResults.Visibility = Visibility.Visible;
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

        private void SetContentFromHandListYaku(ContentControl container, HandYakuListPivot handYakus, double probability = 1, List<Tuple<TilePivot, TilePivot>> substitutions = null)
        {
            if (handYakus == null)
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
                    Text = handYakus.FansName,
                    FontSize = 12,
                    Foreground = System.Windows.Media.Brushes.Red
                });
                if (probability < 1)
                {
                    sp.Children.Add(new TextBlock
                    {
                        Text = $"Prob. of {Math.Round(probability * 100, 3)} %",
                        FontSize = 12,
                        Foreground = System.Windows.Media.Brushes.DarkSlateBlue
                    });
                }

                StackPanel spYakus = new StackPanel { Orientation = Orientation.Vertical };
                foreach (YakuPivot yaku in handYakus.Yakus)
                {
                    spYakus.Children.Add(new TextBlock
                    {
                        Text = $"{yaku.Name} ({yaku.FansConcealed})",
                        ToolTip = yaku.Description
                    });
                }

                GroupBox gbYakus = new GroupBox { Header = "Yakus", Content = spYakus };
                sp.Children.Add(gbYakus);

                if (substitutions != null)
                {
                    StackPanel spSubs = new StackPanel { Orientation = Orientation.Vertical };
                    foreach (var sub in substitutions)
                    {
                        spSubs.Children.Add(new TextBlock
                        {
                            Text = $"Discard {sub.Item1}, pick {sub.Item2}"
                        });
                    }

                    GroupBox gbSubs = new GroupBox { Header = "Potential discards & picks", Content = spSubs };
                    sp.Children.Add(gbSubs);
                }

                container.Content = sp;
            }
        }
    }
}
