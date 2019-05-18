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
            ExtractFromForm(out List<TilePivot> handTiles, out TilePivot latestTile, out WindPivot dominantWind, out WindPivot seatWind);

            /*
            HandYakuListPivot handYakus = null;
            int iteration = 0;
            List<Tuple<TilePivot, TilePivot>> substitutions = new List<Tuple<TilePivot, TilePivot>>();
            do
            {
                if (iteration == 0)
                {
                    FullHandPivot hand = new FullHandPivot(handTiles, dominantWind, seatWind, latestTile);
                    handYakus = hand.ComputeHandYakus()?.FirstOrDefault();
                }
                else if (iteration == 1)
                {
                    List<TilePivot> availableTiles = _draw.ComputeRemainingTiles(handTiles, true);
                    foreach (TilePivot subbedTile in handTiles.Distinct())
                    {
                        foreach (TilePivot subTile in availableTiles)
                        {
                            Tuple<TilePivot, TilePivot> substitution = new Tuple<TilePivot, TilePivot>(subbedTile, subTile);
                            List<TilePivot> handTilesWithSub = new List<TilePivot>(handTiles);
                            handTilesWithSub.Remove(substitution.Item1);
                            handTilesWithSub.Add(substitution.Item2);

                            FullHandPivot hand = new FullHandPivot(handTilesWithSub,
                                dominantWind,
                                seatWind,
                                false);

                            HandYakuListPivot tempBestYakusList = hand.ComputeHandYakus()?.FirstOrDefault();
                            if (tempBestYakusList != null && (handYakus == null || tempBestYakusList.OfficialFansCount > handYakus.OfficialFansCount))
                            {
                                handYakus = tempBestYakusList;
                                substitutions.Clear();
                                substitutions.Add(substitution);
                            }
                        }
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
                iteration++;
            }
            while (handYakus == null);*/
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
            GrbResults.Content = null;
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
    }
}
