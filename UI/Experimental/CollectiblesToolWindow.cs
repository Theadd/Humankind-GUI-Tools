using Amplitude.Mercury.Interop;
using System.Collections.Generic;
using UnityEngine;
using Amplitude.Mercury.Presentation;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI
{
    public class CollectiblesToolWindow : FloatingToolWindow
    {
        private Vector2 scrollPosition;
        private string filter;

        public override string WindowTitle { get; set; } = "CURIOSITIES TOOL";

        public override string WindowGUIStyle { get; set; } = "PopupWindow";

        public override bool ShouldBeVisible => !GlobalSettings.ShouldHideTools;

        public override bool ShouldRestoreLastWindowPosition => true;

        public override Rect WindowRect { get; set; } = new Rect(130f, 260f, 720f, 500f);

        private Color bgColor = new Color32(255, 255, 255, 230);
        private Color bgColorOpaque = new Color32(255, 255, 255, 255);

        public override void OnGUIStyling()
        {
            base.OnGUIStyling();
            GUI.backgroundColor = GlobalSettings.WindowTransparency.Value ? bgColor : bgColorOpaque;
        }

        public override void OnDrawUI()
        {
            if (GlobalSettings.WindowTitleBar.Value)
                WindowUtils.DrawWindowTitleBar(this);

            GUILayout.BeginVertical((GUIStyle) "Widget.ClientArea", GUILayout.ExpandWidth(true));

            OnDrawWindowContent();

            GUILayout.EndVertical();
        }

        private static void SetGodMode(bool enable) =>
            HarmonyLib.AccessTools.PropertySetter(typeof(GodMode), "Enabled")?
                .Invoke(null, new object[] {enable});

        private static bool curiosityCursor = false;

        protected void OnDrawWindowContent()
        {
            GUILayout.BeginHorizontal();
            if (curiosityCursor != GUILayout.Toggle(curiosityCursor, "Force curiosity spawn cursor."))
            {
                curiosityCursor = !curiosityCursor;
                SetGodMode(curiosityCursor);

                if (curiosityCursor)
                    Presentation.PresentationCursorController.ChangeToCuriositySpawnCursor();
            }

            GUILayout.FlexibleSpace();
            GUILayout.Label("FILTER:");
            this.filter = GUILayout.TextField(this.filter, GUILayout.Width(135f));
            GUILayout.EndHorizontal();

            if (Snapshots.GameSnapshot == null || Snapshots.GameSnapshot.PresentationData == null)
                return;
            
            GUILayout.Space(8f);
     
            GUILayout.Label("ACTIVE CURIOSITIES", "PopupWindow.SectionHeader");

            scrollPosition = GUILayout.BeginScrollView(
                scrollPosition,
                false,
                true,
                "horizontalscrollbar",
                "verticalscrollbar",
                "scrollview",
                new GUILayoutOption[]
                {
                    GUILayout.Height(320f)
                });

            ArrayWithFrame<CollectibleInfo> collectibleInfo = Snapshots.GameSnapshot.PresentationData.CollectibleInfo;
            GameSnapshot.Data.ExtendedCollectibleInfo[] collectibleInfoIndex =
                Snapshots.GameSnapshot.PresentationData.ExtendedInfoPerCollectibleInfoIndex;

            GUILayout.BeginVertical("PopupWindow.ScrollViewGridContainer");
            
            

            int length = collectibleInfo.Length;
            for (int index1 = 0; index1 < length; ++index1)
            {
                ref CollectibleInfo local = ref collectibleInfo.Data[index1];
                if (local.PoolAllocationIndex >= 0)
                {
                    string text1 = R.Text.GetLocalizedTitle(local.CollectibleDefinitionName);
                    if (string.IsNullOrEmpty(this.filter) || text1.Contains(this.filter))
                    {
                        int availabilityBits = local.EmpireAvailabilityBits;
                        int empireCollectedBits = local.EmpireCollectedBits;
                        int empireDetectedBits = local.EmpireDetectedBits;
                        Amplitude.Mercury.WorldPosition worldPosition =
                            new Amplitude.Mercury.WorldPosition(local.TileIndex);
                        int lootCount = collectibleInfoIndex[index1].LootCount;
                        List<string> loot = new List<string>();
                        for (int index2 = 0; index2 < lootCount; ++index2)
                        {
                            var lootInfoText = collectibleInfoIndex[index1].LootInfo[index2].Localization;
                            if (lootInfoText.Length > 0)
                            {
                                loot.Add(lootInfoText);
                            }
                        }

                        using (new GUILayout.HorizontalScope(GUILayout.ExpandWidth(false)))
                        {
                            GUILayout.Label("<b><size=10><color=#FFFFFF88>" + worldPosition + "</color></size></b>", "RightAlignedLabel", GUILayout.Width(45f));
                            GUILayout.Label("<b><size=10>" + text1 + "</size></b>");
                            GUILayout.FlexibleSpace();
            
                            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                            foreach (var lootInfo in loot)
                            {
                                Utils.DrawLootInfoLabel(lootInfo);
                                // GUILayout.Label(content, Utils.InlineLabel, GUILayout.ExpandWidth(false));
                            }
                            GUILayout.EndHorizontal();
                            
                            GUILayout.Label("<b><size=10>" + availabilityBits + " / " + empireCollectedBits + " / " + empireDetectedBits + "</size></b>", "RightAlignedLabel", GUILayout.Width(90f));
                            
                            if (GUILayout.Button("<b><size=10>LOCATE</size></b>", GUILayout.Width(65f)))
                                Amplitude.Mercury.Presentation.Presentation.PresentationCameraController.CenterCameraAt(
                                    worldPosition);
                        }
                    }
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            GUILayout.Space(12f);
        }
    }
}