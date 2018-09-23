using System;

namespace MahjongDll.Pivot
{
    /// <summary>
    /// Static helper methods to compute scores.
    /// </summary>
    public static class PointRulePivot
    {
        /// <summary>
        /// Gets points won by east, given by each other player, in case of tsumo.
        /// </summary>
        /// <param name="fans">Number of fans.</param>
        /// <param name="minipoints">Number of minipoints.</param>
        /// <returns>Points won by east, given by each other player.</returns>
        public static int GetPointsEastTsumo(int fans, int minipoints)
        {
            if (fans > 4)
            {
                return LimitEast(fans);
            }

            switch (fans)
            {
                case 1:
                    switch (minipoints)
                    {
                        case 30:
                            return 500;
                        case 40:
                            return 700;
                        case 50:
                            return 800;
                        case 60:
                            return 1000;
                        case 70:
                            return 1200;
                    }
                    break;
                case 2:
                    switch (minipoints)
                    {
                        case 20:
                            return 700;
                        case 30:
                            return 100;
                        case 40:
                            return 1300;
                        case 50:
                            return 1600;
                        case 60:
                            return 2000;
                        case 70:
                            return 2300;
                    }
                    break;
                case 3:
                    switch (minipoints)
                    {
                        case 20:
                            return 1300;
                        case 25:
                            return 1600;
                        case 30:
                            return 2000;
                        case 40:
                            return 2600;
                        case 50:
                            return 3200;
                        case 60:
                            return Rules.Default.Round430Mangan ? 4000 : 3900;
                        case 70:
                            return 4000;
                    }
                    break;
                case 4:
                    switch (minipoints)
                    {
                        case 20:
                            return 2600;
                        case 25:
                            return 3200;
                        case 30:
                            return Rules.Default.Round430Mangan ? 4000 : 3900;
                        case 40:
                        case 50:
                        case 60:
                        case 70:
                            return 4000;
                    }
                    break;
            }

            return 0;
        }

        /// <summary>
        /// Gets points won by east in case of ron.
        /// </summary>
        /// <param name="fans">Number of fans.</param>
        /// <param name="minipoints">Number of minipoints.</param>
        /// <returns>Points won by east.</returns>
        public static int GetPointsEastRon(int fans, int minipoints)
        {
            if (fans > 4)
            {
                return LimitEast(fans);
            }

            switch (fans)
            {
                case 1:
                    switch (minipoints)
                    {
                        case 30:
                            return 1500;
                        case 40:
                            return 2000;
                        case 50:
                            return 2400;
                        case 60:
                            return 2900;
                        case 70:
                            return 3400;
                    }
                    break;
                case 2:
                    switch (minipoints)
                    {
                        case 25:
                            return 2400;
                        case 30:
                            return 2900;
                        case 40:
                            return 3900;
                        case 50:
                            return 4800;
                        case 60:
                            return 5800;
                        case 70:
                            return 6800;
                    }
                    break;
                case 3:
                    switch (minipoints)
                    {
                        case 25:
                            return 4800;
                        case 30:
                            return 5800;
                        case 40:
                            return 7700;
                        case 50:
                            return 9600;
                        case 60:
                            return Rules.Default.Round430Mangan ? 12000 : 11600;
                        case 70:
                            return 12000;
                    }
                    break;
                case 4:
                    switch (minipoints)
                    {
                        case 25:
                            return 9600;
                        case 30:
                            return Rules.Default.Round430Mangan ? 12000 : 11600;
                        case 40:
                        case 50:
                        case 60:
                        case 70:
                            return 12000;
                    }
                    break;
            }

            return 0;
        }

        /// <summary>
        /// Gets points won by not-east in case of tsumo.
        /// </summary>
        /// <param name="fans">Number of fans.</param>
        /// <param name="minipoints">Number of minipoints.</param>
        /// <returns>Points won by not-east. First is points lost by east, second is points lost by each other non-east.</returns>
        public static Tuple<int, int> GetPointsOtherTsumo(int fans, int minipoints)
        {
            if (fans > 4)
            {
                return LimitOther(fans);
            }

            switch (fans)
            {
                case 1:
                    switch (minipoints)
                    {
                        case 30:
                            return new Tuple<int, int>(500, 300);
                        case 40:
                            return new Tuple<int, int>(700, 400);
                        case 50:
                            return new Tuple<int, int>(800, 400);
                        case 60:
                            return new Tuple<int, int>(1000, 500);
                        case 70:
                            return new Tuple<int, int>(1200, 600);
                    }
                    break;
                case 2:
                    switch (minipoints)
                    {
                        case 20:
                            return new Tuple<int, int>(700, 400);
                        case 30:
                            return new Tuple<int, int>(1000, 500);
                        case 40:
                            return new Tuple<int, int>(1300, 700);
                        case 50:
                            return new Tuple<int, int>(1600, 800);
                        case 60:
                            return new Tuple<int, int>(2000, 1000);
                        case 70:
                            return new Tuple<int, int>(2300, 1200);
                    }
                    break;
                case 3:
                    switch (minipoints)
                    {
                        case 20:
                            return new Tuple<int, int>(1300, 700);
                        case 25:
                            return new Tuple<int, int>(1600, 800);
                        case 30:
                            return new Tuple<int, int>(2000, 1000);
                        case 40:
                            return new Tuple<int, int>(2600, 1300);
                        case 50:
                            return new Tuple<int, int>(3200, 1600);
                        case 60:
                            return Rules.Default.Round430Mangan ?
                                new Tuple<int, int>(4000, 2000) : new Tuple<int, int>(3900, 2000);
                        case 70:
                            return new Tuple<int, int>(4000, 2000);
                    }
                    break;
                case 4:
                    switch (minipoints)
                    {
                        case 20:
                            return new Tuple<int, int>(2600, 1300);
                        case 25:
                            return new Tuple<int, int>(3200, 1600);
                        case 30:
                            return Rules.Default.Round430Mangan ?
                                new Tuple<int, int>(4000, 2000) : new Tuple<int, int>(3900, 2000);
                        case 40:
                        case 50:
                        case 60:
                        case 70:
                            return new Tuple<int, int>(4000, 2000);
                    }
                    break;
            }

            return new Tuple<int, int>(0, 0);
        }

        /// <summary>
        /// Gets points won by not-east in case of ron.
        /// </summary>
        /// <param name="fans">Number of fans.</param>
        /// <param name="minipoints">Number of minipoints.</param>
        /// <returns>Points won by not-east.</returns>
        public static int GetPointsOtherRon(int fans, int minipoints)
        {
            if (fans > 4)
            {
                Tuple<int, int> limit = LimitOther(fans);
                return limit.Item1 + limit.Item2 + limit.Item2;
            }

            switch (fans)
            {
                case 1:
                    switch (minipoints)
                    {
                        case 30:
                            return 1000;
                        case 40:
                            return 1300;
                        case 50:
                            return 1600;
                        case 60:
                            return 2000;
                        case 70:
                            return 2300;
                    }
                    break;
                case 2:
                    switch (minipoints)
                    {
                        case 25:
                            return 1600;
                        case 30:
                            return 2000;
                        case 40:
                            return 2600;
                        case 50:
                            return 3200;
                        case 60:
                            return 3900;
                        case 70:
                            return 4500;
                    }
                    break;
                case 3:
                    switch (minipoints)
                    {
                        case 25:
                            return 3200;
                        case 30:
                            return 3900;
                        case 40:
                            return 5200;
                        case 50:
                            return 6400;
                        case 60:
                            return Rules.Default.Round430Mangan ? 8000 : 7700;
                        case 70:
                            return 8000;
                    }
                    break;
                case 4:
                    switch (minipoints)
                    {
                        case 25:
                            return 6400;
                        case 30:
                            return Rules.Default.Round430Mangan ? 8000 : 7700;
                        case 40:
                        case 50:
                        case 60:
                        case 70:
                            return 8000;
                    }
                    break;
            }

            return 0;
        }

        // Points for east, when beyond 4 fans.
        private static int LimitEast(int fans)
        {
            switch (fans)
            {
                case 5:
                    return 12000;
                case 6:
                case 7:
                    return 18000;
                case 8:
                case 9:
                case 10:
                    return 24000;
                case 11:
                case 12:
                    return 36000;
                case 13:
                    return 48000;
            }

            return 0;
        }

        // Points for non-east, when beyond 4 fans.
        private static Tuple<int, int> LimitOther(int fans)
        {
            switch (fans)
            {
                case 5:
                    return new Tuple<int, int>(4000, 2000);
                case 6:
                case 7:
                    return new Tuple<int, int>(6000, 3000);
                case 8:
                case 9:
                case 10:
                    return new Tuple<int, int>(8000, 4000);
                case 11:
                case 12:
                    return new Tuple<int, int>(12000, 6000);
                case 13:
                    return new Tuple<int, int>(16000, 8000);
            }

            return new Tuple<int, int>(0, 0);
        }
    }
}
