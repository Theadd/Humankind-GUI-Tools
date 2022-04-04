using System;
using System.Collections.Generic;
using System.Linq;
using Amplitude.Mercury.Interop;
using UnityEngine;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using Amplitude.Mercury.Interop.AI.Entities;
using Amplitude.Mercury.Presentation;


namespace DevTools.Humankind.GUITools.UI
{
    public class ArmyToolsWindow : FloatingToolWindow
    {
        public override string WindowTitle { get; set; } = " A R M Y   T O O L S";
        public override string WindowGUIStyle { get; set; } = "PopupWindow";
        public override Rect WindowRect { get; set; } = new Rect(980f, 150f, 410f, 300f);
        public override bool ShouldBeVisible => !GlobalSettings.ShouldHideTools;
        public override bool ShouldRestoreLastWindowPosition => true;
        public Texture2D HeaderImage { get; set; } = Modding.Humankind.DevTools.DevTools.Assets.Load<Texture2D>("GameplayOrientation_Warmonger");
        public string GUITooltip { get; private set; } = "";
        public ArmyToolSettings Settings => Controller.Settings;
        private readonly ArmyController Controller = new ArmyController();
        public override void OnGUIStyling()
        {
            base.OnGUIStyling();
            UIController.DefaultSkin.label.fontSize = 13;
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
                        "<b>in-game<size=3>\n\n</size>  Tool</b> made with <color=#1199EECC><b>Humankind Modding DevTools</b></color>" + 
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


    public class ArmyController
    {
        public int SelectedEmpireIndex { get; set; } = 0;
        public int SelectedArmyIndex { get; set; } = 0;

        // Currently selected Army's EntityGUID by the tool
        public ulong SelectedArmyEntityGUID { get; set; } = 0;

        // Currently selected Army's EntityGUID in game
        public ulong ArmyCursorEntityGUID { get; private set; } = 0;

        // EmpireIndex of currently selected Army in game
        public int ArmyCursorEmpireIndex { get; private set; } = 0;
        public Army[] Armies;
        public StaticGrid EmpireGrid;
        public ScrollableGrid ArmyGrid;
        public PresentationArmy SelectedPresentationArmy { get; private set; }
        public ArmyToolSettings Settings { get; private set; }
        public int GameID => _gameId;
        private bool _isValidGame = false;
        private int _gameId;

        public ArmyController(/*ToolSettings settings = null*/)
        {
            Settings = new ArmyToolSettings();
            // Settings = settings == null ? new ToolSettings() : settings;
        }

        public bool IsValidGame()
        {
            if (HumankindGame.IsGameLoaded)
            {
                if (!_isValidGame || _gameId != HumankindGame.GameID)
                    Initialize();
                _gameId = HumankindGame.GameID;
                
                return (_isValidGame = true);
            }

            return (_isValidGame = false);
        }

        protected void Initialize()
        {
            SelectedEmpireIndex = 0;
            Armies = HumankindGame.Empires[SelectedEmpireIndex].Armies.ToArray();
            SelectedArmyIndex = 0;

            EmpireGrid = new StaticGrid() {
                ItemsPerRow = HumankindGame.Empires.Length > 3 ? 3 : HumankindGame.Empires.Length,
                Items = GetEmpiresGridItems(),
                SelectedIndex = 0
            };
            ArmyGrid = new ScrollableGrid() {
                ItemsPerRow = 1,
                Items = ArmiesAsGridItems(Armies.AsEnumerable()).ToArray(),
            };
        }

        public void SyncArmyCursor()
        {
            if (Snapshots.ArmyCursorSnapshot == null)
                return;
        
            if (Presentation.PresentationCursorController.CurrentCursor is ArmyCursor armyCursor)
            {
                if (ArmyCursorEntityGUID == 0 && armyCursor.EntityGUID == SelectedArmyEntityGUID)
                    ArmyCursorEntityGUID = armyCursor.EntityGUID;

                if (armyCursor.EntityGUID == SelectedArmyEntityGUID)
                {
                    if (SelectedPresentationArmy == null || SelectedPresentationArmy.SimulationEntityGuid != (ulong) SelectedArmyEntityGUID)
                    {
                        SetPresentationArmy((ulong) armyCursor.EntityGUID);
                        ArmyCursorEntityGUID = armyCursor.EntityGUID;
                        ArmyCursorEmpireIndex = SelectedEmpireIndex;
                    }

                    return;
                }
                // Select ArmyCursorEntityGUID when controlled by major empire.
                if (ArmyCursorEntityGUID != armyCursor.EntityGUID)
                {
                    ArmyCursorEntityGUID = armyCursor.EntityGUID;
                    SetPresentationArmy((ulong) armyCursor.EntityGUID);
                    ArmyCursorEmpireIndex = SelectedPresentationArmy.EmpireIndex;
                    // Set selection to ArmyCursor
                    SetSelectedEmpire(ArmyCursorEmpireIndex, ArmyCursorEntityGUID);
                }
            }
        }

        private void SetPresentationArmy(ulong entityGUID)
        {
            SelectedPresentationArmy = Presentation.PresentationEntityFactoryController.GetArmy(entityGUID);
        }

        public void SetSelectedEmpire(int selectedEmpireIndex, ulong selectedEntityGUID = 0)
        {
            SelectedEmpireIndex = selectedEmpireIndex;
            EmpireGrid.SelectedIndex = selectedEmpireIndex;
            UpdateArmies(-1, selectedEntityGUID);
        }

        public void UpdateArmies(int selectedArmyIndex, ulong selectedEntityGUID = 0)
        {
            Armies = GetArmiesOfEmpireAt(SelectedEmpireIndex);
            SelectedArmyIndex = selectedEntityGUID != 0 ? Array.FindIndex(Armies, army => army.EntityGUID == selectedEntityGUID) : selectedArmyIndex;
            ArmyGrid.Items = ArmiesAsGridItems(Armies.AsEnumerable()).ToArray();
            SelectedArmyEntityGUID = SelectedArmyIndex >= 0 ? Armies[SelectedArmyIndex].EntityGUID : 0;
            ArmyGrid.SelectedIndex = SelectedArmyIndex;
            MakeSelectedArmyVisibleInScrollView();
        }

        private void MakeSelectedArmyVisibleInScrollView()
        {
            var targetY = ArmyGrid.SelectedIndex * 25;
            var view = ArmyGrid.ScrollViewPosition;

            if (view.y <= targetY && targetY + 12 < view.y + ArmyGrid.Height)
                return;

            ArmyGrid.ScrollViewPosition = new Vector2(
                view.x,
                Math.Min(Math.Max(targetY - 25, 0), Math.Max((ArmyGrid.Items.Length * 25) - ArmyGrid.Height, 0))
            );
        }

        protected static Army[] GetArmiesOfEmpireAt(int empireIndex) => empireIndex < HumankindGame.Empires.Length ? 
            HumankindGame.Empires[empireIndex].Armies.ToArray() : HumankindGame.GetEmpireEntityAt(empireIndex).Armies;

        protected static GUIContent[] GetEmpiresGridItems() => HumankindGame.Empires.Select(e => 
                new GUIContent(StyledUI.GridItem(e))).ToArray();

        public static GUIContent[] ArmiesAsGridItems(IEnumerable<Army> sequence) => sequence.Select(
            (army, armyIndex) => new GUIContent(StyledUI.GridItem(army))).ToArray();

        // ACTIONS

        protected List<EndlessMovingArmy> MovingArmies = new List<EndlessMovingArmy>();

        public void SyncBackgroundTasks()
        {
            if (MovingArmies.Count() > 0)
                SyncMovingArmies();
        }

        private void SyncMovingArmies()
        {
            for (var i = MovingArmies.Count() - 1; i >= 0; i--)
            {
                var movingArmy = MovingArmies[i];

                if (ArmyUtils.ArmyMovementRatio(movingArmy.Army) <= 0.5f)
                    Amplitude.Mercury.Sandbox.SandboxManager.PostOrder(movingArmy.Order, movingArmy.EmpireIndex);
                else if (!ArmyUtils.IsRunning(movingArmy.Army))
                {
                    movingArmy.SkipOneTurn();
                    MovingArmies.Remove(movingArmy);
                }
            }
        }

        public void AddToEndlessMovingArmies(Army army)
        {
            if (!MovingArmies.Any(item => item.Army.EntityGUID == army.EntityGUID))
            {
                if (IsArmySelectedInGame(army) && TryGetArmyCursor(out ArmyCursor armyCursor))
                {
                    if (army.EntityGUID == armyCursor.EntityGUID && ArmyUtils.IsRunning(army))
                    {
                        if (armyCursor.SelectedUnitCount != army.Units.Length)
                            armyCursor.SelectAll();

                        EndlessMovingArmy movingArmy = new EndlessMovingArmy() {
                            Order = new OrderChangeMovementRatio(
                                armyCursor.EntityGUID, 
                                armyCursor.SelectedUnits.Select(guid => guid).ToArray(), 
                                1.0f
                            ),
                            EmpireIndex = (int) Snapshots.ArmyCursorSnapshot.PresentationData.EmpireIndex,
                            Army = army
                        };
                        MovingArmies.Add(movingArmy);
                    }
                }
            }
        }

        public bool IsArmySelectedInGame(Army army) => army.EntityGUID == ArmyCursorEntityGUID;

        public static bool TryGetArmyCursor(out ArmyCursor armyCursor) => (
            (armyCursor = Presentation.PresentationCursorController.CurrentCursor as ArmyCursor) != null 
            && Snapshots.ArmyCursorSnapshot != null);
    }

    public class EndlessMovingArmy
    {
        public OrderChangeMovementRatio Order { get; set; }
        public int EmpireIndex { get; set; }
        public Army Army { get; set; }
        public EndlessMovingArmy() {}

    }

    public static class EndlessMovingArmyEx
    {
        public static void SkipOneTurn(this EndlessMovingArmy self) => 
            Amplitude.Mercury.Sandbox.SandboxManager.PostOrder((Order) new OrderChangeEntityAwakeState() {
                EntityGuid = self.Order.UnitCollectionSimulationEntityGUID, AwakeState = AwakeState.SkipOneTurn
            }, self.EmpireIndex);
    }

    public static class ArmyUtils
    {
        public static string ArmyUnitNames(Army army) => string.Join(", ", army.Units.Select(UnitName)
            .GroupBy(name => name).Select(g => (g.Count() > 1 ? StyledUI.Tag.Hot(g.Count() + "x") : "") 
                + g.Key.ToUpper()).ToArray());

        public static string UnitName(Amplitude.Mercury.Interop.AI.Data.Unit unit) => (ArmyToolSettings.Instance.LocalizedTitles.Value ? 
            R.Text.GetLocalizedTitle(unit.UnitDefinition.Name) : unit.UnitDefinition.Name.ToString().Split('_').LastOrDefault());

        public static string ArmyTags(Army army) {
            List<string> res = new List<string>();

            if ((float) army.PathfindContext.MovementRatio >= 0.5f && army.BattleIndex < 0)
                res.Add(StyledUI.Tag.Success("READY"));

            if (army.State != Amplitude.Mercury.Simulation.ArmyState.Idle)
                res.Add(StyledUI.Tag.State(army.State.ToString()));

            if (army.AutoExplore) res.Add(StyledUI.Tag.Common("AUTO"));

            if (army.SpawnType != Amplitude.Mercury.Data.Simulation.UnitSpawnType.Land)
                res.Add(StyledUI.Tag.Class(army.SpawnType.ToString()));

            if (army.BattleIndex >= 0) res.Add(StyledUI.Tag.Warn("BATTLE"));

            return res.Count > 0 ? string.Join(StyledUI.Tag.Separator, res) : "";
        }

        private static string row(string name, string value) => 
            name.ToUpper() + ": " + R.Text.Color(value, "ACAC77FF");

        public static string SummaryOfSelectedArmy(Army army)
        {
            List<string> res;

            try {
                var data = Snapshots.ArmyCursorSnapshot.PresentationData;
                Amplitude.Mercury.Simulation.PathfindContext ctx = army.PathfindContext;
                res = new List<string>() {
                    "<size=7>\n</size>" + row("Name", data.EntityName.ToString()),
                    row("State", data.ArmyState.ToString() + " / " + data.AwakeState.ToString()), 
                    row("Movement", ArmyMovementPoints(army)),
                    row("Vision Range", ((int)data.VisionRange).ToString()),
                    "<size=5>\n\n</size>" + row("Detection Range", ((int)data.DetectionRange).ToString()),
                    row("Attack Range", ((int)data.DetectionRange).ToString()),
                    row("Combat Strength", ((float)data.DetectionRange).ToString()),
                    "<size=5>\n\n</size>" + row("Units", data.NumberOfUnits + " / " + data.ArmyMaximumSize),
                    row("Health", ((int)((float)data.HealthRatio * (float)data.HitPoints)).ToString() + " / " + ((int)data.HitPoints).ToString()),
                    row("Health Regen", "+" + ((int)((float)data.HealthRegen * (float)data.HitPoints)).ToString() + " / Turn"),
                    row("Upkeep", ((int)data.Upkeep).ToString()),
                    "<size=5>\n\n</size>" + row("AutoExplore", ((bool)army.AutoExplore).ToString()),
                    row("Mission", ((int)army.ArmyMissionIndex).ToString()),
                    row("GoToActionStatus", army.GoToActionStatus.ToString()),
                    row("IsInHealingArea", ((bool)army.IsInHealingArea).ToString()),
                    row("Flags", army.Flags.ToString()),
                    row("Flags2", data.Flags.ToString()),
                    row("IsTrespassing", ((bool)army.IsTrespassing).ToString()),
                    row("IsRetreating", ((bool)army.IsRetreating).ToString()),
                    row("IsLocked", ((bool)army.IsLocked).ToString()),
                    row("IsLocked2", ((bool)data.IsLocked).ToString()),
                    row("IsLockedByBattle", ((bool)data.IsLockedByBattle).ToString()),
                    row("HasGoToAction", ((bool)data.HasGoToAction).ToString()),
                    row("IsNomadic", ((bool)army.IsNomadic).ToString()),
                    row("IsMercenary", ((bool)army.IsMercenary).ToString()),
                    row("AwakeState", data.AwakeState.ToString()),
                    row("ArmyActions", string.Join(", ", data.ArmyActions?.Select(a => a.ArmyAction.ToString()))),
                    "\n",
                };
            }
            catch (Exception) {
                return "";
            }

            return string.Join(StyledUI.Tag.Separator, res);
        }

        public static string ArmyMovementPoints(Army army) =>
            "" + ((int)(ArmyMovementRatio(army) * army.MovementSpeed)) + " / " + ((int)army.MovementSpeed);

        public static float ArmyMovementRatio(Army army) => (float) army.PathfindContext.MovementRatio;

        public static bool IsRunning(Army army) => army.GoToActionStatus == Army.ActionStatus.Running;
    }
    
    public abstract class CustomSetting<T>
    {
        public T Value { get; set; }
        public string Name;
        public string Tooltip;
        public GUIContent Content;

        public CustomSetting(string name, string tooltip, T defaultValue)
        {
            Name = R.Text.Bold(R.Text.Size(name, 11));
            Tooltip = tooltip;
            Value = defaultValue;
            Content = new GUIContent(Name, Tooltip);
        }

        public abstract bool Draw();
    }

    public class BooleanSetting : CustomSetting<bool>
    {
        public BooleanSetting(string name, string tooltip, bool defaultValue) : base(name, tooltip, defaultValue) {}

        public override bool Draw() => (Value = GUILayout.Toggle(Value, Content));
    }

   

     public static class StyledUI
    {
        public static string GridItem(HumankindEmpire e) => R.Text.Color(
            R.Text.Bold(e.EmpireIndex.ToString() + ". " + e.PersonaName.ToUpper()), e.PrimaryColor);

        public static string GridItem(Army army) => Tag.Badge(army.Units.Length.ToString()) + Tag.Separator +
            Tag.ListItem(ArmyUtils.ArmyUnitNames(army)) + Tag.Separator + 
            Tag.BarelyVisible(army.EntityGUID.ToString()) + Tag.Separator + ArmyUtils.ArmyTags(army);

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

    public class ArmyToolSettings
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

        public static ArmyToolSettings Instance;

        public ArmyToolSettings() {
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
