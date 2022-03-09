using System.Globalization;
using UnityEngine;

namespace StyledGUI
{
    public static class Colors
    {
        public static Colorable[] Values =
        {
            new Colorable("#a8a8a8"),
            new Colorable("#92c470"),
            new Colorable("#3a8d71"),
            new Colorable("#2df7b2"),
            new Colorable("#0fba3a"),
            new Colorable("#9b9b82"),
            new Colorable("#8d8dc6"),
            new Colorable("#c266ff"),
            new Colorable("#b55b02"),
            new Colorable("#ff8000"),
            new Colorable("#588075"),
            new Colorable("#55a38e"),
            new Colorable("#a6e9e9"),
            new Colorable("#D49C85"),
            new Colorable("#91C26E"),
            new Colorable("#4C9CD4"),
            new Colorable("#B5CCA6"),
            new Colorable("#FFFFFF")
        };

        public static Colorable Grey => Values[0];
        public static Colorable OliveDrab => Values[1];
        public static Colorable SeaGreen => Values[2];
        public static Colorable Aquamarine => Values[3];
        public static Colorable LimeGreen => Values[4];
        public static Colorable DarkGray => Values[5];
        public static Colorable MediumPurple => Values[6];
        public static Colorable MediumOrchid => Values[7];
        public static Colorable Chocolate => Values[8];
        public static Colorable DarkOrange => Values[9];
        public static Colorable CadetBlue => Values[10];
        public static Colorable DarkCyan => Values[11];
        public static Colorable LightBlue => Values[12];
        public static Colorable DarkSalmon => Values[13];
        public static Colorable DarkSeaGreen => Values[14];
        public static Colorable SteelBlue => Values[15];
        public static Colorable PaleGoldenRod => Values[16];
        public static Colorable White => Values[17];
    }
    
    public readonly struct Colorable
    {
        private readonly string hex;
        private readonly Color rgb;

        public Colorable(string color)
        {
            hex = color;
            rgb = color.ToColor();
        }
        
        public Colorable(Color color)
        {
            rgb = color;
            hex = "#" + color.ToHex();
        }
        
        // Color AlphaMultiplied(float multiplier) => new Color(this.r, this.g, this.b, this.a * multiplier);
        
        public static implicit operator string(Colorable c) => c.hex;
        public static explicit operator Colorable(string s) => new Colorable(s);
        public static implicit operator Color(Colorable c) => c.rgb;
        public static explicit operator Colorable(Color c) => new Colorable(c);

        public override string ToString() => hex;
    }

    public static class ColorsEx
    {
        /// <summary>
        /// Converts Color to 6-digit RGB hex code (without # symbol). Eg, RGBA(1,0,0,1) -> FF0000
        /// </summary>
        public static string ToHex(this Color color)
        {
            byte r = (byte)Mathf.Clamp(Mathf.RoundToInt(color.r * 255f), 0, 255);
            byte g = (byte)Mathf.Clamp(Mathf.RoundToInt(color.g * 255f), 0, 255);
            byte b = (byte)Mathf.Clamp(Mathf.RoundToInt(color.b * 255f), 0, 255);

            return $"{r:X2}{g:X2}{b:X2}";
        }

        /// <summary>
        /// Assumes the string is a 6-digit RGB Hex color code (with optional leading #) which it will parse into a UnityEngine.Color.
        /// Eg, FF0000 -> RGBA(1,0,0,1)
        /// </summary>
        public static Color ToColor(this string _string)
        {
            _string = _string.Replace("#", "");

            if (_string.Length != 6)
                return Color.magenta;

            var r = byte.Parse(_string.Substring(0, 2), NumberStyles.HexNumber);
            var g = byte.Parse(_string.Substring(2, 2), NumberStyles.HexNumber);
            var b = byte.Parse(_string.Substring(4, 2), NumberStyles.HexNumber);

            var color = new Color
            {
                r = (float)(r / (decimal)255),
                g = (float)(g / (decimal)255),
                b = (float)(b / (decimal)255),
                a = 1
            };

            return color;
        }
    }
}
