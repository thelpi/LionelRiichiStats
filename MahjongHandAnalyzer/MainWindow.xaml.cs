﻿using System.Collections.Generic;
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

            List<YakuPivot> yakusList = null;
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
                    yakusList = hand.ComputeHandYakus()?.FirstOrDefault();
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

                            List<YakuPivot> tempBestYakusList = hand.ComputeHandYakus()?.FirstOrDefault();
                            if (tempBestYakusList != null && (yakusList == null || tempBestYakusList.Sum(x => x.FansConcealed) > yakusList?.Sum(x => x.FansConcealed)))
                            {
                                yakusList = tempBestYakusList;
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
            while (yakusList?.Any() != true);

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

            int fansCount = 0;
            int yakumanCount = 0;
            foreach (YakuPivot yaku in yakusList)
            {
                TextBlock tb = new TextBlock
                {
                    Text = $"{yaku.Name} ({yaku.FansConcealed} fans)",
                    VerticalAlignment = VerticalAlignment.Center,
                    ToolTip = yaku.Description
                };
                StpYakus.Children.Add(tb);
                fansCount += yaku.FansConcealed;
                if (yaku.Yakuman)
                {
                    yakumanCount++;
                }
            }
            string fansLabel = string.Empty;
            if (yakumanCount > 0 || fansCount >= 13)
            {
                if (yakumanCount > 0)
                {
                    fansLabel = $"Yakuman ({yakumanCount})";
                }
                else
                {
                    fansLabel = $"Kazoe yakuman ({fansCount} fans)";
                }
            }
            else
            {
                switch (fansCount)
                {
                    case 12:
                    case 11:
                        fansLabel = $"Sanbaiman ({fansCount} fans)";
                        break;
                    case 10:
                    case 9:
                    case 8:
                        fansLabel = $"Baiman ({fansCount} fans)";
                        break;
                    case 7:
                    case 6:
                        fansLabel = $"Haneman ({fansCount} fans)";
                        break;
                    case 5:
                        fansLabel = $"Mangan ({fansCount} fans)";
                        break;
                    default:
                        fansLabel = $"Total : {fansCount} fans";
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
