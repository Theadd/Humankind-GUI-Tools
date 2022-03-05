using UnityEngine;

namespace DevTools.Humankind.GUITools.Collections
{
    public interface IMetaContainer : System.IEquatable<IMetaContainer>
    {
        StringHandle Value { get; set; }
    }
    
    public class MetaContainer : IMetaContainer
    {
        public StringHandle Value { get; set; }

        public MetaContainer(StringHandle value)
        {
            Value = value;
        }

        public MetaContainer() { }

        public static implicit operator StringHandle(MetaContainer c) => c.Value;

        public static explicit operator MetaContainer(StringHandle s) =>
            new MetaContainer() { Value = s };
        
        public static bool operator ==(MetaContainer x, MetaContainer y) =>
            (x?.Value.Handle ?? 0) == (y?.Value.Handle ?? 0);

        public static bool operator !=(MetaContainer x, MetaContainer y) =>
            (x?.Value.Handle ?? 0) != (y?.Value.Handle ?? 0);

        public bool Equals(IMetaContainer other) =>
            this.Value.Handle == (other?.Value.Handle ?? 0);
        
        public override bool Equals(object x)
        {
            if (x == null)
                return this.Value.Handle == 0;
            
            if (x is IMetaContainer metaContainer)
                return metaContainer.Value.Handle == this.Value.Handle;
            
            if (x is StringHandle stringHandle)
                return stringHandle.Handle == this.Value.Handle;

            return false;
        }
        
        public override int GetHashCode() => this.Value.Handle;
    }
}
