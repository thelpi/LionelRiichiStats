using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace MahjongHandAnalyzer
{
    /// <summary>
    /// <see cref="IValueConverter"/> to transform a <see cref="Bitmap"/> to a <see cref="BitmapSource"/>.
    /// </summary>
    public class BitmapToImageConverter : IValueConverter
    {
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        /// <summary>
        /// Transform a <see cref="Bitmap"/> to a <see cref="BitmapSource"/>.
        /// </summary>
        /// <param name="value">The <see cref="Bitmap"/>.</param>
        /// <param name="targetType">N/A.</param>
        /// <param name="parameter">N/A.</param>
        /// <param name="culture">N/A.</param>
        /// <returns>The <see cref="BitmapSource"/>.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var handle = (value as Bitmap).GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value">N/A.</param>
        /// <param name="targetType">N/A.</param>
        /// <param name="parameter">N/A.</param>
        /// <param name="culture">N/A.</param>
        /// <returns>N/A.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
