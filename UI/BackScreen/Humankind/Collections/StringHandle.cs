using System;
using System.Collections.Generic;
using System.Text;

namespace DevTools.Humankind.GUITools.Collections
{
    public interface IStringIdentity
    {
        string ToIdentityString();
        StringHandle FromIdentityString(string value);
    }

    /// <summary>
    /// Clone of StaticString's implementation
    /// </summary>
    public struct StringHandle : IComparable<StringHandle>, IEquatable<StringHandle>, IStringIdentity
    {
        private static readonly char IdentityMark = '⁫';

        public static readonly StringHandle Empty;

        public int Handle;

        private static Dictionary<string, int> handles = new Dictionary<string, int>();

        private static List<string> strings;

        static StringHandle()
        {
            StringHandle.handles.Add(string.Empty, 0);
            StringHandle.strings = new List<string>();
            StringHandle.strings.Add(string.Empty);
            StringHandle.Empty = new StringHandle();
        }

        public StringHandle(string x)
        {
            if (string.IsNullOrEmpty(x))
            {
                this.Handle = 0;
            }
            else
            {
                lock (StringHandle.handles)
                {
                    int num;
                    if (StringHandle.handles.TryGetValue(x, out num))
                    {
                        this.Handle = num;
                    }
                    else
                    {
                        this.Handle = StringHandle.strings.Count;
                        StringHandle.strings.Add(x);
                        StringHandle.handles.Add(x, this.Handle);
                    }
                }
            }
        }

        public StringHandle(StringHandle x) => this.Handle = x.Handle;

        public StringHandle(int handle) => this.Handle = handle;

        public static bool operator ==(StringHandle x, StringHandle y) =>
            x.Handle == y.Handle;

        public static bool operator !=(StringHandle x, StringHandle y) =>
            x.Handle != y.Handle;

        public static bool IsNullOrEmpty(StringHandle x) => x.Handle == 0;

        public static string Join(string separator, StringHandle[] collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (collection.Length == 0)
                return string.Empty;
            if (collection.Length == 1)
                return collection[0].ToString();
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < collection.Length; ++index)
            {
                if (index > 0)
                    stringBuilder.Append(separator);
                stringBuilder.Append((object) collection[index]);
            }

            return stringBuilder.ToString();
        }

        public static bool Exists(string x) =>
            string.IsNullOrEmpty(x) || StringHandle.handles.ContainsKey(x);

        public override bool Equals(object x)
        {
            if (x == null)
                return this.Handle == 0;
                
            if (x is StringHandle stringHandle)
                return stringHandle.Handle == this.Handle;
            
            // TODO: Should equality with strings be implemented?

            return false;
        }

        public bool Equals(StringHandle other) => this.Handle == other.Handle;

        public override int GetHashCode() => this.Handle;

        public int CompareTo(StringHandle obj) => string.CompareOrdinal(
            StringHandle.strings[this.Handle], StringHandle.strings[obj.Handle]);

        public int CompareTo(ref StringHandle obj) =>
            string.CompareOrdinal(StringHandle.strings[this.Handle],
                StringHandle.strings[obj.Handle]);

        public int CompareHandleTo(StringHandle obj) => this.Handle.CompareTo(obj.Handle);

        public override string ToString() => StringHandle.strings[this.Handle];

        public string ToIdentityString() => "" + IdentityMark + Handle;

        public StringHandle FromIdentityString(string value) => Parse(value);

        public static StringHandle Parse(string value) => 
            (value.Length > 1 && value[0] == IdentityMark &&
             uint.TryParse(value.Substring(1), out var handle))
                ? new StringHandle((int)handle)
                : new StringHandle(value);
    }
}
