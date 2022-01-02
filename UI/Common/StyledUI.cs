using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools;
using Amplitude.Mercury.Interop.AI.Entities;

namespace DevTools.Humankind.GUITools.UI
{
    public static class StyledUI
    {
        public static string GridItem(HumankindEmpire e) => R.Text.Color(
            R.Text.Bold(e.EmpireIndex.ToString() + ". " + e.PersonaName.ToUpper()), e.PrimaryColor);

        public static string GridItem(Army army) => Tag.Badge(army.Units.Length.ToString()) + Tag.Separator +
            Tag.ListItem(ArmyTools.ArmyUtils.ArmyUnitNames(army)) + Tag.Separator + 
            Tag.BarelyVisible(army.EntityGUID.ToString()) + Tag.Separator + ArmyTools.ArmyUtils.ArmyTags(army);

        public static class Tag
        {
            public static string Separator = "  ";
            public static string State(string text) => R.Text.Color(text.ToUpper(), "#FFD700A5");
            public static string Common(string text) => R.Text.Color(text.ToUpper(), "#00000090");
            public static string Success(string text) => R.Text.Color(text, "#009D13CC");
            public static string Warn(string text) => R.Text.Color(text, "#FF333399");
            public static string Hot(string text) => R.Text.Bold(R.Text.Color(text, "#E1E500BF"));
            public static string Class(string text) => R.Text.Color(text.ToUpper(), "#4169E1FF");
            public static string Link(string text) => R.Text.Color(text.ToUpper(), "#337AB7FF");
            public static string ListItem(string text) => R.Text.Color(text, "#8CDAFFAA");
            public static string BarelyVisible(string text) => R.Text.Color(text, "#FFFFFF40");
            public static string Badge(string text) => "<b><size=14><color=#FFFFFF20>" + text + "</color></size></b> ";
        }
    }
}
