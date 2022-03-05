using System.Collections.Generic;
using System.Linq;
using Amplitude.Framework;
using Amplitude.UI;
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
                Description = $"<size=22><b><color={Colors.LimeGreen}>Zoom In </color></b></size>\n\nIncrease the size of displayed images in all tabs with similar grid lists."
            },
            new TextContent()
            {
                Name = DataTypeDefinitionsToolbox.ZoomOut, Title = "<size=12> </size>",
                Description = $"<size=22><b><color={Colors.LimeGreen}>Zoom Out <size=12></size></color></b></size>\n\nReduce the size of displayed images in all tabs with similar grid lists."
            },
        });

        public static T Get<T>(StringHandle name) where T : IStoredType => (T) Values.FirstOrDefault(k => k is T e && e.Name == name);
    }
}
