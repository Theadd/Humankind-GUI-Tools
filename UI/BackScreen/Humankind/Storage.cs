using System.Collections.Generic;
using System.Linq;
using DevTools.Humankind.GUITools.Collections;
using StyledGUI;

namespace DevTools.Humankind.GUITools.UI.Humankind
{
    public class Storage
    {
        private static IEnumerable<IStoredType> _values = null;
        
        // HARDCODED VALUES WHILE NOT IN DISK
        public static IEnumerable<IStoredType> Values { get; set; } = _values ?? (_values = new IStoredType[]
        {
            new TextContent()
            {
                Name = TabbedToolbox.ZoomIn, Title = "",
                Description = $"<size=18><b><color={Colors.Aquamarine}>Zoom In </color></b></size>\n\nIncrease the size of displayed images in all tabs with similar grid lists."
            },
            new TextContent()
            {
                Name = TabbedToolbox.ZoomOut, Title = "<size=12> </size>",
                Description = $"<size=18><b><color={Colors.Aquamarine}>Zoom Out <size=12></size></color></b></size>\n\nReduce the size of displayed images in all tabs with similar grid lists."
            },
            new TextContent()
            {
                Name = TabbedToolbox.RebuildDataTypes, Title = " <size=12> </size>",
                Description = $"<size=18><b><color={Colors.Aquamarine}>Rebuild displayed definitions <size=12> </size></color></b></size>\n\nReloads content from database definitions."
            },
            new TextContent()
            {
                Name = TabbedToolbox.ViewModeList, Title = "",
                Description = $"<size=18><b><color={Colors.Aquamarine}>List Display Mode </color></b></size>\n\nDisplay content in a List View."
            },
            new TextContent()
            {
                Name = TabbedToolbox.ViewModeGrid, Title = "<size=16></size>",
                Description = $"<size=18><b><color={Colors.Aquamarine}>Grid Display Mode <size=16></size></color></b></size>\n\nDisplay content in a Grid View."
            },
            new TextContent()
            {
                Name = BackScreenWindow.GodModeHandle, Title = "GOD MODE",
                Description = $@"<size=18><b><color={Colors.Gold}>God Mode Cursor</color></b></size>

When active, all buttons/icons that were not yet available in your game will be 
visible and your cursor will turn pink, then, simply by clicking on them, you can:

    - Unlock <b>Civics</b>
    - Complete <b>Technologies</b>
    - Unlock <b>Era Stars</b>
    - Restore <b>Army Movement Points</b>
    - Increase <b>Influence</b>, <b>Money</b>, <b>Luxury</b> and <b>Strategic Resources</b>, <b>City Cap</b>, ...
    - In the <b>City Screen</b>:
          - Increase <b>Population</b> and <b>Stability</b>
          - Create <b>Units</b>
          - Build <b>Districts</b> and <b>Infrastructures</b>
          - Complete the construction queue
    - And probably more

All this, instantly and with no buyout cost at all."
            },
        });

        public static T Get<T>(StringHandle name) where T : IStoredType => (T) Values.FirstOrDefault(k => k is T e && e.Name == name);
    }
}
