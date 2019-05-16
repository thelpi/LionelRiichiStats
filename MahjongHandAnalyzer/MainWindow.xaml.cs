using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MahjongDll.Pivot;
using System;
using System.Linq;

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
            GrbYakus.Visibility = Visibility.Collapsed;

            _draw = new DrawPivot();
            for (int i = 1; i <= 14; i++)
            {
                GetCombo(i).ItemsSource = _draw.UniqueTiles;
            }
            CbbWindDominant.ItemsSource = Enum.GetValues(typeof(WindPivot));
            CbbWindDominant.SelectedIndex = 0;
            CbbWindTurn.ItemsSource = Enum.GetValues(typeof(WindPivot));
            CbbWindTurn.SelectedIndex = 0;
        }

        #region Actions

        private void BtnAnalyze_Click(object sender, RoutedEventArgs e)
        {
            List<TilePivot> handTiles = new List<TilePivot>();
            for (int i = 1; i <= 14; i++)
            {
                if (GetCombo(i).SelectedItem == null)
                {
                    MessageBox.Show("Some slots are empty.", "LionelRiichiStats - Error");
                    return;
                }
                handTiles.Add(GetCombo(i).SelectedItem as TilePivot);
            }

            HandYakuListPivot handYakus = null;
            int iteration = 0;
            List<Tuple<TilePivot, TilePivot>> substitutions = new List<Tuple<TilePivot, TilePivot>>();
            do
            {
                if (iteration == 0)
                {
                    FullHandPivot hand = new FullHandPivot(handTiles,
                        CbbWindDominant.SelectedIndex > -1 ? (WindPivot)CbbWindDominant.SelectedItem : WindPivot.East,
                        CbbWindTurn.SelectedIndex > -1 ? (WindPivot)CbbWindTurn.SelectedItem : WindPivot.East,
                        false);
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
                                CbbWindDominant.SelectedIndex > -1 ? (WindPivot)CbbWindDominant.SelectedItem : WindPivot.East,
                                CbbWindTurn.SelectedIndex > -1 ? (WindPivot)CbbWindTurn.SelectedItem : WindPivot.East,
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
                    /*for (int j = 0; j < iteration; j++)
                    {
                        List<YakuPivot> tempBestYakusList = null;

                        if (tempBestYakusList != null && tempBestYakusList.Sum(x => x.FansConcealed) > yakusList?.Sum(x => x.FansConcealed))
                        {
                            yakusList = tempBestYakusList;
                        }
                    }*/
                }
                iteration++;
            }
            while (handYakus == null);

            if (substitutions != null)
            {
                TextBlock subTb = new TextBlock
                {
                    Text = string.Format("Substitutions ({0}) : {1}",
                        substitutions.Count,
                        string.Join(", ", substitutions.Select(x => $"[{x.Item1.ToString()}] to [{x.Item2.ToString()}]"))),
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 12,
                    Foreground = System.Windows.Media.Brushes.Red
                };
                StpYakus.Children.Add(subTb);
            }

            foreach (YakuPivot yaku in handYakus.Yakus)
            {
                TextBlock tb = new TextBlock
                {
                    Text = $"{yaku.Name} ({yaku.FansConcealed} fans)",
                    VerticalAlignment = VerticalAlignment.Center,
                    ToolTip = yaku.Description
                };
                StpYakus.Children.Add(tb);
            }
            string fansLabel = string.Empty;
            if (handYakus.Yakuman)
            {
                fansLabel = $"Yakuman";
            }
            else
            {
                switch (handYakus.OfficialFansCount)
                {
                    case 12:
                    case 11:
                        fansLabel = $"Sanbaiman ({handYakus.OfficialFansCount} fans)";
                        break;
                    case 10:
                    case 9:
                    case 8:
                        fansLabel = $"Baiman ({handYakus.OfficialFansCount} fans)";
                        break;
                    case 7:
                    case 6:
                        fansLabel = $"Haneman ({handYakus.OfficialFansCount} fans)";
                        break;
                    case 5:
                        fansLabel = $"Mangan ({handYakus.OfficialFansCount} fans)";
                        break;
                    default:
                        fansLabel = $"Total : {handYakus.OfficialFansCount} fans";
                        break;
                }
            }

            TextBlock tbLast = new TextBlock
            {
                Text = fansLabel,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 12,
                Foreground = System.Windows.Media.Brushes.Red
            };
            StpYakus.Children.Add(tbLast);

            GrbYakus.Visibility = Visibility.Visible;
        }

        private void BtnRandomize_Click(object sender, RoutedEventArgs e)
        {
            List<TilePivot> toto = _draw.PickHandOfTiles().ToList();
            for (int i = 1; i <= 14; i++)
            {
                GetCombo(i).SelectedItem = toto[i - 1];
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i <= 14; i++)
            {
                GetCombo(i).SelectedIndex = -1;
            }
        }

        #endregion Actions

        private ComboBox GetCombo(int i)
        {
            return FindName(string.Format("CbbTile{0}", (i < 10 ? "0" : "") + i.ToString())) as ComboBox;
        }
    }
}
