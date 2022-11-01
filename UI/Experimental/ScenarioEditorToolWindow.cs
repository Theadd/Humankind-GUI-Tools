using System;
using Amplitude.Framework;
using Amplitude.Framework.Runtime;
using Amplitude.Mercury.Data.Scenario;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Overlay.Scenario;
using Amplitude.Mercury.Presentation;
using Amplitude.Mercury.Runtime;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using UnityEngine;
using IRuntimeService = Amplitude.Framework.Runtime.IRuntimeService;
// ReSharper disable BitwiseOperatorOnEnumWithoutFlags

namespace DevTools.Humankind.GUITools.UI
{
  public class ScenarioEditorToolWindow : FloatingToolWindow
  {
    public override string WindowTitle { get; set; } = " Scenario Editor Window";

    public override string WindowGUIStyle { get; set; } = "PopupWindow";

    public override bool ShouldBeVisible => true; // !GlobalSettings.ShouldHideTools;

    public override bool ShouldRestoreLastWindowPosition => true;

    public override Rect WindowRect { get; set; } = new Rect(130f, 260f, 720f, 500f);

    private Color _bgColor = new Color32(255, 255, 255, 230);
    private Color _bgColorOpaque = new Color32(255, 255, 255, 255);

    public override void OnGUIStyling()
    {
      base.OnGUIStyling();
      GUI.backgroundColor = GlobalSettings.WindowTransparency.Value ? _bgColor : _bgColorOpaque;
    }

    public override void OnDrawUI()
    {
      if (GlobalSettings.WindowTitleBar.Value)
        WindowUtils.DrawWindowTitleBar(this);

      OnDrawWindowClientArea(0);
    }

    private bool _isIngameStarted;
    private GameScenarioDefinition[] _scenarios;
    private EditorTab[] _editroTabs = new EditorTab[]
    {
      new ScenarioEditorTab(),
      new EmpireEditorTab(),
      new SettlementEditorTab(),
      new MilitaryEditorTab(),
      new DiplomacyEditorTab(),
      new TechnologyEditorTab(),
      new CivicsEditorTab(),
      new MinorFactionEditorTab(),
      new CuriosityEditorTab(),
      new PathCuriosityEditorTab(),
      new VisionEditorTab()
    };
    private string[] _editroTabsNames;
    private int _currentTabIndex = -1;
    private string[] _majorEmpireNames = new string[0];
    private int[] _minorEmpireIndexes = new int[0];
    private string[] _minorEmpireNames = new string[0];
    private int _lesserEmpireIndex = -1;
    private int[] _animalGroupIndexes = new int[0];
    private string[] _animalGroupNames = new string[0];
    private EmpireTypeFlags _currentEmpireType = EmpireTypeFlags.NoEmpire;
    private int _currentEmpireIndex = -1;
    private int _currentAnimalGroupIndex = -1;
    private Vector2 _scenarioDefinitionScrollPosition;
    private Vector2 _toolbarScrollPosition;

    //Play CustomMap
    private int empireCount = 2;
    private string mapName = "Default";
    public ScenarioEditorToolWindow()
    {
      _editroTabsNames = new string[_editroTabs.Length];
      for (int index = 0; index < _editroTabs.Length; ++index)
        _editroTabsNames[index] = _editroTabs[index].Name;
    }

    protected IRuntimeService RuntimeService { get; private set; }

    protected override void OnBecomeInvisible()
    {
      if (_isIngameStarted)
        StopIngame();
      base.OnBecomeInvisible();
    }

    protected override void OnDrawWindowClientArea(int instanceId)
    {
      if (!IsVisible)
        return;
      if (RuntimeService == null)
      {
        using (new GUILayout.VerticalScope("Widget.ClientArea", Array.Empty<GUILayoutOption>()))
          GUILayout.Label("Waiting for the runtime service...");
        if (Event.current.type != EventType.Repaint)
          return;
        RuntimeService = Services.GetService<IRuntimeService>();
        RuntimeService.RuntimeChange -= OnRuntimeChange;
        RuntimeService.RuntimeChange += OnRuntimeChange;
      }
      else if (RuntimeService.Runtime == null || !RuntimeService.Runtime.HasBeenLoaded || RuntimeService.Runtime.FiniteStateMachine.CurrentState == null)
      {
        using (new GUILayout.VerticalScope("Widget.ClientArea", Array.Empty<GUILayoutOption>()))
          GUILayout.Label("Waiting for the runtime...");
      }
      else
      {
        Type type = RuntimeService.Runtime.FiniteStateMachine.CurrentState.GetType();
        if (type == typeof (RuntimeState_InGame))
        {
          DrawInGame();
        }
        else
        {
          if (_isIngameStarted)
            StopIngame();
          if (type == typeof (RuntimeState_OutGame))
          {
            DrawOutGame();
          }
          else
          {
            using (new GUILayout.VerticalScope("Widget.ClientArea", Array.Empty<GUILayoutOption>()))
              GUILayout.Label("Not available in runtime state '" + type.Name + "'");
          }
        }
      }
    }

    private void DrawOutGame()
    {
      using (new GUILayout.VerticalScope("Widget.ClientArea", GUILayout.Height(580f)))
      {
        // Draw CustomMap UI
        DrawOutGamePlayCustomMap();
        GUILayout.Space(5f);
        
        // Draw ScenarioManagement UI
        GUILayout.Label("Scenarios :");
        GUILayout.Space(5f);
        if (_scenarios == null)
        {
          IDatabase<GameScenarioDefinition> database = Databases.GetDatabase<GameScenarioDefinition>();
          _scenarios = database == null ? new GameScenarioDefinition[0] : database.GetValues();
        }
        _scenarioDefinitionScrollPosition = GUILayout.BeginScrollView(_scenarioDefinitionScrollPosition);
        for (int index = 0; index < _scenarios.Length; ++index)
        {
          GameScenarioDefinition scenario = _scenarios[index];
          GUILayout.BeginHorizontal();
          GUILayout.Label(scenario.name);
          GUILayout.FlexibleSpace();
          GUI.enabled = scenario.IsSaveFileValid;
          if (GUILayout.Button("Play", GUILayout.Width(100f)))
            RuntimeService.Runtime.FiniteStateMachine.PostStateChange(typeof (RuntimeState_Staging), new RuntimeStateScenarioParameter(scenario.name, false));
          GUI.enabled = scenario.AreCreationSettingsValid;
          if (GUILayout.Button("Edit", GUILayout.Width(100f)))
            RuntimeService.Runtime.FiniteStateMachine.PostStateChange(typeof (RuntimeState_Staging), new RuntimeStateScenarioParameter(scenario.name, true));
          GUI.enabled = scenario.AreCreationSettingsValid;
          if (GUILayout.Button("Edit Map", GUILayout.Width(100f)))
            RuntimeService.Runtime.FiniteStateMachine.PostStateChange(typeof (RuntimeState_MapEditor), new RuntimeStateMapEditorParameter(scenario.TerrainGuid, null, true));
          GUI.enabled = true;
          GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
      }
      GUI.enabled = true;
    }
    
    protected void DrawOutGamePlayCustomMap()
    {
      GUILayout.BeginHorizontal();
      GUILayout.Label(string.Format("EmpireCount {0}", (object) this.empireCount));
      if (GUILayout.Button("-", GUILayout.MaxWidth(32f)))
        this.empireCount = System.Math.Max(this.empireCount - 1, 2);
      if (GUILayout.Button("+", GUILayout.MaxWidth(32f)))
        this.empireCount = System.Math.Min(this.empireCount + 1, 8);
      GUILayout.EndHorizontal();
      this.mapName = GUILayout.TextField(this.mapName);
      if (!GUILayout.Button("Play"))
        return;
      Amplitude.Framework.Runtime.IRuntimeService service = Services.GetService<Amplitude.Framework.Runtime.IRuntimeService>();
      if (service == null || !(service.Runtime.FiniteStateMachine.CurrentState.GetType() == typeof (RuntimeState_OutGame)))
        return;
      RuntimeStateParameter runtimeStateParameter = (RuntimeStateParameter) new RuntimeStateCustomMapParameter(this.mapName, this.empireCount);
      service.Runtime.FiniteStateMachine.PostStateChange(typeof (RuntimeState_Staging), (object) runtimeStateParameter);
    }

    private void DrawInGame()
    {
      if (!Presentation.HasBeenStarted)
      {
        using (new GUILayout.VerticalScope("Widget.ClientArea", Array.Empty<GUILayoutOption>()))
          GUILayout.Label("Waiting for the presentation...");
      }
      else
      {
        string sandboxStateName = Snapshots.SandboxSnapshot.PresentationData.CurrentSandboxStateName;
        if (sandboxStateName == "SandboxState_Bootstrapper" || sandboxStateName == "SandboxState_BeginScenarioEdition")
        {
          using (new GUILayout.VerticalScope("Widget.ClientArea", Array.Empty<GUILayoutOption>()))
            GUILayout.Label("Waiting for sandbox initialization...");
        }
        else if (Snapshots.SandboxSnapshot == null || Snapshots.GameSnapshot == null || Snapshots.ScenarioSnapshot == null)
        {
          using (new GUILayout.VerticalScope("Widget.ClientArea", Array.Empty<GUILayoutOption>()))
            GUILayout.Label("Waiting for snapshots...");
        }
        else
        {
          if (!_isIngameStarted)
            StartIngame();
          if (Snapshots.ScenarioEditionSnapshot.PresentationData.Frame == -1)
          {
            GUILayout.Label("Waiting for snapshots...");
          }
          else
          {
            bool wordWrap = GUI.skin.label.wordWrap;
            GUI.skin.label.wordWrap = false;
            using (new GUILayout.VerticalScope("Widget.ClientArea", GUILayout.Height(600f)))
            {
              GUILayout.Label("Scenario : " + Snapshots.ScenarioSnapshot.PresentationData.ScenarioDefinitionName);
              GUILayout.Space(5f);
              _toolbarScrollPosition = GUILayout.BeginScrollView(_toolbarScrollPosition, GUILayout.Height(48f));
              int newTabIndex = GUILayout.Toolbar(_currentTabIndex, _editroTabsNames, "PopupWindow.ToolbarButton", GUILayout.Width(_editroTabsNames.Length * 94));
              GUILayout.EndScrollView();
              if (newTabIndex != _currentTabIndex)
                SetNewSelectedEditorTab(newTabIndex);
              GUILayout.Space(5f);
              DrawEmpireSelection();
              GUILayout.Space(5f);
              EditorTab editroTab = _editroTabs[_currentTabIndex];
              TextAnchor alignment = GUI.skin.label.alignment;
              GUI.skin.label.alignment = TextAnchor.MiddleCenter;
              GUILayout.Label("------------------------------------------------ " + editroTab.Name + " ------------------------------------------------");
              GUI.skin.label.alignment = alignment;
              if ((editroTab.AllowedEmpireTypeFlags & EmpireTypeFlags.NoEmpire) == EmpireTypeFlags.None && _currentEmpireIndex < 0)
                GUILayout.Label("No empire selected.");
              else
                editroTab.Draw();
            }
            GUI.skin.label.wordWrap = wordWrap;
            GUI.enabled = true;
          }
        }
      }
    }

    private void StartIngame()
    {
      Snapshots.ScenarioEditionSnapshot.Start(ScenarioEditionSnapshot.ActivationFlags.ScenarioEditorWindow);
      ClearEmpireInfo();
      SetNewSelectedEditorTab(0);
      _isIngameStarted = true;
    }

    private void StopIngame()
    {
      if (_currentTabIndex >= 0)
      {
        _editroTabs[_currentTabIndex].Stop();
        _currentTabIndex = -1;
      }
      ClearEmpireInfo();
      if (Snapshots.ScenarioEditionSnapshot != null)
        Snapshots.ScenarioEditionSnapshot.Stop(ScenarioEditionSnapshot.ActivationFlags.ScenarioEditorWindow);
      _isIngameStarted = false;
    }

    private void SetNewSelectedEditorTab(int newTabIndex)
    {
      if (_currentTabIndex >= 0)
        _editroTabs[_currentTabIndex].Stop();
      EditorTab editroTab = _editroTabs[newTabIndex];
      UpdateCurrentEmpireIndexFor(editroTab);
      editroTab.SetEmpireIndex(_currentEmpireType, _currentEmpireIndex, _currentAnimalGroupIndex);
      editroTab.Start();
      _currentTabIndex = newTabIndex;
    }

    private void DrawEmpireSelection()
    {
      EditorTab editroTab = _editroTabs[_currentTabIndex];
      UpdateEmpireIndexes();
      int allowedEmpireTypeFlags = (int) editroTab.AllowedEmpireTypeFlags;
      bool flag1 = (uint) (allowedEmpireTypeFlags & 2) > 0U;
      bool flag2 = (uint) (allowedEmpireTypeFlags & 4) > 0U;
      bool flag3 = (uint) (allowedEmpireTypeFlags & 8) > 0U;
      if (flag1)
      {
        using (new GUILayout.HorizontalScope(GUILayout.ExpandWidth(false)))
        {
          GUILayout.Label("Major Empires:", GUILayout.ExpandWidth(false));
          GUILayout.Space(5f);
          int selected = _currentEmpireIndex < _majorEmpireNames.Length ? _currentEmpireIndex : -1;
          int num = GUILayout.Toolbar(selected, _majorEmpireNames, "PopupWindow.ToolbarButton", GUILayout.ExpandWidth(false));
          if (num != selected)
          {
            _currentEmpireType = EmpireTypeFlags.MajorEmpire;
            _currentEmpireIndex = num;
            _currentAnimalGroupIndex = -1;
            editroTab.SetEmpireIndex(_currentEmpireType, _currentEmpireIndex, _currentAnimalGroupIndex);
            FocusCameraOnEmpire(_currentEmpireIndex);
          }
        }
      }
      if (flag2)
      {
        using (new GUILayout.HorizontalScope(GUILayout.ExpandWidth(false)))
        {
          GUILayout.Label("Minor Empires:", GUILayout.ExpandWidth(false));
          GUILayout.Space(5f);
          if (_minorEmpireIndexes.Length != 0)
          {
            int selected = Array.IndexOf(_minorEmpireIndexes, _currentEmpireIndex);
            int index = GUILayout.Toolbar(selected, _minorEmpireNames, "PopupWindow.ToolbarButton", GUILayout.ExpandWidth(false));
            if (index != selected)
            {
              _currentEmpireType = EmpireTypeFlags.MinorEmpire;
              _currentEmpireIndex = _minorEmpireIndexes[index];
              _currentAnimalGroupIndex = -1;
              editroTab.SetEmpireIndex(_currentEmpireType, _currentEmpireIndex, _currentAnimalGroupIndex);
              FocusCameraOnEmpire(_currentEmpireIndex);
            }
          }
          else
            GUILayout.Label("(No minor empires spawned)", GUILayout.ExpandWidth(false));
        }
      }
      if (!flag3)
        return;
      using (new GUILayout.HorizontalScope(GUILayout.ExpandWidth(false)))
      {
        GUILayout.Label("Animal group:", GUILayout.ExpandWidth(false));
        GUILayout.Space(5f);
        if (_animalGroupNames.Length != 0)
        {
          int selected = Array.IndexOf(_animalGroupIndexes, _currentAnimalGroupIndex);
          int num = GUILayout.Toolbar(selected, _animalGroupNames, "PopupWindow.ToolbarButton", GUILayout.ExpandWidth(false));
          if (num == selected)
            return;
          _currentEmpireType = EmpireTypeFlags.LesserEmpire;
          _currentEmpireIndex = _lesserEmpireIndex;
          _currentAnimalGroupIndex = num;
          editroTab.SetEmpireIndex(_currentEmpireType, _currentEmpireIndex, _currentAnimalGroupIndex);
          FocusCameraOnAnimalGroup(_currentAnimalGroupIndex);
        }
        else
          GUILayout.Label("(No animal group spawned)", GUILayout.ExpandWidth(false));
      }
    }

    private void UpdateEmpireIndexes()
    {
      ScenarioEditionSnapshot.Data presentationData = Snapshots.ScenarioEditionSnapshot.PresentationData;
      bool flag = false;
      if (_majorEmpireNames == null || _majorEmpireNames.Length != presentationData.NumberOfMajorEmpires)
      {
        _majorEmpireNames = new string[presentationData.NumberOfMajorEmpires];
        for (int index = 0; index < presentationData.NumberOfMajorEmpires; ++index)
          _majorEmpireNames[index] = index.ToString();
        flag = true;
      }
      int count1 = presentationData.MinorEmpireIndexes.Count;
      if (_minorEmpireIndexes == null || _minorEmpireIndexes.Length != count1)
      {
        _minorEmpireIndexes = new int[count1];
        _minorEmpireNames = new string[count1];
        for (int index = 0; index < count1; ++index)
        {
          int minorEmpireIndex = presentationData.MinorEmpireIndexes[index];
          _minorEmpireIndexes[index] = minorEmpireIndex;
          _minorEmpireNames[index] = minorEmpireIndex.ToString();
        }
        flag = true;
      }
      int count2 = presentationData.AnimalGroupIndexes.Count;
      if (_animalGroupIndexes == null || _animalGroupIndexes.Length != count2)
      {
        _lesserEmpireIndex = _lesserEmpireIndex = presentationData.LesserEmpireIndex;
        _animalGroupIndexes = new int[count2];
        _animalGroupNames = new string[count2];
        for (int index = 0; index < count2; ++index)
        {
          int animalGroupIndex = presentationData.AnimalGroupIndexes[index];
          _animalGroupIndexes[index] = animalGroupIndex;
          _animalGroupNames[index] = animalGroupIndex.ToString();
        }
        flag = true;
      }
      if (!flag)
        return;
      EditorTab editroTab = _editroTabs[_currentTabIndex];
      UpdateCurrentEmpireIndexFor(editroTab);
      editroTab.SetEmpireIndex(_currentEmpireType, _currentEmpireIndex, _currentAnimalGroupIndex);
    }

    private void UpdateCurrentEmpireIndexFor(EditorTab editorTab)
    {
      EmpireTypeFlags allowedEmpireTypeFlags = editorTab.AllowedEmpireTypeFlags;
      if ((allowedEmpireTypeFlags & EmpireTypeFlags.AnyEmpire) != EmpireTypeFlags.None)
      {
        bool flag1 = (uint) (allowedEmpireTypeFlags & EmpireTypeFlags.MajorEmpire) > 0U;
        bool flag2 = (uint) (allowedEmpireTypeFlags & EmpireTypeFlags.MinorEmpire) > 0U;
        bool flag3 = (uint) (allowedEmpireTypeFlags & EmpireTypeFlags.LesserEmpire) > 0U;
        bool flag4 = false;
        if (_currentEmpireIndex >= 0)
        {
          if (_currentEmpireIndex < _majorEmpireNames.Length)
            flag4 = flag1;
          else if (Array.IndexOf(_minorEmpireIndexes, _currentEmpireIndex) >= 0)
            flag4 = flag2;
          else if (_currentEmpireIndex == _lesserEmpireIndex && Array.IndexOf(_animalGroupIndexes, _currentAnimalGroupIndex) >= 0)
            flag4 = flag3;
        }
        if (flag4)
          return;
        if (flag1)
        {
          _currentEmpireType = EmpireTypeFlags.MajorEmpire;
          _currentEmpireIndex = 0;
          _currentAnimalGroupIndex = -1;
        }
        else if (flag2 && _minorEmpireIndexes.Length != 0)
        {
          _currentEmpireType = EmpireTypeFlags.MinorEmpire;
          _currentEmpireIndex = _minorEmpireIndexes[0];
          _currentAnimalGroupIndex = -1;
        }
        else if (flag3 && _animalGroupIndexes.Length != 0)
        {
          _currentEmpireType = EmpireTypeFlags.LesserEmpire;
          _currentEmpireIndex = _lesserEmpireIndex;
          _currentAnimalGroupIndex = _animalGroupIndexes[0];
        }
        else
        {
          _currentEmpireType = EmpireTypeFlags.NoEmpire;
          _currentEmpireIndex = -1;
          _currentAnimalGroupIndex = -1;
        }
      }
      else
      {
        _currentEmpireType = EmpireTypeFlags.NoEmpire;
        _currentEmpireIndex = -1;
        _currentAnimalGroupIndex = -1;
      }
    }

    private void ClearEmpireInfo()
    {
      _majorEmpireNames = new string[0];
      _minorEmpireIndexes = new int[0];
      _minorEmpireNames = new string[0];
      _lesserEmpireIndex = -1;
      _animalGroupIndexes = new int[0];
      _animalGroupNames = new string[0];
      _currentEmpireType = EmpireTypeFlags.NoEmpire;
      _currentEmpireIndex = -1;
      _currentAnimalGroupIndex = -1;
    }

    private void FocusCameraOnEmpire(int empireIndex)
    {
      int tileIndex = Snapshots.GameSnapshot.PresentationData.EmpireInfo[empireIndex].TileIndex;
      if (tileIndex < 0)
        return;
      Presentation.PresentationCameraController.CenterCameraAt(tileIndex);
    }

    private void FocusCameraOnAnimalGroup(int animalGroupindex)
    {
      int tileIndex = Snapshots.ScenarioEditionSnapshot.PresentationData.AnimalGroupInfo.Data[animalGroupindex].TileIndex;
      if (tileIndex < 0)
        return;
      Presentation.PresentationCameraController.CenterCameraAt(tileIndex);
    }

    private void OnRuntimeChange(object sender, RuntimeChangeEventArgs e) => _scenarios = null;
  }
}
