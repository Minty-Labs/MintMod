using System;
using UnityEngine;
using System.Globalization;

namespace MintMod.Libraries {
    class ColorConversion {
        public static Color HexToColor(string hexColor, bool hasTransparency = false, float transparcenyLevel = 1) {
            if (hexColor.IndexOf('#') != -1)
                hexColor = hexColor.Replace("#", "");
            double num1 = int.Parse(hexColor.Substring(0, 2), NumberStyles.AllowHexSpecifier) / (double)byte.MaxValue;
            float num2 = int.Parse(hexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier) / (float)byte.MaxValue;
            float num3 = int.Parse(hexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier) / (float)byte.MaxValue;
            double num4 = num2;
            double num5 = num3;
            if (hasTransparency)
                return new Color((float)num1, (float)num4, (float)num5, (float)transparcenyLevel);
            else
                return new Color((float)num1, (float)num4, (float)num5);
        }

        public static string ColorToHex(Color baseColor, bool hash = false) {
            string str = Convert.ToInt32(baseColor.r * byte.MaxValue).ToString("X2") + Convert.ToInt32(baseColor.g * byte.MaxValue).ToString("X2") + Convert.ToInt32(baseColor.b * byte.MaxValue).ToString("X2");
            if (hash)
                str = "#" + str;
            return str;
        }
    }
}
