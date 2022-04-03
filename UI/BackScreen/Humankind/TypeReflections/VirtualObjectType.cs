using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DevTools.Humankind.GUITools.TypeReflections
{
    public class VirtualObjectType
    {
        private IEnumerable<VirtualMember> _members;
        public Type Type { get; set; }
        public object Value { get; set; }

        public IEnumerable<VirtualMember> Members => _members ?? (_members = this.GetVirtualMembers());

        public static VirtualObjectType Create(Type type, object obj = null)
        {
            var instance = new VirtualObjectType()
            {
                Type = type,
                Value = obj
            };

            return instance;
        }
    }

    public static class VirtualObjectTypeEx
    {
        public static IEnumerable<VirtualMember> GetVirtualMembers(this VirtualObjectType self)
        {
            throw new NotImplementedException();
//            IEnumerable<Type> baseTypes = new List<Type>(); // TODO: TypeExtensions.GetAllBaseTypes(self.Type);
//
//            var members = baseTypes.SelectMany(t =>
//                t.GetExtensionMethods().Select(m =>
//                    new VirtualMember()
//                    {
//                        Value = m,
//                        RelatedType = t
//                    }));
//
//            return members;
        }
    }
}
