// Decompiled with JetBrains decompiler
// Type: Amplitude.Mercury.Overlay.FloatingWindow_TerrainPicking
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 08C51C5C-2353-4B85-AFD8-BE823F2052A3
// Assembly location: S:\Program Files\Steam\steamapps\common\Humankind\Humankind_Data\Managed\Assembly-CSharp.dll

using Amplitude.Framework.Overlay;
using Amplitude.Graphics;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Overlay;
using Amplitude.Mercury;
using Amplitude;
using Amplitude.Mercury.Terrain;
using System;
using System.Reflection;
using UnityEngine;

using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI
{
    public class TerrainPickingToolWindow : FloatingToolWindow
    {
        public override string WindowTitle { get; set; } = "TERRAIN PICKING TOOL";

        public override string WindowGUIStyle { get; set; } = "PopupWindow";

        public override bool ShouldBeVisible => !GlobalSettings.ShouldHideTools;

        public override bool ShouldRestoreLastWindowPosition => true;

        public override Rect WindowRect { get; set; } = new Rect(130f, 260f, 468f, 500f);

        private Color bgColor = new Color32(255, 255, 255, 230);
        private Color bgColorOpaque = new Color32(255, 255, 255, 255);

        public override void OnGUIStyling()
        {
            base.OnGUIStyling();
            GUI.backgroundColor = GlobalSettings.WindowTransparency.Value ? bgColor : bgColorOpaque;
        }

        public static readonly FieldInfo GameUtilsField =
                typeof(Amplitude.Mercury.UI.Utils).GetField("GameUtils", R.NonPublicStatic);

        public static Amplitude.Mercury.UI.Helpers.GameUtils GameUtils =>
                ((Amplitude.Mercury.UI.Helpers.GameUtils)GameUtilsField.GetValue(null));

        public override void OnDrawUI()
        {
            if (GlobalSettings.WindowTitleBar.Value)
                WindowUtils.DrawWindowTitleBar(this);

            OnDrawWindowClientArea(0);
        }
        [RenderContext.RenderContext]
        [SerializeField]
        private int renderContext;
        [SerializeField]
        private bool moreTileInfo;
        [SerializeField]
        private bool moreDistrictInfo;

        private GUIStyle GridHeaderStyle { get; set; } = new GUIStyle(UIManager.DefaultSkin.FindStyle("PopupWindow.Grid"))
        {
            alignment = TextAnchor.LowerRight,
            padding = new RectOffset(0, 4, 4, 4),
            normal = new GUIStyleState()
            {
                background = null,
                textColor = Color.white
            }
        };

        private static float cellWidth = 34f;
        private static float cellSpace = 1f;
        private GUILayoutOption span1Cell = GUILayout.Width(cellWidth);
        private GUILayoutOption span2Cells = GUILayout.Width(cellWidth * 2 + cellSpace);
        private GUILayoutOption span3Cells = GUILayout.Width(cellWidth * 3 + cellSpace * 2);
        private GUILayoutOption span4Cells = GUILayout.Width(cellWidth * 4 + cellSpace * 3);
        private GUILayoutOption span5Cells = GUILayout.Width(cellWidth * 5 + cellSpace * 4);
        private GUILayoutOption span6Cells = GUILayout.Width(cellWidth * 6 + cellSpace * 5);
        private GUILayoutOption span7Cells = GUILayout.Width(cellWidth * 7 + cellSpace * 6);
        private GUILayoutOption span8Cells = GUILayout.Width(cellWidth * 8 + cellSpace * 7);

        private Color32 bgCellColor = new Color32(165, 206, 254, 255);

        protected override void OnDrawWindowClientArea(int instanceId)
        {
            GUI.color = Color.white;
            GUI.backgroundColor = bgCellColor;
            GUILayout.BeginVertical((GUIStyle)"Widget.ClientArea", GUILayout.ExpandWidth(true));
            GUILayout.Space(4f);
            if ((UnityEngine.Object)Amplitude.Mercury.Presentation.Presentation.PresentationCursorController != (UnityEngine.Object)null && Amplitude.Mercury.Presentation.Presentation.PresentationCursorController.HasBeenStarted && !Amplitude.Mercury.Presentation.Presentation.PresentationCursorController.HasBeenShutdown)
            {
                ITerrainPickingService instance = RenderContextAccess.GetInstance<ITerrainPickingService>(this.renderContext);
                if (instance != null)
                {
                    Hexagon.OffsetCoords offsetHexCoords = new Hexagon.OffsetCoords();
                    if (instance.ScreenPositionToHexagonOffsetCoords((Vector2)Input.mousePosition, ref offsetHexCoords))
                    {
                        WorldPosition hexagon = new WorldPosition(offsetHexCoords);
                        using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
                        {
                            GUILayout.Label("Coordinates", GridHeaderStyle, span5Cells);
                            GUILayout.Label(hexagon.Column.ToString(), (GUIStyle)"PopupWindow.Grid", span1Cell);
                            int num = hexagon.Row;
                            GUILayout.Label(num.ToString(), (GUIStyle)"PopupWindow.Grid", span1Cell);
                            GUILayout.Label("Index", GridHeaderStyle, span4Cells);
                            num = hexagon.ToTileIndex();
                            GUILayout.Label(num.ToString(), (GUIStyle)"PopupWindow.Grid", span2Cells);
                        }
                        this.DisplayTileInfoAt(hexagon);
                        this.DisplayDistrictInfoAt(hexagon);
                    }
                    else
                        GUILayout.Label("<size=10>\tOUT OF BOUNDS!</size>");
                }
                else
                    GUILayout.Label("Waiting for the terrain picking service...");
            }
            else
                GUILayout.Label("Waiting for the presentation cursor controller to be started...");

            GUILayout.Space(12f);
            GUILayout.EndVertical();
        }

        private void DisplayTileInfoAt(WorldPosition hexagon)
        {
            ref TileInfo local = ref Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.TileInfo.Data[hexagon.ToTileIndex()];
            using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
            {
                GUILayout.Label("Territory", GridHeaderStyle, span5Cells);
                GUILayout.Label(local.TerritoryIndex.ToString(), (GUIStyle)"PopupWindow.Grid", span1Cell);

                GUILayout.Label(GameUtils.GetTerritoryName((int)local.TerritoryIndex), (GUIStyle)"PopupWindow.Grid", span7Cells);
            }
            this.moreTileInfo = true;
            /*using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
            {
                GUILayout.FlexibleSpace();
                this.moreTileInfo = GUILayout.Toggle(this.moreTileInfo, this.moreTileInfo ? "Less tile info" : "More tile info", (GUIStyle)"PopupWindow.Button");
            }*/
            if (!this.moreTileInfo)
                return;
            using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
            {
                GUILayout.Label("Elevation", GridHeaderStyle, span5Cells);
                GUILayout.Label(local.Elevation.ToString(), (GUIStyle)"PopupWindow.Grid", span1Cell);
            }
            using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
            {
                string text = (int)local.TerrainType < Snapshots.WorldSnapshot.PresentationData.TerrainTypeDefinitions.Length ? Snapshots.WorldSnapshot.PresentationData.TerrainTypeDefinitions[(int)local.TerrainType].Name.ToString() : "Unknown";
                GUILayout.Label("Terrain", GridHeaderStyle, span5Cells);
                GUILayout.Label(local.TerrainType.ToString(), (GUIStyle)"PopupWindow.Grid", span1Cell);
                GUILayout.Label(text, (GUIStyle)"PopupWindow.Grid", span7Cells);
            }
            using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
            {
                string text = (int)local.Biome < Snapshots.WorldSnapshot.PresentationData.BiomeDefinitions.Length ? Snapshots.WorldSnapshot.PresentationData.BiomeDefinitions[(int)local.Biome].Name.ToString() : "Unknown";
                GUILayout.Label("Biome", GridHeaderStyle, span5Cells);
                GUILayout.Label(local.Biome.ToString(), (GUIStyle)"PopupWindow.Grid", span1Cell);
                GUILayout.Label(text, (GUIStyle)"PopupWindow.Grid", span7Cells);
            }
            using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
            {
                string text = (int)local.PointOfInterest < Snapshots.WorldSnapshot.PresentationData.PointOfInterestDefinitions.Length ? Snapshots.WorldSnapshot.PresentationData.PointOfInterestDefinitions[(int)local.PointOfInterest].Name.ToString() : "None";
                GUILayout.Label("Point of Interest", GridHeaderStyle, span5Cells);
                GUILayout.Label(local.PointOfInterest.ToString(), (GUIStyle)"PopupWindow.Grid", span1Cell);
                GUILayout.Label(text, (GUIStyle)"PopupWindow.Grid", span7Cells);
            }
            using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
            {
                byte wonderDefinitionIndex = local.GetNaturalWonderDefinitionIndex();
                string text = (int)wonderDefinitionIndex < Snapshots.WorldSnapshot.PresentationData.NaturalWonderDefinitions.Length ? Snapshots.WorldSnapshot.PresentationData.NaturalWonderDefinitions[(int)wonderDefinitionIndex].Name.ToString() : "None";
                GUILayout.Label("Natural wonder", GridHeaderStyle, span5Cells);
                GUILayout.Label(wonderDefinitionIndex.ToString(), (GUIStyle)"PopupWindow.Grid", span1Cell);
                GUILayout.Label(local.EncodedNaturalWonder.ToString(), (GUIStyle)"PopupWindow.Grid", span1Cell);
                GUILayout.Label(text, (GUIStyle)"PopupWindow.Grid", span6Cells);
            }
            using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
            {
                GUILayout.Label("River (id, distance, downstream)", GridHeaderStyle, span5Cells);
                GUILayout.Label(local.RiverIndex.ToString(), (GUIStyle)"PopupWindow.Grid", span1Cell);
                GUILayout.Label(local.RiverDistance.ToString(), (GUIStyle)"PopupWindow.Grid", span1Cell);
                GUILayout.Label(local.RiverDownstream.ToString(), (GUIStyle)"PopupWindow.Grid", span1Cell);
            }
        }

        private void DisplayDistrictInfoAt(WorldPosition hexagon)
        {
            int districtIndexAt = Snapshots.GameSnapshot.PresentationData.GetDistrictIndexAt(hexagon.ToTileIndex());
            if (districtIndexAt < 0)
            {
                // using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
                //     GUILayout.Label("No district under the mouse!");
                /*using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
                {
                    GUILayout.FlexibleSpace();
                    this.moreDistrictInfo = GUILayout.Toggle(this.moreDistrictInfo, this.moreDistrictInfo ? "Less district info" : "More district info", (GUIStyle)"PopupWindow.Button");
                }*/
            }
            else
            {
                ref DistrictInfo local = ref Snapshots.GameSnapshot.PresentationData.DistrictInfo.Data[districtIndexAt];
                using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
                {
                    GUILayout.Label("District (Index, Guid)", GridHeaderStyle, span5Cells);
                    GUILayout.Label(districtIndexAt.ToString(), (GUIStyle)"PopupWindow.Grid", span1Cell);
                    GUILayout.Label(local.SimulationEntityGUID.ToString(), (GUIStyle)"PopupWindow.Grid", span1Cell);
                    GUILayout.Label("Owner EmpireIndex", GridHeaderStyle, span5Cells);
                    GUILayout.Label(local.EmpireIndex.ToString(), (GUIStyle)"PopupWindow.Grid", span1Cell);
                }
                /*using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
                {
                    GUILayout.FlexibleSpace();
                    this.moreDistrictInfo = GUILayout.Toggle(this.moreDistrictInfo, this.moreDistrictInfo ? "Less district info" : "More district info", (GUIStyle)"PopupWindow.Button");
                }*/
                this.moreDistrictInfo = true;
                if (!this.moreDistrictInfo)
                    return;
                using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
                {
                    GUILayout.Label("Definition", GridHeaderStyle, span5Cells);
                    GUILayout.Label(local.DistrictDefinitionName.ToString(), (GUIStyle)"PopupWindow.Grid", span8Cells);
                }
                using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
                {
                    GUILayout.Label("VisualAffinityName", GridHeaderStyle, span5Cells);
                    GUILayout.Label(local.VisualAffinityName.ToString(), (GUIStyle)"PopupWindow.Grid", span8Cells);
                }
                using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
                {
                    GUILayout.Label("Visual level", GridHeaderStyle, span5Cells);
                    GUILayout.Label(local.VisualLevel.ToString(), (GUIStyle)"PopupWindow.Grid", span1Cell);
                }
                using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
                {
                    GUILayout.Label("IsFortified", GridHeaderStyle, span5Cells);
                    GUILayout.Label(local.IsFortified.ToString(), (GUIStyle)"PopupWindow.Grid", span2Cells);
                    GUILayout.Label("Fort Health", GridHeaderStyle, span2Cells);
                    GUILayout.Label(((int)(local.FortificationHealthRatio * 100f)).ToString() + "%", (GUIStyle)"PopupWindow.Grid", span1Cell);
                    GUILayout.Label("Fort Level", GridHeaderStyle, span2Cells);
                    GUILayout.Label(local.FortificationLevel.ToString(), (GUIStyle)"PopupWindow.Grid", span1Cell);
                }
                
            }
        }
    }
}
