using System;
using System.Collections;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace DevTools.Humankind.GUITools.UI.PauseMenu
{
    
    public class InGameMenuWindow : FloatingToolWindow
    {

        public override string WindowTitle { get; set; } = "GAME MENU";

        public override bool ShouldBeVisible => InGameMenuController.IsVisible && UIController.IsAmplitudeUIVisible && ViewController.View == ViewType.InGame;

        public override bool ShouldRestoreLastWindowPosition => false;
        
        public static float FixedWidth => Screen.width < 1920f ? 315f : 410f;

        public static bool IsBigScreen => Screen.width >= 1920f;

        public override Rect WindowRect { get; set; } = new Rect(Screen.width - FixedWidth, 0, FixedWidth, Screen.height);

        public static string LatestVersion { get; private set; }
        public static bool VersionFound { get; private set; }

        private GUIStyle CenteredLink { get; set; } = new GUIStyle(UIController.DefaultSkin.FindStyle("Link")) {
            alignment = TextAnchor.MiddleCenter,
            margin = new RectOffset(4, 4, 4, 0),
            padding = new RectOffset(4, 4, 3, 0)
        };
        private GUIStyle CenteredText { get; set; } = new GUIStyle(UIController.DefaultSkin.FindStyle("Link")) {
            alignment = TextAnchor.MiddleCenter,
            margin = new RectOffset(4, 4, 0, 4),
            padding = new RectOffset(4, 4, 0, 3)
        };

        private GUIStyle CustomTooltip { get; set; } = new GUIStyle(UIController.DefaultSkin.FindStyle("Tooltip")) {
            alignment = TextAnchor.LowerCenter,
            stretchHeight = true,
            fixedHeight = 32f,
            normal = new GUIStyleState() {
                background = null,
                textColor = Color.white
            }
        };

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
            StartCoroutine(GetLatestVersion());
        }
        

        IEnumerator GetLatestVersion() {
            UnityWebRequest www = UnityWebRequest.Get("https://raw.githubusercontent.com/Theadd/Humankind-GUI-Tools/main/VERSION");
            yield return www.SendWebRequest();
 
            if (www.result != UnityWebRequest.Result.Success) {
                Loggr.Log(www.error, ConsoleColor.Red);
                VersionFound = false;
            }
            else
            {
                LatestVersion = www.downloadHandler.text;
                VersionFound = true;
                Loggr.Log(LatestVersion, ConsoleColor.Green);
            }
        }

        public override void OnGUIStyling()
        {
            base.OnGUIStyling();
            GUI.backgroundColor = new Color32(255, 255, 255, 230);
            WindowRect = new Rect(Screen.width - FixedWidth, 0, FixedWidth, Screen.height);
        }

        public static void CloseInGameMenu()
        {
            Amplitude.Mercury.UI.PauseMenuModalWindow window = Amplitude.Mercury.UI.Helpers.WindowsUtils.GetWindow<Amplitude.Mercury.UI.PauseMenuModalWindow>();
            Amplitude.Mercury.UI.Helpers.WindowsUtils.UpdateWindowVisibility(window, false, false);
        }

        private int yStart = 71;
        private int contentHeight = 0;
        private Vector2 contentScrollPos = Vector2.zero;
        private static int loop = 0;
        private static bool isLateRepaint = false;

        public static void ResetLoop()
        {
            loop = 0;
            isLateRepaint = false;
        }

        public override void OnDrawUI()
        {
            if (Event.current.type == EventType.Repaint)
            {
                isLateRepaint = loop == 0;
                loop = loop == 20 ? 0 : loop + 1;
            }
            // Already done in ShouldBeVisible: if (PauseMenuController.IsVisible)
            BackgroundFader.OnAnimateBackgroundColor();
            
            GUILayout.BeginHorizontal();
                GUILayout.Space(IsBigScreen ? 20f : 6f);
            
                GUILayout.BeginVertical(GUILayout.ExpandHeight(true));
                    GUILayout.Space(IsBigScreen ? 52f : 38f);
                    Utils.DrawH1("HUMANKIND GUI TOOLS", false);

                    if (isLateRepaint)
                    {
                        yStart = (int) GUILayoutUtility.GetLastRect().y;
                        contentHeight = Screen.height - 84 - yStart;
                    }

                    contentScrollPos = GUILayout.BeginScrollView(
                        contentScrollPos, 
                        false, 
                        false, 
                        "horizontalscrollbar",
                        "verticalscrollbar",
                        "scrollview",
                        new GUILayoutOption[]
                    {
                        GUILayout.MaxHeight(contentHeight)
                    });
                    GUILayout.Space(12f);

                    OnDrawWindowHeader();

                    OnDrawWindowContent();

                    GUILayout.EndScrollView();
                    
                GUILayout.EndVertical();

                GUILayout.Space(IsBigScreen ? 20f : 12f);
            GUILayout.EndHorizontal();
            
            
            OnDrawTooltip();
        }

        private void OnDrawWindowHeader()
        {
            Utils.DrawText("Customize keys and discover more features by switching to the " + BlueText("Keyboard Shortcuts") + " tab in the " + BlueText("Game Overview") + " window.");

            GUILayout.Space(IsBigScreen ? 12f : 8f);

            GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("<size=10><b>GAME OVERVIEW</b></size>"))
                {
                    //PopupToolWindow.Open<GameStatsWindow>(w => {});
                    MainTools.ToggleGameOverviewWindow();
                    CloseInGameMenu();
                    ViewController.ViewMode = ViewModeType.Overview;
                }
                    
                if (GUILayout.Button("<size=10><b>END GAME STATISTICS</b></size>"))
                {
                    //PopupToolWindow.Open<EndGameStatisticsWindow>(w => {});
                    MainTools.ToggleEndGameStatisticsWindow();
                    CloseInGameMenu();
                    ViewController.ViewMode = ViewModeType.EndGame;
                }
                    
                GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(IsBigScreen ? 12f : 8f);
        }

        private int activeTab = 0;

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
            GlobalSettings.StatisticsAndAchievementsTool.Draw();
            GUI.enabled = false;
            GlobalSettings.AffinityTool.Draw();
            GUI.enabled = true;
            GUILayout.Space(8f);
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
            GlobalSettings.TerrainPickingTool.Draw();
            GlobalSettings.GameInfoTool.Draw();
            GUI.enabled = false;
            GlobalSettings.ArchetypesTool.Draw();
            GUI.enabled = true;
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            
            var enableProfilingTools = GlobalSettings.ProfilingTools.Draw();
            GUILayout.BeginHorizontal();
            GUILayout.Space(48f);
            GUILayout.BeginVertical();
            GUI.enabled = enableProfilingTools;
            GlobalSettings.FramerateTool.Draw();
            // GlobalSettings.GPUProfilerTool.Draw(); 
            GlobalSettings.GraphicsTool.Draw();
            GlobalSettings.MemoryProfilerTool.Draw();
            GUI.enabled = true;
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            
            var enableExperimentalTools = GlobalSettings.ExperimentalTools.Draw();
            GUILayout.BeginHorizontal();
            GUILayout.Space(48f);
            GUILayout.BeginVertical();
            GUI.enabled = enableExperimentalTools;
            GlobalSettings.AITool.Draw();
            GlobalSettings.BattleAITool.Draw();
            GlobalSettings.DistrictPainterTool.Draw();
            GlobalSettings.SettlementTools.Draw();
            GlobalSettings.FameTool.Draw();
            GlobalSettings.EndGameTool.Draw();
            GUI.enabled = true;
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
        }

        /*private GUIStyle noStyle = new GUIStyle(UIController.DefaultSkin.FindStyle("PopupWindow.Sidebar.Highlight")) {
            margin = new RectOffset(0, 0, 0, 0),
            padding = new RectOffset(0, 0, 0, 0)
        };*/

        private void OnDrawTooltip()
        {
            GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false), GUILayout.Height(82f), GUILayout.MaxHeight(82f));

            GUI.backgroundColor = Color.black;
            GUILayout.BeginVertical("PopupWindow.Sidebar.Heading", GUILayout.ExpandHeight(false));
            if (GUILayout.Button(("<size=10>Check out " + BlueText("<b>Humankind GUI Tools</b>") + " on <b>GitHub</b>!</size>").ToUpper(), CenteredLink))
            {
                Application.OpenURL("https://github.com/Theadd/Humankind-GUI-Tools");
            }
            GUILayout.Label("<size=10>It's <b>Open Source</b>! Feel free to contribute!</size>", CenteredText);
            GUILayout.EndVertical();
            GUI.backgroundColor = Color.white;

            GUILayout.Label(R.Text.Size((GUI.tooltip ?? "").ToUpper(), 9), CustomTooltip);
            GUI.color = Color.white;
            GUILayout.EndVertical();
        }
        
        public override void OnWritePlayerPreferences()
        {
            base.OnWritePlayerPreferences();

            GlobalSettings.WritePlayerPreferences(this);
            KeyMappings.WritePlayerPreferences(this);
        }

        public override void OnReadPlayerPreferences()
        {
            GlobalSettings.ResetPlayerPreferencesIfNecessary(this);
            
            base.OnReadPlayerPreferences();

            GlobalSettings.ReadPlayerPreferences(this);
            KeyMappings.ReadPlayerPreferences(this);
            KeyMappings.Apply();
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

        public static float Duration { get; set; } = 2f;

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
