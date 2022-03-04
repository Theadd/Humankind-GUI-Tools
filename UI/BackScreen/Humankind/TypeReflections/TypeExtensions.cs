using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Modding.Humankind.DevTools;
using UniverseLib;

namespace DevTools.Humankind.GUITools.TypeReflections
{
    public static class TypeExtensions
    {
        

        private static readonly MethodInfo[] NoMethods = new MethodInfo[]{};
        private static readonly IEnumerable<Type> NoTypes = new Type[]{};

        public static IEnumerable<Type> GetAllBaseTypes(object obj) =>
            obj != null ? GetAllBaseTypes(obj.GetActualType()) : NoTypes;

        public static IEnumerable<Type> GetAllBaseTypes(Type t) =>
            ReflectionUtility.GetAllBaseTypes(t).Concat(t.GetInterfaces());

        /// <summary>
        /// This Methode extends the System.Type-type to get all extended methods. It searches hereby in all assemblies which are known by the current AppDomain.
        /// </summary>
        /// <remarks>
        /// Insired by Jon Skeet from his answer on http://stackoverflow.com/questions/299515/c-sharp-reflection-to-identify-extension-methods
        /// </remarks>
        /// <returns>returns MethodInfo[] with the extended Method</returns>
        public static MethodInfo[] GetExtensionMethods(this Type t)
        {
            List<Type> AssTypes = new List<Type>();

            foreach (Assembly item in AppDomain.CurrentDomain.GetAssemblies())
            {
                AssTypes.AddRange(item.GetTypes());
            }

            var query = from type in AssTypes
                where type.IsSealed && !type.IsGenericType && !type.IsNested
                from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                where method.IsDefined(typeof(ExtensionAttribute), false)
                where method.GetParameters()[0].ParameterType == t
                select method;
            return query.ToArray<MethodInfo>();
        }

        /// <summary>
        /// Extends the System.Type-type to search for a given extended MethodeName.
        /// </summary>
        /// <param name="MethodeName">Name of the Methode</param>
        /// <returns>the found Methode or null</returns>
        public static MethodInfo GetExtensionMethod(this Type t, string MethodeName)
        {
            var mi = from methode in t.GetExtensionMethods()
                where methode.Name == MethodeName
                select methode;
            if (mi.Count<MethodInfo>() <= 0)
                return null;
            else
                return mi.First<MethodInfo>();
        }
    }
}
