using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Douban.Converter
{
    internal class ColorExtraReversalConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            try
            {
                Color OriginalColor = (Color)value;
                Color Color1 = Color.FromRgb((byte)(255 - OriginalColor.R), (byte)(255 - OriginalColor.G), (byte)(255 - OriginalColor.B));
                byte R = (byte)ScaleInte(Color1.R);
                byte G = (byte)ScaleInte(Color1.G);
                byte B = (byte)ScaleInte(Color1.B);

                byte sum = (byte)((R + B + G) / 3);

                R = sum;
                G = sum;
                B = sum;

                return Color.FromRgb(R, G, B);
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
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

        private int ScaleInte(int R)
        {
            if (R < 127)
            {
                R -= 63;
                if (R < 0)
                    R = 0;
                else
                    R *= 2;
            }
            else
            {
                R += 63;
                if (R > 255)
                    R = 255;
                else
                {
                    R -= 190;
                    R *= 2;
                    R += 125;
                }
            }
            return R;
        }
    }
}
