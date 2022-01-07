using Amplitude.Framework;
using Amplitude.Framework.Overlay;
using Amplitude.Mercury.Data.Simulation;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Presentation;
using Amplitude.Mercury.Runtime;
using Amplitude.Mercury.Sandbox;
using Amplitude.Mercury.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.Core;
using UnityEngine;
using HarmonyLib;
using SharedAssets = DevTools.Humankind.SharedAssets.SharedAssets;

namespace DevTools.Humankind.GUITools.UI
{
    public class MilitaryToolsWindow : FloatingToolWindow
    {
        private string[] unitDefinitions = new string[0];
        private string[] unitDefinitionsFiltered = new string[0];
        private List<UnitDefinition> unitList = new List<UnitDefinition>();
        private float damageAmount = 0.1f;
        private float movementRatio = 1f;
        private int experienceAmount = 10;
        private Vector2 unitDefinitionPosition = Vector2.zero;
        private int selectedUnit = -1;
        private bool sendToGodCursor;
        private string unitNameFilter = string.Empty;
        private bool transformToNavalTransport;
        private PresentationArmy selectedArmy;
        private PresentationSquadron selectedSquadron;

        protected Amplitude.Framework.Runtime.IRuntimeService RuntimeService { get; private set; }

        public override string WindowTitle { get; set; } = "MILITARY TOOLS";

        public override string WindowGUIStyle { get; set; } = "PopupWindow";

        public override bool ShouldBeVisible => HumankindGame.IsGameLoaded;

        public override bool ShouldRestoreLastWindowPosition => true;

        public override Rect WindowRect { get; set; } = new Rect(130f, 260f, 380f, 500f);

        public static void SetGodMode(bool enabled) => AccessTools.PropertySetter(typeof(GodMode), "Enabled")?.Invoke(null, new object[] { enabled });

        public override void OnGUIStyling()
        {
            base.OnGUIStyling();
            GUI.backgroundColor = new Color32(255, 255, 255, 230);
        }

        public override void OnDrawUI()
        {
            WindowUtils.DrawWindowTitleBar(this);

            OnDrawWindowContent();
        }

        private Texture selectedUnitTexture = Utils.TransparentTexture;
        private String selectedUnitLocalizedName = string.Empty;

        protected void OnDrawWindowContent()
        {
            GUILayout.BeginVertical((GUIStyle)"Widget.ClientArea", GUILayout.ExpandWidth(true));

            if (Amplitude.Mercury.Presentation.Presentation.HasBeenStarted)
            {
                if (this.unitDefinitions == null || this.unitDefinitions.Length == 0)
                {
                    IDatabase<ConstructibleDefinition> database =
                        Databases.GetDatabase<ConstructibleDefinition>();
                    if (database == null)
                    {
                        GUILayout.Label("Waiting for the constructible database to be loaded...");
                    }
                    else
                    {
                        ConstructibleDefinition[] values = database.GetValues();
                        int length = values.Length;
                        for (int index = 0; index < length; ++index)
                        {
                            UnitDefinition unitDefinition = values[index] as UnitDefinition;

                            if (!((UnityEngine.Object)unitDefinition == (UnityEngine.Object)null) &&
                                !(unitDefinition is NavalTransportDefinition))
                                unitList.Add(unitDefinition);
                        }

                        int count = this.unitList.Count;
                        unitDefinitions = new string[count];
                        for (int index = 0; index < count; ++index){
                            unitDefinitions[index] = unitList[index].name;
                        }
                        Array.Sort<string>(this.unitDefinitions,
                            new Comparison<string>(MilitaryToolsWindow
                                .CompareUnitDefinitions));
                    }

                    this.unitDefinitionsFiltered =
                        ((IEnumerable<string>)this.unitDefinitions).ToArray<string>();

                }

                string unitNameFilter1 = this.unitNameFilter;
                this.unitNameFilter = GUILayout.TextField(this.unitNameFilter).ToLower();
                string unitNameFilter2 = this.unitNameFilter;
                if (unitNameFilter1 != unitNameFilter2)
                {
                    string str = this.selectedUnit >= 0
                        ? unitDefinitions[this.selectedUnit]
                        : string.Empty;
                    unitDefinitionsFiltered = !string.IsNullOrEmpty(unitNameFilter)
                        ? (unitDefinitions)
                        .Where((name =>
                           name.ToLower().Contains(unitNameFilter))).ToArray()
                        : (unitDefinitions).ToArray();
                    this.selectedUnit = -1;
                    for (int index = 0; index < unitDefinitionsFiltered.Length; ++index)
                    {
                        if (unitDefinitionsFiltered[index] == str)
                        {
                            this.selectedUnit = index;
                            break;
                        }
                    }
                }

                int lastSelectedUnit = this.selectedUnit;
                this.unitDefinitionPosition = GUILayout.BeginScrollView(this.unitDefinitionPosition,
                    GUILayout.Height(140f));
                this.selectedUnit =
                    GUILayout.SelectionGrid(this.selectedUnit, this.unitDefinitionsFiltered, 1);
                GUILayout.EndScrollView();
                bool flag = lastSelectedUnit != this.selectedUnit && this.selectedUnit >= 0 &&
                            this.selectedUnit < this.unitDefinitionsFiltered.Length;
                string selectedUnitName = selectedUnit >= 0 && selectedUnit < unitDefinitionsFiltered.Length ? this.unitDefinitionsFiltered[this.selectedUnit] : "";

                this.sendToGodCursor &= GodMode.Enabled;
                if (this.sendToGodCursor)
                    this.sendToGodCursor &=
                        Amplitude.Mercury.Presentation.Presentation.PresentationCursorController
                            .CurrentCursor is DefaultCursor;
                bool sendToGodCursor = this.sendToGodCursor;

                GUILayout.Space(8f);
                GUILayout.BeginHorizontal();
                    GUILayout.Space(8f);

                    if (flag)
                    {
                        var sprite = Utils.LoadUnitSprite(selectedUnitName);
                        selectedUnitTexture = sprite == null ? Texture2D.blackTexture : sprite.texture;
                        selectedUnitLocalizedName = R.Text.GetLocalizedTitle(new Amplitude.StaticString(selectedUnitName));

                        Loggr.Log(sprite);
                    }
                    GUILayout.BeginVertical("Checkbox", GUILayout.Width(64f), GUILayout.Height(64f));
                        GUI.DrawTexture(GUILayoutUtility.GetRect(64f, 64f),
                            selectedUnitTexture, ScaleMode.StretchToFill, true, 1f, new Color(1f, 1f, 1f, 0.9f), 0,0);
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical(GUILayout.Height(62f));
                        GUILayout.BeginHorizontal();
                            GUILayout.Label(selectedUnitLocalizedName);
                            GUILayout.FlexibleSpace();
                            this.sendToGodCursor = GUILayout.Toggle(this.sendToGodCursor, "Send to God Cursor") | flag;
                        GUILayout.EndHorizontal();
                        GUILayout.FlexibleSpace();
                        GUILayout.TextField(selectedUnitName);
                    GUILayout.EndVertical();
                    
                    GUILayout.Space(8f);
                GUILayout.EndHorizontal();
                GUILayout.Space(4f);
                GUILayout.BeginHorizontal();

                
                
                if (this.sendToGodCursor != sendToGodCursor | flag)
                {
                    if (this.sendToGodCursor)
                    {
                        if (this.selectedUnit >= 0 &&
                            this.selectedUnit < this.unitDefinitionsFiltered.Length)
                        {
                            SetGodMode(true);
                            Amplitude.Mercury.Presentation.Presentation.PresentationCursorController
                                .ChangeToDefaultCursor(
                                    new Amplitude.StaticString(this.unitDefinitionsFiltered[this.selectedUnit]));
                        }
                    }
                    else
                    {
                        Amplitude.Mercury.Presentation.Presentation.PresentationCursorController
                            .ChangeToDefaultCursor();
                        SetGodMode(false);
                    }
                }

                
                

                GUILayout.EndHorizontal();


                GUILayout.Space(20f);
                GUILayout.Label("ARMY TOOLS");
                if (Snapshots.ArmyCursorSnapshot != null)
                {
                    if (Amplitude.Mercury.Presentation.Presentation.PresentationCursorController
                        .CurrentCursor is ArmyCursor currentCursor15)
                    {
                        if ((UnityEngine.Object)this.selectedArmy == (UnityEngine.Object)null ||
                            currentCursor15.EntityGUID !=
                            (SimulationEntityGUID)this.selectedArmy.SimulationEntityGuid)
                            this.selectedArmy =
                                Amplitude.Mercury.Presentation.Presentation
                                    .PresentationEntityFactoryController
                                    .GetArmy((ulong)currentCursor15.EntityGUID);
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(
                            string.Format("Army selected : {0}", (object)this.selectedArmy));
                        if (GUILayout.Button("Disband Unit"))
                        {
                            if (currentCursor15.SelectedUnitCount <= 0)
                                currentCursor15.SelectAll();
                            SimulationEntityGUID[] unitGUIDsToDisband =
                                new SimulationEntityGUID[currentCursor15.SelectedUnitCount];
                            Array.Copy((Array)currentCursor15.SelectedUnits,
                                (Array)unitGUIDsToDisband, currentCursor15.SelectedUnitCount);
                            SandboxManager.PostOrder(
                                (Order)new OrderDisbandUnits(currentCursor15.EntityGUID,
                                    unitGUIDsToDisband),
                                (int)Snapshots.ArmyCursorSnapshot.PresentationData.EmpireIndex);
                        }

                        GUILayout.EndHorizontal();
                        GUILayout.Space(5f);
                        GUILayout.BeginHorizontal();
                        GUILayout.BeginVertical();
                        GUILayout.Label("Army Awake State");
                        GUILayout.FlexibleSpace();
                        int awakeState = (int)Snapshots.ArmyCursorSnapshot.PresentationData.AwakeState;
                        int num = GUILayout.SelectionGrid(awakeState,
                            System.Enum.GetNames(typeof(AwakeState)), 3, GUILayout.ExpandWidth(true));
                        if (num != awakeState)
                            SandboxManager.PostOrder((Order)new OrderChangeEntityAwakeState()
                            {
                                EntityGuid = Snapshots.ArmyCursorSnapshot.PresentationData.EntityGUID,
                                AwakeState = (AwakeState)num
                            }, (int)Snapshots.ArmyCursorSnapshot.PresentationData.EmpireIndex);
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();
                        GUILayout.Space(5f);
                        GUILayout.BeginHorizontal();
                        this.damageAmount = GUILayout.HorizontalSlider(this.damageAmount, 0.1f, 0.9f);
                        if (GUILayout.Button("Damage Unit"))
                        {
                            if (currentCursor15.SelectedUnitCount <= 0)
                                currentCursor15.SelectAll();
                            SimulationEntityGUID[] unitGUIDsToDisband =
                                new SimulationEntityGUID[currentCursor15.SelectedUnitCount];
                            Array.Copy((Array)currentCursor15.SelectedUnits,
                                (Array)unitGUIDsToDisband, currentCursor15.SelectedUnitCount);
                            PostOrderTicket postOrderTicket = SandboxManager.PostAndTrackOrder(
                                (Order)new OrderDamageUnits(currentCursor15.EntityGUID,
                                    unitGUIDsToDisband, this.damageAmount),
                                (int)Snapshots.ArmyCursorSnapshot.PresentationData.EmpireIndex);
                            if ((UnityEngine.Object)this.selectedArmy != (UnityEngine.Object)null)
                                postOrderTicket.UponCompletion((System.Action)(() =>
                                   this.selectedArmy.DoUpdateMesh(false, false)));
                        }

                        GUILayout.EndHorizontal();
                        GUILayout.Space(5f);
                        GUILayout.BeginHorizontal();
                        int result;
                        if (int.TryParse(GUILayout.TextField(this.experienceAmount.ToString()),
                            out result))
                            this.experienceAmount = result;
                        if (GUILayout.Button("Add Experience to Unit(s)"))
                        {
                            if (currentCursor15.SelectedUnitCount <= 0)
                                currentCursor15.SelectAll();
                            int[] unitsExperiencePoints = new int[currentCursor15.SelectedUnits.Length];
                            for (int index = 0; index < currentCursor15.SelectedUnits.Length; ++index)
                                unitsExperiencePoints[index] = this.experienceAmount;
                            SimulationEntityGUID[] unitGUIDs =
                                new SimulationEntityGUID[currentCursor15.SelectedUnitCount];
                            Array.Copy((Array)currentCursor15.SelectedUnits, (Array)unitGUIDs,
                                currentCursor15.SelectedUnitCount);
                            SandboxManager.PostOrder(
                                (Order)new OrderChangeUnitsXP(currentCursor15.EntityGUID, unitGUIDs,
                                    unitsExperiencePoints),
                                (int)Snapshots.ArmyCursorSnapshot.PresentationData.EmpireIndex);
                        }

                        GUILayout.EndHorizontal();
                        GUILayout.Space(5f);
                        GUILayout.BeginHorizontal();
                        this.movementRatio = GUILayout.HorizontalSlider(this.movementRatio, 0.0f, 1f);
                        if (GUILayout.Button("Change Movement Ratio"))
                        {
                            SimulationEntityGUID[] unitGUIDs;
                            if (currentCursor15.SelectedUnitCount > 0)
                            {
                                unitGUIDs = new SimulationEntityGUID[currentCursor15.SelectedUnitCount];
                                Array.Copy((Array)currentCursor15.SelectedUnits, (Array)unitGUIDs,
                                    currentCursor15.SelectedUnitCount);
                            }
                            else
                            {
                                UnitInfo[] unitInfo = Snapshots.ArmyCursorSnapshot.PresentationData
                                    .UnitInfo;
                                unitGUIDs = new SimulationEntityGUID[unitInfo.Length];
                                for (int index = unitInfo.Length - 1; index >= 0; --index)
                                    unitGUIDs[index] = unitInfo[index].SimulationEntityGUID;
                            }

                            SandboxManager.PostOrder(
                                (Order)new OrderChangeMovementRatio(currentCursor15.EntityGUID,
                                    unitGUIDs, this.movementRatio),
                                (int)Snapshots.ArmyCursorSnapshot.PresentationData.EmpireIndex);
                        }

                        GUILayout.EndHorizontal();
                        GUILayout.Space(5f);
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Toggle(this.transformToNavalTransport,
                            "Transform to Naval Transport") != this.transformToNavalTransport)
                        {
                            this.transformToNavalTransport = !this.transformToNavalTransport;
                            if ((UnityEngine.Object)this.selectedArmy != (UnityEngine.Object)null)
                                this.selectedArmy.DoUpdateMesh(false, this.transformToNavalTransport);
                        }

                        GUI.enabled = Snapshots.ArmyCursorSnapshot.PresentationData.ArmyActions[18]
                            .IsValid;
                        if (GUILayout.Button("Use Airport"))
                            Amplitude.Mercury.Presentation.Presentation.PresentationCursorController
                                .ChangeToDestinationAirportSelectionCursor(
                                    (SimulationEntityGUID)this.selectedArmy.SimulationEntityGuid);
                        GUI.enabled = true;
                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        this.selectedArmy = (PresentationArmy)null;
                        GUILayout.Label("Also waiting for an army to be selected...");
                    }
                }
                else
                    GUILayout.Label("Waiting for the army cursor snapshot...");

                GUILayout.Space(20f);
                GUILayout.Label("SQUADRON TOOLS");
                if (Snapshots.SquadronCursorSnapshot != null)
                {
                    if (Amplitude.Mercury.Presentation.Presentation.PresentationCursorController
                        .CurrentCursor is SquadronCursor currentCursor16)
                    {
                        if ((UnityEngine.Object)this.selectedSquadron == (UnityEngine.Object)null ||
                            currentCursor16.EntityGUID !=
                            (SimulationEntityGUID)this.selectedSquadron.SimulationEntityGuid)
                            this.selectedSquadron =
                                Amplitude.Mercury.Presentation.Presentation
                                    .PresentationEntityFactoryController
                                    .GetSquadron((ulong)currentCursor16.EntityGUID);
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("Disband Unit"))
                        {
                            if (currentCursor16.SelectedUnitCount <= 0)
                                currentCursor16.SelectAll(SquadronCursorSnapshot.ActivationFlags
                                    .FloatingWindow);
                            SimulationEntityGUID[] unitGUIDsToDisband =
                                new SimulationEntityGUID[currentCursor16.SelectedUnitCount];
                            Array.Copy((Array)currentCursor16.SelectedUnits,
                                (Array)unitGUIDsToDisband, currentCursor16.SelectedUnitCount);
                            SandboxManager.PostOrder(
                                (Order)new OrderDisbandUnits(currentCursor16.EntityGUID,
                                    unitGUIDsToDisband),
                                (int)Snapshots.SquadronCursorSnapshot.PresentationData.EmpireIndex);
                        }

                        GUILayout.EndHorizontal();
                        GUILayout.Space(5f);
                        GUILayout.BeginHorizontal();
                        this.damageAmount = GUILayout.HorizontalSlider(this.damageAmount, 0.1f, 0.9f);
                        if (GUILayout.Button("Damage Unit"))
                        {
                            if (currentCursor16.SelectedUnitCount <= 0)
                                currentCursor16.SelectAll(SquadronCursorSnapshot.ActivationFlags
                                    .FloatingWindow);
                            SimulationEntityGUID[] unitGUIDsToDisband =
                                new SimulationEntityGUID[currentCursor16.SelectedUnitCount];
                            Array.Copy((Array)currentCursor16.SelectedUnits,
                                (Array)unitGUIDsToDisband, currentCursor16.SelectedUnitCount);
                            PostOrderTicket postOrderTicket = SandboxManager.PostAndTrackOrder(
                                (Order)new OrderDamageUnits(currentCursor16.EntityGUID,
                                    unitGUIDsToDisband, this.damageAmount),
                                (int)Snapshots.SquadronCursorSnapshot.PresentationData.EmpireIndex);
                            if ((UnityEngine.Object)this.selectedSquadron != (UnityEngine.Object)null)
                                postOrderTicket.UponCompletion((System.Action)(() =>
                                   this.selectedSquadron.SetIsInAction(false)));
                        }

                        GUILayout.EndHorizontal();
                        GUILayout.Space(5f);
                        GUILayout.BeginHorizontal();
                        int result;
                        if (int.TryParse(GUILayout.TextField(this.experienceAmount.ToString()),
                            out result))
                            this.experienceAmount = result;
                        if (GUILayout.Button("Add Experience to Unit(s)"))
                        {
                            if (currentCursor16.SelectedUnitCount <= 0)
                                currentCursor16.SelectAll(SquadronCursorSnapshot.ActivationFlags
                                    .FloatingWindow);
                            int[] unitsExperiencePoints = new int[currentCursor16.SelectedUnits.Length];
                            for (int index = 0; index < currentCursor16.SelectedUnits.Length; ++index)
                                unitsExperiencePoints[index] = this.experienceAmount;
                            SimulationEntityGUID[] unitGUIDs =
                                new SimulationEntityGUID[currentCursor16.SelectedUnitCount];
                            Array.Copy((Array)currentCursor16.SelectedUnits, (Array)unitGUIDs,
                                currentCursor16.SelectedUnitCount);
                            SandboxManager.PostOrder(
                                (Order)new OrderChangeUnitsXP(currentCursor16.EntityGUID, unitGUIDs,
                                    unitsExperiencePoints),
                                (int)Snapshots.SquadronCursorSnapshot.PresentationData.EmpireIndex);
                        }

                        GUILayout.EndHorizontal();
                        GUILayout.Space(5f);
                        GUILayout.BeginHorizontal();
                        this.movementRatio = GUILayout.HorizontalSlider(this.movementRatio, 0.0f, 1f);
                        if (GUILayout.Button("Change Movement Ratio"))
                        {
                            SimulationEntityGUID[] unitGUIDs;
                            if (currentCursor16.SelectedUnitCount > 0)
                            {
                                unitGUIDs = new SimulationEntityGUID[currentCursor16.SelectedUnitCount];
                                Array.Copy((Array)currentCursor16.SelectedUnits, (Array)unitGUIDs,
                                    currentCursor16.SelectedUnitCount);
                            }
                            else
                            {
                                UnitInfo[] unitInfo = Snapshots.SquadronCursorSnapshot.PresentationData
                                    .UnitInfo;
                                unitGUIDs = new SimulationEntityGUID[unitInfo.Length];
                                for (int index = unitInfo.Length - 1; index >= 0; --index)
                                    unitGUIDs[index] = unitInfo[index].SimulationEntityGUID;
                            }

                            SandboxManager.PostOrder(
                                (Order)new OrderChangeMovementRatio(currentCursor16.EntityGUID,
                                    unitGUIDs, this.movementRatio),
                                (int)Snapshots.SquadronCursorSnapshot.PresentationData.EmpireIndex);
                        }

                        GUILayout.EndHorizontal();
                    }
                    else
                    {
                        this.selectedSquadron = (PresentationSquadron)null;
                        GUILayout.Label("Also waiting for a squadron to be selected...");
                    }
                }
                else
                    GUILayout.Label("Waiting for the squadron cursor snapshot...");
            }
            else
                GUILayout.Label("Waiting for the presentation...");

            GUILayout.EndVertical();
        }

        private static int CompareUnitDefinitions(string lhs, string rhs)
        {
            int num1 = lhs.IndexOf("Era");
            int num2 = (int)lhs[num1 + 3] - 48;
            int num3 = rhs.IndexOf("Era");
            int num4 = (int)rhs[num3 + 3] - 48;
            int num5 = num2.CompareTo(num4);
            if (num5 == 0)
                num5 = lhs.CompareTo(rhs);
            return num5;
        }


    }
}
