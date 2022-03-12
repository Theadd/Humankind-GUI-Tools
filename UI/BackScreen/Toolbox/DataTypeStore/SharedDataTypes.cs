using Amplitude;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public class DefinitionsGroup
    {
        public string Title { get; set; }
        public DataTypeDefinition[] Values { get; set; }
    }

    public class DataTypeDefinition
    {
        public StaticString DefinitionName { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public int Era { get; set; }
        public Texture Image { get; set; }
    }

    public class DataTypeStoreBuildOptions
    {
        public bool ExcludeKnownInvalid { get; set; } = true;
        public bool ExcludeExtractorsAndManufactories { get; set; } = true;
        public bool ExcludeOthersGroup { get; set; } = false;
        public bool ExcludeObsolete { get; set; } = true;
    }
}
