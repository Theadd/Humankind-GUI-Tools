using Amplitude.UI;
using Modding.Humankind.DevTools;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public static class InGameUIController
    {

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
                var height = battleScreen.Parent.Parent.GlobalRect.height * backScreen.MinWindowRect.height /
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
    }
}
