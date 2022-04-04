using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Amplitude.Extensions;
using Amplitude.Mercury;
using Amplitude.Mercury.Presentation;
using Amplitude.UI;
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
        // private static bool _wireRendererAttached = false;

        public void TestRaycastFromCameraToScreenPoint()
        {
            var cam = GetCameraGraphicService.Camera;
            var mousePos = Input.mousePosition;
            RaycastHit hit;
            
            
            
            Ray ray = cam.ScreenPointToRay(mousePos);
            
            Loggr.Log("\nInput.mousePosition = " + mousePos.ToString(), ConsoleColor.Yellow);
            Loggr.Log("Ray = " + ray.ToString(), ConsoleColor.Yellow);
        
            if (Physics.Raycast(ray, out hit)) {
                Transform objectHit = hit.transform;
            
                // Do something with the object that was hit by the raycast.
                Loggr.Log("\tThe object that was hit by the raycast is at position " + objectHit.position.ToString());
                Loggr.Log($"\tGameObject \"{objectHit.gameObject.name}\"'s PATH = " + objectHit.gameObject.GetPath());
            }

            /*if (!_wireRendererAttached)
            {
                WireRenderer.Attach(cam);
                _wireRendererAttached = true;
            }
            else
            {
                WireRenderer.Detach();
                _wireRendererAttached = false;
            }*/
        }
        
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

        public void GetAllTransformsAt(Vector3 position)
        {
            Loggr.Log("GETTING ALL TRANSFORMS AT POSITION %WHITE%" + position.ToString(), ConsoleColor.Magenta);

            var allChildren = SceneInspectorController.PresentationGO.transform.GetComponentsInChildren<Transform>(true);
            Loggr.Log("ALL CHILDREN COUNT = " + allChildren.Length);
            var childrenNearby = allChildren
                .Where(t =>
                    Mathf.Abs(t.position.x - position.x) <= 3f
                    && Mathf.Abs(t.position.z - position.z) <= 3f)
                .ToArray();
            Loggr.Log("NEARBY CHILDREN COUNT = " + childrenNearby.Length);

            
            
            if (childrenNearby.Length < 20)
            {
                foreach (var transform in childrenNearby)
                {
                    var go = transform.gameObject;
                    Component[] components = go.GetComponents<Component>();
                    var componentNames = string.Join(", ", components.Select(c => c.GetType().Name));
                    Loggr.Log("\t" 
                              + go.name + " %WHITE%@ " 
                              + go.GetPath(SceneInspectorController.PresentationGO.name), 
                        ConsoleColor.DarkCyan);
                    Loggr.Log("\t\tWITH: %GREEN%" +componentNames);
                }
            }
            
            var allTransforms = GameObject.FindObjectsOfType<Transform>(true);
            Loggr.Log("ALL TRANSFORMS COUNT = " + allTransforms.Length, ConsoleColor.Green);
            var transformsNearby = allTransforms
                .Where(t =>
                    Mathf.Abs(t.position.x - position.x) <= 3f
                    && Mathf.Abs(t.position.z - position.z) <= 3f)
                .ToArray();
            Loggr.Log("NEARBY TRANSFORMS COUNT = " + transformsNearby.Length);
            
            
        }

        private Rect _globalRect;

        private Vector2 ApplyRelativePosition(Vector2 position)
        {
            return new Vector2(
                (_globalRect.width * position.x) / Screen.width,
                (_globalRect.height * position.y) / Screen.height
            );
        }
        
        public void GetAllUITransformsAt(Vector2 position)
        {
            Loggr.Log("GETTING ALL UI_TRANSFORMS AT POSITION %WHITE%" + position.ToString(), ConsoleColor.Magenta);

            _globalRect = InGameUIController.GetWindowsRootGlobalRect();
            Vector2 pos = ApplyRelativePosition(position);
            Loggr.Log("GETTING ALL UI_TRANSFORMS AT %DARKYELLOW%RELATIVE %DEFAULT%POSITION %WHITE%" + pos.ToString(), ConsoleColor.Magenta);
            
            var allUITransforms = GameObject.FindObjectsOfType<UITransform>(true);
            Loggr.Log("ALL UI_TRANSFORMS COUNT = " + allUITransforms.Length, ConsoleColor.Green);

            var uiTransformsAtPosition = allUITransforms
                .Where(t => t.GlobalRect != t.LocalRect)
                .Where(t => t.GlobalRect.Contains(pos))
                .ToArray();

            Loggr.Log("COUNT OF UI_TRANSFORMS AT POSITION = " + uiTransformsAtPosition.Length, ConsoleColor.Green);

            if (uiTransformsAtPosition.Length > 0)
            {
                var visibleUITransforms = uiTransformsAtPosition
                    .Where(t => t.VisibleGlobally)
                    .ToArray();
                Loggr.Log("COUNT OF VISIBLE UI_TRANSFORMS AT POSITION = " + visibleUITransforms.Length, ConsoleColor.Green);

                if (visibleUITransforms.Length <= 120)
                {
                    var displayValues = visibleUITransforms
                        .Select(t => "%RED%"
                                     + t.gameObject.name
                                     + " %DARKYELLOW%" 
                                     + t.GetScenePath()
                                     + " %WHITE%"
                                     + t.GlobalRect.ToString()
                                     + "")
                        .ToArray();
                    
                    Loggr.Log(string.Join("\n", displayValues.Select(s => "\t" + s)));
                }
                
                SceneInspectorController.Screen.RebuildUIHierarchyUsing(visibleUITransforms);
            }
        }
    }
}
