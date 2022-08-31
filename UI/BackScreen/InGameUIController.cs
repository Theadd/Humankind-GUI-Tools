using System.Reflection;
using Amplitude.UI;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.Core;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public static class InGameUIController
    {
        public static bool IsMouseCovered =>
            (bool) mouseCovered.GetValue(Amplitude.Mercury.Presentation.Presentation.PresentationCursorController);
        
        private static readonly FieldInfo mouseCovered =
            typeof(Amplitude.Mercury.Presentation.PresentationCursorController).GetField("mouseCovered",
                R.NonPublicInstance);
        
        public static void AdaptUIForBackScreenToFit(BackScreenWindow backScreen)
        {
            var battleScreen = GameObject.Find("/WindowsRoot/InGameSelection/BattleScreen")
                ?.GetComponent<UITransform>();
            var battleResultsScreen = GameObject.Find("/WindowsRoot/InGameSelection/BattleAftermathScreen")
                ?.GetComponent<UITransform>();
            var diplomacyHeaderPanel = GameObject.Find("/WindowsRoot/InGameSelection/DiplomaticScreen/_NegociationGroup")
                ?.GetComponent<UITransform>();
            var diplomacyContentPanel = GameObject.Find("/WindowsRoot/InGameSelection/DiplomaticScreen/PanelsGroup")
                ?.GetComponent<UITransform>();
            var diplomacyMeSidePanel = GameObject.Find("/WindowsRoot/InGameSelection/DiplomaticScreen/MeGroup")
                ?.GetComponent<UITransform>();
            var settlementPopulationScreen = GameObject
                .Find("/WindowsRoot/InGameSelection/SettlementScreen/PopulationManagementParent")
                ?.GetComponent<UITransform>();

            if (battleScreen != null)
            {
                var height = battleScreen.Parent.Parent.GlobalRect.height * BackScreenWindow.MinWindowRect.height /
                             Screen.height;
                
                battleScreen.TopAnchor = new UIBorderAnchor(true, 0, height, 0);
                
                if (battleResultsScreen != null)
                    battleResultsScreen.TopAnchor = new UIBorderAnchor(true, 0, height, 0);
                
                if (diplomacyHeaderPanel != null)
                    diplomacyHeaderPanel.TopAnchor = new UIBorderAnchor(true, 0, height, 0);
                
                if (diplomacyContentPanel != null && diplomacyHeaderPanel != null && diplomacyMeSidePanel != null)
                {
                    diplomacyContentPanel.TopAnchor = new UIBorderAnchor(true, 0, 0,
                        diplomacyHeaderPanel.Bottom + diplomacyMeSidePanel.Top - 1f);
                }
                if (settlementPopulationScreen != null)
                    settlementPopulationScreen.TopAnchor = new UIBorderAnchor(true, 0, height, 0);

            }
        }

        public static void SetVisibilityOfInGameOverlays(bool shouldBeVisible)
        {
            var inGameOverlays = GameObject.Find("/WindowsRoot/InGameOverlays")?.GetComponent<UITransform>();
            var diplomaticBanner = GameObject.Find("/WindowsRoot/InGameSelection/DiplomaticBanner")?.GetComponent<UITransform>();
            var terrainTooltipHolder = GameObject.Find("/WindowsRoot/InGameBackground/TerrainTooltipHolderWindow")?.GetComponent<UITransform>();
            var terrainTooltipAnchorShift = GameObject.Find("/WindowsRoot/InGameBackground/TerrainTooltipHolderWindow/LowerLeftAnchorShifted")?.GetComponent<UITransform>();

            if (inGameOverlays != null)
                inGameOverlays.VisibleSelf = shouldBeVisible;
            if (diplomaticBanner != null)
                diplomaticBanner.VisibleSelf = shouldBeVisible;

            if (ToolboxController.IsDocked && terrainTooltipHolder != null)
            {
                if (shouldBeVisible)
                {
                    terrainTooltipHolder.LeftAnchor = new UIBorderAnchor(true, terrainTooltipHolder.LeftAnchor.Percent,
                        terrainTooltipHolder.LeftAnchor.Margin, 0);
                    terrainTooltipHolder.BottomAnchor = new UIBorderAnchor(true, terrainTooltipHolder.BottomAnchor.Percent,
                        terrainTooltipHolder.BottomAnchor.Margin, 0);
                }
                else
                {
                    Rect uiRect = terrainTooltipHolder.Parent.GlobalRect;
                    float nextX = uiRect.width * ToolboxController.ToolboxRect.width / Screen.width;
                    terrainTooltipHolder.LeftAnchor = new UIBorderAnchor(true, terrainTooltipHolder.LeftAnchor.Percent,
                        terrainTooltipHolder.LeftAnchor.Margin, nextX);

                    if (terrainTooltipAnchorShift != null)
                    {
                        terrainTooltipHolder.BottomAnchor = new UIBorderAnchor(true, terrainTooltipHolder.BottomAnchor.Percent,
                            terrainTooltipHolder.BottomAnchor.Margin, terrainTooltipAnchorShift.BottomAnchor.Offset * -1);
                    }
                }
            }

        }

        public static Rect GetWindowsRootGlobalRect()
        {
            Rect result = Rect.zero;
            
            var windowsRoot = GameObject.Find("/WindowsRoot")
                ?.GetComponent<UITransform>();

            if (windowsRoot != null)
            {
                result = windowsRoot.GlobalRect;
            }

            return result;
        }
    }
}
