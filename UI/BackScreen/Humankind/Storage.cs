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
                Name = DataTypeDefinitionsToolbox.ZoomIn, Title = "",
                Description = $"<size=18><b><color={Colors.Aquamarine}>Zoom In </color></b></size>\n\nIncrease the size of displayed images in all tabs with similar grid lists."
            },
            new TextContent()
            {
                Name = DataTypeDefinitionsToolbox.ZoomOut, Title = "<size=12> </size>",
                Description = $"<size=18><b><color={Colors.Aquamarine}>Zoom Out <size=12></size></color></b></size>\n\nReduce the size of displayed images in all tabs with similar grid lists."
            },
            new TextContent()
            {
                Name = DataTypeDefinitionsToolbox.RebuildDataTypes, Title = " <size=12> </size>",
                Description = $"<size=18><b><color={Colors.Aquamarine}>Rebuild displayed definitions <size=12> </size></color></b></size>\n\nReloads content from database definitions."
            },
            new TextContent()
            {
                Name = DataTypeDefinitionsToolbox.ViewModeList, Title = "",
                Description = $"<size=18><b><color={Colors.Aquamarine}>List Display Mode </color></b></size>\n\nDisplay content in a List View."
            },
            new TextContent()
            {
                Name = DataTypeDefinitionsToolbox.ViewModeGrid, Title = "<size=16></size>",
                Description = $"<size=18><b><color={Colors.Aquamarine}>Grid Display Mode <size=16></size></color></b></size>\n\nDisplay content in a Grid View."
            },
        });

        public static T Get<T>(StringHandle name) where T : IStoredType => (T) Values.FirstOrDefault(k => k is T e && e.Name == name);
    }
}
