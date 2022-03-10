using System.Globalization;
using UnityEngine;

namespace StyledGUI
{
    public static class Colors
    {
        public static Colorable[] Values =
        {
            new Colorable("#F0F8FF"),
            new Colorable("#FAEBD7"),
            new Colorable("#00FFFF"),
            new Colorable("#7FFFD4"),
            new Colorable("#F0FFFF"),
            new Colorable("#F5F5DC"),
            new Colorable("#FFE4C4"),
            new Colorable("#000000"),
            new Colorable("#FFEBCD"),
            new Colorable("#0000FF"),
            new Colorable("#8A2BE2"),
            new Colorable("#A52A2A"),
            new Colorable("#DEB887"),
            new Colorable("#5F9EA0"),
            new Colorable("#7FFF00"),
            new Colorable("#D2691E"),
            new Colorable("#FF7F50"),
            new Colorable("#6495ED"),
            new Colorable("#FFF8DC"),
            new Colorable("#DC143C"),
            new Colorable("#00FFFF"),
            new Colorable("#00008B"),
            new Colorable("#008B8B"),
            new Colorable("#B8860B"),
            new Colorable("#A9A9A9"),
            new Colorable("#A9A9A9"),
            new Colorable("#006400"),
            new Colorable("#BDB76B"),
            new Colorable("#8B008B"),
            new Colorable("#556B2F"),
            new Colorable("#FF8C00"),
            new Colorable("#9932CC"),
            new Colorable("#8B0000"),
            new Colorable("#E9967A"),
            new Colorable("#8FBC8F"),
            new Colorable("#483D8B"),
            new Colorable("#2F4F4F"),
            new Colorable("#2F4F4F"),
            new Colorable("#00CED1"),
            new Colorable("#9400D3"),
            new Colorable("#FF1493"),
            new Colorable("#00BFFF"),
            new Colorable("#696969"),
            new Colorable("#696969"),
            new Colorable("#1E90FF"),
            new Colorable("#B22222"),
            new Colorable("#FFFAF0"),
            new Colorable("#228B22"),
            new Colorable("#FF00FF"),
            new Colorable("#DCDCDC"),
            new Colorable("#F8F8FF"),
            new Colorable("#FFD700"),
            new Colorable("#DAA520"),
            new Colorable("#808080"),
            new Colorable("#808080"),
            new Colorable("#008000"),
            new Colorable("#ADFF2F"),
            new Colorable("#F0FFF0"),
            new Colorable("#FF69B4"),
            new Colorable("#CD5C5C"),
            new Colorable("#4B0082"),
            new Colorable("#FFFFF0"),
            new Colorable("#F0E68C"),
            new Colorable("#E6E6FA"),
            new Colorable("#FFF0F5"),
            new Colorable("#7CFC00"),
            new Colorable("#FFFACD"),
            new Colorable("#ADD8E6"),
            new Colorable("#F08080"),
            new Colorable("#E0FFFF"),
            new Colorable("#FAFAD2"),
            new Colorable("#D3D3D3"),
            new Colorable("#D3D3D3"),
            new Colorable("#90EE90"),
            new Colorable("#FFB6C1"),
            new Colorable("#FFA07A"),
            new Colorable("#20B2AA"),
            new Colorable("#87CEFA"),
            new Colorable("#778899"),
            new Colorable("#778899"),
            new Colorable("#B0C4DE"),
            new Colorable("#FFFFE0"),
            new Colorable("#00FF00"),
            new Colorable("#32CD32"),
            new Colorable("#FAF0E6"),
            new Colorable("#FF00FF"),
            new Colorable("#800000"),
            new Colorable("#66CDAA"),
            new Colorable("#0000CD"),
            new Colorable("#BA55D3"),
            new Colorable("#9370DB"),
            new Colorable("#3CB371"),
            new Colorable("#7B68EE"),
            new Colorable("#00FA9A"),
            new Colorable("#48D1CC"),
            new Colorable("#C71585"),
            new Colorable("#191970"),
            new Colorable("#F5FFFA"),
            new Colorable("#FFE4E1"),
            new Colorable("#FFE4B5"),
            new Colorable("#FFDEAD"),
            new Colorable("#000080"),
            new Colorable("#FDF5E6"),
            new Colorable("#808000"),
            new Colorable("#6B8E23"),
            new Colorable("#FFA500"),
            new Colorable("#FF4500"),
            new Colorable("#DA70D6"),
            new Colorable("#EEE8AA"),
            new Colorable("#98FB98"),
            new Colorable("#AFEEEE"),
            new Colorable("#DB7093"),
            new Colorable("#FFEFD5"),
            new Colorable("#FFDAB9"),
            new Colorable("#CD853F"),
            new Colorable("#FFC0CB"),
            new Colorable("#DDA0DD"),
            new Colorable("#B0E0E6"),
            new Colorable("#800080"),
            new Colorable("#663399"),
            new Colorable("#FF0000"),
            new Colorable("#BC8F8F"),
            new Colorable("#4169E1"),
            new Colorable("#8B4513"),
            new Colorable("#FA8072"),
            new Colorable("#F4A460"),
            new Colorable("#2E8B57"),
            new Colorable("#FFF5EE"),
            new Colorable("#A0522D"),
            new Colorable("#C0C0C0"),
            new Colorable("#87CEEB"),
            new Colorable("#6A5ACD"),
            new Colorable("#708090"),
            new Colorable("#708090"),
            new Colorable("#FFFAFA"),
            new Colorable("#00FF7F"),
            new Colorable("#4682B4"),
            new Colorable("#D2B48C"),
            new Colorable("#008080"),
            new Colorable("#D8BFD8"),
            new Colorable("#FF6347"),
            new Colorable("#40E0D0"),
            new Colorable("#EE82EE"),
            new Colorable("#F5DEB3"),
            new Colorable("#FFFFFF"),
            new Colorable("#F5F5F5"),
            new Colorable("#FFFF00"),
            new Colorable("#9ACD32")
        };

        public static Colorable AliceBlue => Values[0];
        public static Colorable AntiqueWhite => Values[1];
        public static Colorable Aqua => Values[2];
        public static Colorable Aquamarine => Values[3];
        public static Colorable Azure => Values[4];
        public static Colorable Beige => Values[5];
        public static Colorable Bisque => Values[6];
        public static Colorable Black => Values[7];
        public static Colorable BlanchedAlmond => Values[8];
        public static Colorable Blue => Values[9];
        public static Colorable BlueViolet => Values[10];
        public static Colorable Brown => Values[11];
        public static Colorable BurlyWood => Values[12];
        public static Colorable CadetBlue => Values[13];
        public static Colorable Chartreuse => Values[14];
        public static Colorable Chocolate => Values[15];
        public static Colorable Coral => Values[16];
        public static Colorable CornflowerBlue => Values[17];
        public static Colorable Cornsilk => Values[18];
        public static Colorable Crimson => Values[19];
        public static Colorable Cyan => Values[20];
        public static Colorable DarkBlue => Values[21];
        public static Colorable DarkCyan => Values[22];
        public static Colorable DarkGoldenRod => Values[23];
        public static Colorable DarkGray => Values[24];
        public static Colorable DarkGrey => Values[25];
        public static Colorable DarkGreen => Values[26];
        public static Colorable DarkKhaki => Values[27];
        public static Colorable DarkMagenta => Values[28];
        public static Colorable DarkOliveGreen => Values[29];
        public static Colorable DarkOrange => Values[30];
        public static Colorable DarkOrchid => Values[31];
        public static Colorable DarkRed => Values[32];
        public static Colorable DarkSalmon => Values[33];
        public static Colorable DarkSeaGreen => Values[34];
        public static Colorable DarkSlateBlue => Values[35];
        public static Colorable DarkSlateGray => Values[36];
        public static Colorable DarkSlateGrey => Values[37];
        public static Colorable DarkTurquoise => Values[38];
        public static Colorable DarkViolet => Values[39];
        public static Colorable DeepPink => Values[40];
        public static Colorable DeepSkyBlue => Values[41];
        public static Colorable DimGray => Values[42];
        public static Colorable DimGrey => Values[43];
        public static Colorable DodgerBlue => Values[44];
        public static Colorable FireBrick => Values[45];
        public static Colorable FloralWhite => Values[46];
        public static Colorable ForestGreen => Values[47];
        public static Colorable Fuchsia => Values[48];
        public static Colorable Gainsboro => Values[49];
        public static Colorable GhostWhite => Values[50];
        public static Colorable Gold => Values[51];
        public static Colorable GoldenRod => Values[52];
        public static Colorable Gray => Values[53];
        public static Colorable Grey => Values[54];
        public static Colorable Green => Values[55];
        public static Colorable GreenYellow => Values[56];
        public static Colorable HoneyDew => Values[57];
        public static Colorable HotPink => Values[58];
        public static Colorable IndianRed => Values[59];
        public static Colorable Indigo => Values[60];
        public static Colorable Ivory => Values[61];
        public static Colorable Khaki => Values[62];
        public static Colorable Lavender => Values[63];
        public static Colorable LavenderBlush => Values[64];
        public static Colorable LawnGreen => Values[65];
        public static Colorable LemonChiffon => Values[66];
        public static Colorable LightBlue => Values[67];
        public static Colorable LightCoral => Values[68];
        public static Colorable LightCyan => Values[69];
        public static Colorable LightGoldenRodYellow => Values[70];
        public static Colorable LightGray => Values[71];
        public static Colorable LightGrey => Values[72];
        public static Colorable LightGreen => Values[73];
        public static Colorable LightPink => Values[74];
        public static Colorable LightSalmon => Values[75];
        public static Colorable LightSeaGreen => Values[76];
        public static Colorable LightSkyBlue => Values[77];
        public static Colorable LightSlateGray => Values[78];
        public static Colorable LightSlateGrey => Values[79];
        public static Colorable LightSteelBlue => Values[80];
        public static Colorable LightYellow => Values[81];
        public static Colorable Lime => Values[82];
        public static Colorable LimeGreen => Values[83];
        public static Colorable Linen => Values[84];
        public static Colorable Magenta => Values[85];
        public static Colorable Maroon => Values[86];
        public static Colorable MediumAquaMarine => Values[87];
        public static Colorable MediumBlue => Values[88];
        public static Colorable MediumOrchid => Values[89];
        public static Colorable MediumPurple => Values[90];
        public static Colorable MediumSeaGreen => Values[91];
        public static Colorable MediumSlateBlue => Values[92];
        public static Colorable MediumSpringGreen => Values[93];
        public static Colorable MediumTurquoise => Values[94];
        public static Colorable MediumVioletRed => Values[95];
        public static Colorable MidnightBlue => Values[96];
        public static Colorable MintCream => Values[97];
        public static Colorable MistyRose => Values[98];
        public static Colorable Moccasin => Values[99];
        public static Colorable NavajoWhite => Values[100];
        public static Colorable Navy => Values[101];
        public static Colorable OldLace => Values[102];
        public static Colorable Olive => Values[103];
        public static Colorable OliveDrab => Values[104];
        public static Colorable Orange => Values[105];
        public static Colorable OrangeRed => Values[106];
        public static Colorable Orchid => Values[107];
        public static Colorable PaleGoldenRod => Values[108];
        public static Colorable PaleGreen => Values[109];
        public static Colorable PaleTurquoise => Values[110];
        public static Colorable PaleVioletRed => Values[111];
        public static Colorable PapayaWhip => Values[112];
        public static Colorable PeachPuff => Values[113];
        public static Colorable Peru => Values[114];
        public static Colorable Pink => Values[115];
        public static Colorable Plum => Values[116];
        public static Colorable PowderBlue => Values[117];
        public static Colorable Purple => Values[118];
        public static Colorable RebeccaPurple => Values[119];
        public static Colorable Red => Values[120];
        public static Colorable RosyBrown => Values[121];
        public static Colorable RoyalBlue => Values[122];
        public static Colorable SaddleBrown => Values[123];
        public static Colorable Salmon => Values[124];
        public static Colorable SandyBrown => Values[125];
        public static Colorable SeaGreen => Values[126];
        public static Colorable SeaShell => Values[127];
        public static Colorable Sienna => Values[128];
        public static Colorable Silver => Values[129];
        public static Colorable SkyBlue => Values[130];
        public static Colorable SlateBlue => Values[131];
        public static Colorable SlateGray => Values[132];
        public static Colorable SlateGrey => Values[133];
        public static Colorable Snow => Values[134];
        public static Colorable SpringGreen => Values[135];
        public static Colorable SteelBlue => Values[136];
        public static Colorable Tan => Values[137];
        public static Colorable Teal => Values[138];
        public static Colorable Thistle => Values[139];
        public static Colorable Tomato => Values[140];
        public static Colorable Turquoise => Values[141];
        public static Colorable Violet => Values[142];
        public static Colorable Wheat => Values[143];
        public static Colorable White => Values[144];
        public static Colorable WhiteSmoke => Values[145];
        public static Colorable Yellow => Values[146];
        public static Colorable YellowGreen => Values[147];

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

        public Colorable gamma => new Colorable(rgb.gamma);
        public Colorable grayscale => new Colorable(new Color(rgb.grayscale, rgb.grayscale, rgb.grayscale));
        public Colorable linear => new Colorable(rgb.linear);
        
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
