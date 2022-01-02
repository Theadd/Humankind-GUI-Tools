using UnityEngine;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using Amplitude.Mercury.Interop.AI.Entities;
using DevTools.Humankind.GUITools.UI.ArmyTools;


namespace DevTools.Humankind.GUITools.UI
{
    public class ArmyToolsWindow : FloatingToolWindow
    {
        public override string WindowTitle { get; set; } = " A R M Y   T O O L S";
        public override string WindowGUIStyle { get; set; } = "PopupWindow";
        public override Rect WindowRect { get; set; } = new Rect(980f, 150f, 410f, 300f);
        public override bool ShouldBeVisible => true;
        public override bool ShouldRestoreLastWindowPosition => true;
        public Texture2D HeaderImage { get; set; } = Texture2D.blackTexture;//Modding.Humankind.DevTools.DevTools.Assets.Load<Texture2D>("GameplayOrientation_Warmonger");
        public string GUITooltip { get; private set; } = "";
        public ToolSettings Settings => Controller.Settings;
        private readonly ArmyController Controller = new ArmyController();
        public override void OnGUIStyling()
        {
            base.OnGUIStyling();
            UIManager.DefaultSkin.label.fontSize = 13;
            GUI.backgroundColor = new Color32(255, 255, 255, Settings.WindowTransparency.Value == true ? (byte)230 : (byte)255);
        }

        public override void OnDrawUI()
        {
            // Draw default window title bar
            if (Settings.TitleBar.Value)
                WindowUtils.DrawWindowTitleBar(this);

            GUILayout.BeginVertical("Widget.ClientArea"); 
                
                OnDrawWindowHeader();

                if (Controller.IsValidGame())
                {
                    Controller.SyncArmyCursor();
                    OnDrawEmpireSection();
                    OnDrawArmiesSection();
                    OnDrawSettings();

                    if (Event.current.type == EventType.Layout)
                        Controller.SyncBackgroundTasks();
                }
                else
                {
                    OnDrawSettings();
                    GUILayout.Label("\n\t<b>WAITING FOR A VALID GAME STATE</b>\n");
                }
                
                if (Event.current.type == EventType.Repaint)
                    GUITooltip = GUI.tooltip;

                GUILayout.Space(10f);
                    
            GUILayout.EndVertical();

        }

        private void OnDrawSettings()
        {
            GUILayout.Label("T O O L   S E T T I N G S", "PopupWindow.SectionHeader");
            
            GUILayout.BeginVertical();
                GUILayout.Space(8f);

                GUILayout.BeginHorizontal();
                    GUILayout.Space(8f);
                    GUILayout.BeginHorizontal();
                        Settings.WindowTransparency.Draw();
                        Settings.TitleBar.Draw();
                        Settings.EndlessMove.Draw();
                        Settings.SkipOnMoveEnd.Draw();
                    GUILayout.EndHorizontal();
                    GUILayout.Space(12f);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                    GUILayout.Space(8f);
                    GUILayout.BeginHorizontal();
                        Settings.LocalizedTitles.Draw();
                    GUILayout.EndHorizontal();
                    GUILayout.Space(12f);
                GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        private void OnDrawWindowHeader()
        {
            GUILayout.Space(Settings.TitleBar.Value ? 0f : 10f);
            GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(GUILayout.ExpandWidth(false), GUILayout.Width(78f), GUILayout.ExpandHeight(false), GUILayout.Height(78f));
                    GUILayoutUtility.GetRect(64f, 64f);
                    GUI.DrawTexture(  
                        new Rect(18f, Settings.TitleBar.Value ? 42f : 10f, 64f, 64f), 
                        HeaderImage, 
                        ScaleMode.ScaleToFit, 
                        true, 
                        1f,
                        new Color32(255, 255, 255, 240), 
                        0, 
                        0
                    );
                GUILayout.EndVertical();
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false), GUILayout.Height(82f), GUILayout.MaxHeight(82f));
                    GUILayout.Label(
                        ("<size=4>\n</size><size=10><b>  Showcasing</b> a fully featured demo of an " + 
                        "<b>in-game<size=3>\n\n</size>  Tool</b> made with <color=#1199EECC><b>Humankind's Modding DevTools</b></color>" + 
                        "</size><size=3>\n</size>").ToUpper(), "Text");
                    GUILayout.FlexibleSpace();
                    GUI.color = new Color(1f, 1f, 1f, 0.5f);
                    GUILayout.Label(R.Text.Size(R.Text.Bold(GUITooltip.ToUpper()), 9), "Tooltip");
                    GUI.color = Color.white;
                GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void OnDrawEmpireSection()
        {
            GUILayout.Label("M A J O R   E M P I R E S", "PopupWindow.SectionHeader");

            var selectedEmpireIndex = Controller.EmpireGrid.Draw<GUIContent>();

            if (selectedEmpireIndex != Controller.SelectedEmpireIndex) {
                Controller.SetSelectedEmpire(selectedEmpireIndex);
            }
        }

        private void OnDrawArmiesSection()
        {
            GUILayout.Label(
                "A R M I E S   O F   E M P I R E   " + 
                R.Text.Color(Controller.SelectedEmpireIndex.ToString(), 
                    Controller.SelectedEmpireIndex < HumankindGame.Empires.Length ? 
                        HumankindGame.Empires[Controller.SelectedEmpireIndex].PrimaryColor : "#95ADC6FF"), 
                "PopupWindow.SectionHeader"
            );

            var selectedArmyIndex = Controller.ArmyGrid.Draw<GUIContent>();

            if (selectedArmyIndex >= 0)
            {
                if (selectedArmyIndex != Controller.SelectedArmyIndex) {
                    Controller.Armies[selectedArmyIndex].CenterToCamera();
                    Controller.Armies[selectedArmyIndex].SelectArmy();
                    Controller.SelectedArmyIndex = selectedArmyIndex;
                }

                GUILayout.Label("S E L E C T E D   A R M Y", "PopupWindow.SectionHeader");

                Army army = Controller.Armies[Controller.SelectedArmyIndex] as Army;
                DrawArmy(army);
            }
            else
            {
                GUILayout.Label("N O   A R M Y   S E L E C T E D", "PopupWindow.SectionHeader");
                // GUI.skin.FindStyle("PopupWindow.Heading1").fontSize = prevFontSize;
                
                // GUI.skin.font = Shared.CustomFont;
                GUILayout.Label("<size=11><b>SELECT AN ARMY TO SEE INFORMATION ABOUT IT</b></size>");
            }
        }

        private void DrawArmy(Army army)
        {
            // If EndlessMove is active and selected army is running, push this army to the moving armies list
            if (Settings.EndlessMove.Value && Event.current.type == EventType.Layout && ArmyUtils.IsRunning(army))
                Controller.AddToEndlessMovingArmies(army);

            GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false));
                    GUILayout.Label(R.Text.Bold(R.Text.Size(ArmyUtils.SummaryOfSelectedArmy(army), 10)), "Text");
                GUILayout.EndVertical();
                // GUILayout.FlexibleSpace();
                // GUILayout.BeginVertical(GUILayout.ExpandWidth(false));
                    
                // GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        public override void OnWritePlayerPreferences()
        {
            base.OnWritePlayerPreferences();

            Settings.WritePlayerPreferences(this, Controller);
        }

        public override void OnReadPlayerPreferences()
        {
            base.OnReadPlayerPreferences();

            Settings.ReadPlayerPreferences(this, Controller);
        }
    }
}
