using System;
using Amplitude.Mercury.Interop.AI.Data;

namespace DevTools.Humankind.GUITools.UI
{
    public interface ITileEx
    {
        int TileIndex { get; set; }
    }
    
    [Flags]
    public enum HexTileType
    {
        None = 0,
        Settlement = 1 << 0,
        CityCenter = 1 << 1,
        District = 1 << 2,
        AdministrativeCenter = 1 << 3,
        Army = 1 << 4,
        MinorEmpire = 1 << 5,
        MajorEmpire = 1 << 6,
        Mountain = 1 << 7,
        Resource = 1 << 8,
        Strategic = 1 << 9,
        Luxury = 1 << 10,
        Manufactory = 1 << 11,
    }
}
