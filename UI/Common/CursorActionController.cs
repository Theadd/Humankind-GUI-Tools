using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Presentation;
using Amplitude.Mercury.Sandbox;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using UnityEngine;
using Amplitude;
using Amplitude.Framework;
using Amplitude.Mercury;
using Amplitude.Mercury.Simulation;

namespace DevTools.Humankind.GUITools.UI
{
    public static partial class ActionController
    {
        public static void GiveVisionAtCursorPosition()
        {
            SandboxManager.PostOrder(new OrderGiveVisionAtPosition
            {
                WorldPosition = Presentation.PresentationCursorController.CurrentHighlightedPosition,
                VisionRange = 6,
                DetectionRange = 6,
                VisionHeight = 10,
                Duration = 1
            });
        }
        
        public static void CreateCityAtCursorPosition()
        {
            SandboxManager.PostOrder(new EditorOrderCreateCityAt
            {
                EmpireIndex = (int)Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex,
                CityTileIndex = Presentation.PresentationCursorController.CurrentHighlightedPosition.ToTileIndex()
            });
        }
        
        public static void CreateExtensionDistrictIndustryUnderCursor()
        {
            DistrictInfo districtInfo2;
            if (TryGetDistrictInfoAt(Presentation.PresentationCursorController.CurrentHighlightedPosition, out districtInfo2))
            {
                SandboxManager.PostOrder(new EditorOrderCreateExtensionDistrictAt
                {
                    DistrictDefinitionName = new StaticString("Extension_Base_Industry"),
                    TileIndex = Presentation.PresentationCursorController.CurrentHighlightedPosition.ToTileIndex()
                });
            }
        }
        
        public static void DestroyDistrictOrSettlementUnderCursor()
        {
            DistrictInfo districtInfo3;
            WorldPosition worldPosition = Presentation.PresentationCursorController.CurrentHighlightedPosition;
            
            if (TryGetDistrictInfoAt(worldPosition, out districtInfo3))
            {
                EditorOrder order2;
                if (districtInfo3.DistrictDefinitionName == PresentationDistrict.CityCenterDistrictDefinition 
                    || districtInfo3.DistrictDefinitionName == PresentationDistrict.CampDistrictDefinition 
                    || districtInfo3.DistrictDefinitionName == PresentationDistrict.BeforeCampDistrictDefinition)
                {
                    order2 = new EditorOrderDestroySettlement
                    {
                        SettlementTileIndex = worldPosition.ToTileIndex()
                    };
                }
                else
                {
                    order2 = new EditorOrderRemoveDistrictAt
                    {
                        TileIndex = worldPosition.ToTileIndex()
                    };
                }
                SandboxManager.PostOrder(order2);
            }
        }
    }
}
