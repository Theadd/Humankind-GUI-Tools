using System;
using System.Collections.Generic;
using System.Reflection;
using Amplitude.Mercury.Presentation;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.Core;
using UnityEngine;
using Cursor = Amplitude.Mercury.Presentation.Cursor;

namespace DevTools.Humankind.GUITools.UI.SceneInspector
{
    public class SceneInspectorRaycaster
    {
        public RaycastHit[] raycastResult = new RaycastHit[5];

        // private Dictionary<System.Type, Cursor> cursorByType = new Dictionary<System.Type, Cursor>();
        
        public Cursor CurrentCursor => Presentation.PresentationCursorController.CurrentCursor;
        
        private static readonly FieldInfo CameraGraphicServiceField = typeof(PresentationCursorController)
            .GetField("cameraGraphicService", R.NonPublicInstance);
        
        public static Amplitude.Graphics.CameraGraphicService GetCameraGraphicService =>
            (Amplitude.Graphics.CameraGraphicService) CameraGraphicServiceField.GetValue(Presentation.PresentationCursorController);

        
        private static readonly FieldInfo CursorByTypeField = typeof(PresentationCursorController)
            .GetField("cursorByType", R.NonPublicInstance);
        
        public static Dictionary<Type, Cursor> GetCursorByType =>
            (Dictionary<Type, Cursor>) CursorByTypeField.GetValue(Presentation.PresentationCursorController);

        private static readonly int AllLayers = ~0;
        
        public void FillWithRaycastedCursorTargets(
            Vector2 mousePosition,
            List<PresentationEntity> entities)
        {
            int layerMask = AllLayers;  // CurrentCursor.LayerMask 

            Ray ray = GetCameraGraphicService.Camera.ScreenPointToRay((Vector3) mousePosition);
            
            Loggr.Log("RAY = " + ray.ToString(), ConsoleColor.DarkMagenta);
            
            Loggr.Log("\tCurrentCursor.TraverseAll = " + CurrentCursor.TraverseAll + 
                      ", CurrentCursor.LayerMask = " + layerMask, ConsoleColor.Cyan);
            if (CurrentCursor.TraverseAll)
            {
                int num = Physics.RaycastNonAlloc(ray, raycastResult, float.PositiveInfinity, layerMask);
                
                Debug.DrawRay(ray.origin, ray.direction, Color.magenta, 10000f);
                
                Loggr.Log("raw collisions found = " + num);
                
                for (int index = 0; index < num; ++index)
                {
                    PresentationCursorTarget component = raycastResult[index].collider.gameObject.GetComponent<PresentationCursorTarget>();
                    
                    Loggr.Log("\t\tCOLLIDED with GameObject <" + raycastResult[index].collider.gameObject.name + 
                              ">, has target component ? " + (component != null), ConsoleColor.DarkCyan);
                    
                    if ((UnityEngine.Object) component != (UnityEngine.Object) null 
                        && (UnityEngine.Object) component.PresentationEntity != (UnityEngine.Object) null) 
                        entities.Add(component.PresentationEntity);
                }
                for (int index = 0; index < num; ++index)
                    raycastResult[index] = new RaycastHit();
            }
            
            //else
            {
                RaycastHit hitInfo;
                if (!Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, layerMask))
                {
                    Loggr.Log("NO raw collisions found for non-traverse all raycast");
                    return;
                }
                PresentationCursorTarget component = hitInfo.collider.gameObject.GetComponent<PresentationCursorTarget>();
                Loggr.Log("\t\tCOLLIDED with GameObject <" + hitInfo.collider.gameObject.name + 
                          ">, has target component ? " + (component != null), ConsoleColor.DarkCyan);
                if (!((UnityEngine.Object) component != (UnityEngine.Object) null) || !((UnityEngine.Object) component.PresentationEntity != (UnityEngine.Object) null))
                    return;
                entities.Add(component.PresentationEntity);
            }
        }
    }
}
