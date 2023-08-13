using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using Sys;
using syscon.stdio;

namespace sqlcli
{
    static partial class Config
    {
        public static Configuration cfg { get; set; }

        public static Brush GetBrush(this string colorString, Color defaultColor)
        {
            if (colorString != null)
            {
                ColorConverter converter = new ColorConverter();

                if (converter.CanConvertFrom(typeof(string)))
                {
                    try
                    {
                        Color color = (Color)converter.ConvertFrom(null, null, colorString);
                        return new SolidColorBrush(color);
                    }
                    catch (Exception)
                    {
                        Cerr.WriteLine($"color string: \"{colorString}\" not supported");
                    }
                }
            }

            return new SolidColorBrush(defaultColor);
        }

        public static Brush GetSolidBrush(string key, Color defaultColor)
        {
            if (cfg != null)
                return new SolidColorBrush(GetColor(key, defaultColor));

            return default;
        }

        private static Color GetColor(string key, Color defaultColor)
        {
            string colorString = cfg.GetValue<string>(key);

            if (colorString != null)
            {
                ColorConverter converter = new ColorConverter();

                if (converter.CanConvertFrom(typeof(string)))
                {
                    try
                    {
                        Color color = (Color)converter.ConvertFrom(null, null, colorString);
                        return color;
                    }
                    catch (Exception)
                    {
                        Cerr.WriteLine($"color setting {key} = {colorString} not supported");
                    }
                }
            }

            return defaultColor;
        }

    }
}

