using DevTools.Humankind.GUITools.Collections;
using DevTools.Humankind.GUITools.UI.Humankind;
using UnityEngine;

namespace DevTools.Humankind.GUITools
{
    public static class StringHandleEx
    {
        public static GUIContent ToGUIContent(this StringHandle self) =>
            new GUIContent(/* TODO: */ self.ToString(), self.ToIdentityString());

        public static string GetLocalizedTitle(this StringHandle self) =>
            Storage.Get<ITextContent>(self)?.Title ?? self.ToString();
        
        public static string GetLocalizedDescription(this StringHandle self) =>
            Storage.Get<ITextContent>(self)?.Description ?? self.ToString();
    }
}
