using System;
using Amplitude.Mercury.Interop.AI.Data;

namespace DevTools.Humankind.GUITools.UI
{
    public interface ITileEx
    {
        int TileIndex { get; set; }
        Tile Tile { get; set; }
    }
    
    [Flags]
    public enum HexTileType
    {
        None = 0,
        Settlement = 1,
        CityCenter = 2,
        District = 4,
        AdministrativeCenter = 8,
        Army = 16,
        MinorEmpire = 32,
        MajorEmpire = 64,
    }
}
