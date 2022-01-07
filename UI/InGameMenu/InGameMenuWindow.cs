using System;
using System.Collections;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI.PauseMenu
{
    
    public class InGameMenuWindow : FloatingToolWindow
    {

        public override string WindowTitle { get; set; } = "GAME MENU";

        public override bool ShouldBeVisible => InGameMenuController.IsVisible;

        public override bool ShouldRestoreLastWindowPosition => false;
        
        public static float FixedWidth => Screen.width < 1920f ? 315f : 410f;

        public static bool IsBigScreen => Screen.width >= 1920f;

        public override Rect WindowRect { get; set; } = new Rect(Screen.width - FixedWidth, 0, FixedWidth, Screen.height);

        protected override void Awake()
        {
            base.Awake();
            OnReadPlayerPreferences();
            MainTools.Toolbar.RestoreVisibleWindows();
        }

        protected override IEnumerator Start()
        {
            while (InGameMenuController.PauseMenuWindow == null || InGameMenuController.Background == null)
            {
                InGameMenuController.Resync();
                
                yield return new WaitForSeconds(1.5f);
            }
        }

        public override void OnGUIStyling()
        {
            base.OnGUIStyling();
            GUI.backgroundColor = new Color32(255, 255, 255, 230);
            WindowRect = new Rect(Screen.width - FixedWidth, 0, FixedWidth, Screen.height);
        }

        public override void OnDrawUI()
        {
            // Already done in ShouldBeVisible: if (PauseMenuController.IsVisible)
            BackgroundFader.OnAnimateBackgroundColor();
            
            GUILayout.BeginHorizontal();
                GUILayout.Space(IsBigScreen ? 20f : 6f);
            
                GUILayout.BeginVertical(GUILayout.ExpandHeight(true));
                    GUILayout.Space(IsBigScreen ? 52f : 38f);
                    OnDrawWindowHeader();

                    OnDrawWindowContent();
                    
                GUILayout.EndVertical();

                GUILayout.Space(IsBigScreen ? 20f : 12f);
            GUILayout.EndHorizontal();
            
            
            OnDrawTooltip();
        }

        private void OnDrawWindowHeader()
        {
            Utils.DrawH1("HUMANKIND GUI TOOLS");
            
            Utils.DrawText("Press the " + GreenText("[<size=10> INSERT </size>]") + " key to toggle the visibility " + 
                           "of all GUI windows made using the " + BlueText("Humankind Modding DevTools") + " library.");
            
            GUILayout.Space(IsBigScreen ? 12f : 8f);
        }

        private int activeTab = 1;

        private string[] tabNames = new[]
            {"<size=10><b> GLOBAL SETTINGS </b></size>", "<size=10><b> ADD/REMOVE TOOLS </b></size>"};
        
        private void OnDrawWindowContent()
        {
            Utils.DrawH2("SETTINGS");
            Utils.DrawText("All settings are stored in <b>Unity Player Preferences</b> and also preserve their values when game exits.\n");
            // GUILayout.Space(4f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(6f);

            activeTab = GUILayout.Toolbar(activeTab, tabNames, "TabButton", GUI.ToolbarButtonSize.FitToContents);
            GUILayout.EndHorizontal();

            Utils.DrawHorizontalLine();
            GUILayout.Space(12f);

            switch (activeTab)
            {
                case 0:
                    DrawGlobalSettingsTab();
                    break;
                case 1:
                    DrawAddRemoveToolsTab();
                    break;
            }
        }
        
        private void DrawGlobalSettingsTab()
        {
            GUILayout.BeginVertical();
                GlobalSettings.WindowTransparency.Draw(/*"PopupWindow.Row"*/);
                GUILayout.Space(8f);
                GlobalSettings.WindowTitleBar.Draw(/*"PopupWindow.RowEven"*/);
                GUILayout.Space(8f);
                GlobalSettings.HideToolbarWindow.Draw(/*"PopupWindow.Rown"*/);
                
                    
            GUILayout.EndVertical();
        }
        
        private void DrawAddRemoveToolsTab()
        {
            GUILayout.BeginVertical();
            
            var enableCheatingTools = GlobalSettings.CheatingTools.Draw();
            GUILayout.BeginHorizontal();
            GUILayout.Space(48f);
            GUILayout.BeginVertical();
            GUI.enabled = enableCheatingTools;
            GlobalSettings.MilitaryTool.Draw();
            GlobalSettings.ArmyTool.Draw();
            GlobalSettings.TechnologyTool.Draw();
            GlobalSettings.ResourcesTool.Draw();
            GUI.enabled = false;
            GlobalSettings.AffinityTool.Draw();
            GUI.enabled = true;
            GUILayout.Space(8f);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            
            var enableProfilingTools = GlobalSettings.ProfilingTools.Draw();
            GUILayout.BeginHorizontal();
            GUILayout.Space(48f);
            GUILayout.BeginVertical();
            GUI.enabled = enableProfilingTools;
            GlobalSettings.FramerateTool.Draw();
            GlobalSettings.GPUProfilerTool.Draw();
            GlobalSettings.MemoryProfilerTool.Draw();
            GUI.enabled = true;
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            
            var enableDeveloperTools = GlobalSettings.DeveloperTools.Draw();
            GUILayout.BeginHorizontal();
            GUILayout.Space(48f);
            GUILayout.BeginVertical();
            GUI.enabled = enableDeveloperTools;
            GlobalSettings.AutoTurnTool.Draw();
            GlobalSettings.CivicsTool.Draw();
            GlobalSettings.CollectiblesTool.Draw();
            GlobalSettings.DiplomacyTool.Draw();
            GUI.enabled = false;
            GlobalSettings.ArchetypesTool.Draw();
            GUI.enabled = true;
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            
            var enableExperimentalTools = GlobalSettings.ExperimentalTools.Draw();
            GUILayout.BeginHorizontal();
            GUILayout.Space(48f);
            GUILayout.BeginVertical();
            GUI.enabled = enableExperimentalTools;
            GlobalSettings.AITool.Draw();
            GUI.enabled = true;
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
        }

        private void OnDrawTooltip()
        {
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false), GUILayout.Height(82f), GUILayout.MaxHeight(82f));
            GUILayout.Label(
                ("<size=4>\n</size><size=10><b>  Showcasing</b> a fully featured demo of an " + 
                 "<b>in-game<size=3>\n\n</size>  Tool</b> made with <color=#1199EECC><b>Humankind Modding DevTools</b></color>" + 
                 "</size><size=3>\n</size>").ToUpper(), "Text");
            GUILayout.FlexibleSpace();
            GUI.color = new Color(1f, 1f, 1f, 0.5f);
            GUILayout.Label(R.Text.Size(R.Text.Bold(GUI.tooltip.ToUpper()), 9), "Tooltip");
            GUI.color = Color.white;
            GUILayout.EndVertical();
        }
        
        public override void OnWritePlayerPreferences()
        {
            base.OnWritePlayerPreferences();

            GlobalSettings.WritePlayerPreferences(this);
        }

        public override void OnReadPlayerPreferences()
        {
            base.OnReadPlayerPreferences();

            GlobalSettings.ReadPlayerPreferences(this);
        }

        private static string GreenText(string text) => "<color=#33DD33DC>" + text + "</color>";
        
        private static string BlueText(string text) => "<color=#5588FEF0>" + text + "</color>";
    }

    public static class BackgroundFader
    {
        public static Color InitialColor { get; set; } = Color.clear;
        // public static Color TargetColor { get; set; } = new Color(0f, 0f, 0f, 0.65f); 
        public static Color TargetColor { get; set; } = new Color(0.05f, 0.1f, 0.1f, 0.65f);
        public static Color ColorVariation { get; set; } = new Color(0.3f, 0.4f, 0.1f, 0.6f);

        public static float Duration { get; set; } = 5f;

        public static bool IsDoneAnimating => t >= 1;

        private static float t = 0;

        public static void StartAnimation() => t = 0;

        public static void OnAnimateBackgroundColor(bool repeat = false)
        {
            if (repeat && t >= 1)
                t = 0;
            
            InGameMenuController.SetBackgroundColor(Color.Lerp(InitialColor, TargetColor, t));
  
            if (t < 1)
                t += Time.deltaTime / Duration;
        }
    }
}
