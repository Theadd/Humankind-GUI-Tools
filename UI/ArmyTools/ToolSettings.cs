using UnityEngine;
using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI.ArmyTools
{
    
    public class ToolSettings
    {
        public BooleanSetting WindowTransparency = new BooleanSetting(
            "TRANSPARENCY", 
            "Toggles tool window transparency on/off", 
            true
        );
        public BooleanSetting TitleBar = new BooleanSetting(
            "TITLE BAR", 
            "Shows/hides tool window title bar", 
            true
        );
        public BooleanSetting EndlessMove = new BooleanSetting(
            "ENDLESS MOVE", 
            "Grants infinite movement of armies that were selected when giving the move order", 
            true
        );
        public BooleanSetting SkipOnMoveEnd = new BooleanSetting(
            "<size=11>SKIP</size><size=7>ON</size><size=9>MOVE</size><size=11>END</size>", 
            "When an army with ENDLESS MOVE stops running, set it to skip one turn state", 
            true
        );
        public BooleanSetting LocalizedTitles = new BooleanSetting(
            "LOCALIZED TITLES", 
            "When active, unit names are displayed as you see them in game. Based on settings > ui > language", 
            true
        );

        public static ToolSettings Instance;

        public ToolSettings() {
            Instance = this;
        }

        public void WritePlayerPreferences(FloatingToolWindow Window, ArmyController Controller)
        {
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("GameID"), Controller.GameID);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("SelectedEmpireIndex"), Controller.SelectedEmpireIndex);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("SelectedArmyEntityGUID"), (int) Controller.SelectedArmyEntityGUID);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("LocalizedTitles"), LocalizedTitles.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("WindowTransparency"), WindowTransparency.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("TitleBar"), TitleBar.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("EndlessMove"), EndlessMove.Value ? 1 : 0);
            PlayerPrefs.SetInt(Window.GetPlayerPrefKey("SkipOnMoveEnd"), SkipOnMoveEnd.Value ? 1 : 0);
        }
        public void ReadPlayerPreferences(FloatingToolWindow Window, ArmyController Controller)
        {
            var gameIdKey = Window.GetPlayerPrefKey("GameID");

            if (PlayerPrefs.HasKey(gameIdKey) && PlayerPrefs.GetInt(gameIdKey) == Controller.GameID)
            {
                var selectedEmpireIndex = PlayerPrefs.GetInt(Window.GetPlayerPrefKey("SelectedEmpireIndex"), 0);
                var selectedArmyEntityGUID = (ulong) PlayerPrefs.GetInt(Window.GetPlayerPrefKey("SelectedArmyEntityGUID"), 0);
                
                if (selectedEmpireIndex != Controller.SelectedEmpireIndex || selectedArmyEntityGUID != Controller.SelectedArmyEntityGUID)
                    Controller.SetSelectedEmpire(selectedEmpireIndex, selectedArmyEntityGUID); 
            }

            LocalizedTitles.Value = PlayerPrefs.GetInt(Window.GetPlayerPrefKey("LocalizedTitles"), LocalizedTitles.Value ? 1 : 0) != 0;
            WindowTransparency.Value = PlayerPrefs.GetInt(Window.GetPlayerPrefKey("WindowTransparency"), WindowTransparency.Value ? 1 : 0) != 0;
            TitleBar.Value = PlayerPrefs.GetInt(Window.GetPlayerPrefKey("TitleBar"), TitleBar.Value ? 1 : 0) != 0;
            EndlessMove.Value = PlayerPrefs.GetInt(Window.GetPlayerPrefKey("EndlessMove"), EndlessMove.Value ? 1 : 0) != 0;
            SkipOnMoveEnd.Value = PlayerPrefs.GetInt(Window.GetPlayerPrefKey("SkipOnMoveEnd"), SkipOnMoveEnd.Value ? 1 : 0) != 0;
        }
    }

}
