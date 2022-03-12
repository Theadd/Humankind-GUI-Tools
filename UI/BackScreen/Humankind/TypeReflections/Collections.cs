using System;
using System.Reflection;

namespace DevTools.Humankind.GUITools.TypeReflections
{
    public interface IMember
    {
        MemberInfo Value { get; }
    }
    
    public class VirtualMember : IMember
    {
        public MemberInfo Value { get; set; }
        public Type RelatedType { get; set; }

        public override string ToString()
        {
            return (RelatedType?.Name ?? "?") + "." + Value?.Name;
        }
    }
}
