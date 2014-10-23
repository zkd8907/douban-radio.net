using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Douban.Converter
{
    internal class ColorReversalConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            try
            {
                Color OriginalColor = ((SolidColorBrush)value).Color;
                return Color.FromArgb(OriginalColor.A, (byte)(255 - OriginalColor.R), (byte)(255 - OriginalColor.G), (byte)(255 - OriginalColor.B));
            }
            catch
            {
                return null;
            }
        }
        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            return null;
        }
    }
}
