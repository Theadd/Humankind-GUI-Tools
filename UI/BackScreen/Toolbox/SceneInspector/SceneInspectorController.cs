using System;
using System.Collections.Generic;
using System.Linq;
using Amplitude.Framework;
using Amplitude.Framework.Input;
using Amplitude.Mercury.Presentation;
using BepInEx.Configuration;
using Modding.Humankind.DevTools;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI.SceneInspector
{
    public static class SceneInspectorController
    {
        public static GameObject PresentationGO => _presentationGO ? _presentationGO : _presentationGO = 
            GameObject.Find("[Presentation]/[View]/Views/\"InGame\"/Presentation(Clone)");
        
        private static GameObject _presentationGO;
        
        public static SceneInspectorScreen Screen =>
            _inspectorScreen ?? (_inspectorScreen = new SceneInspectorScreen());
        
        private static SceneInspectorScreen _inspectorScreen;
        
        public static SceneInspectorRaycaster Raycaster =>
            _inspectorRaycaster ?? (_inspectorRaycaster = new SceneInspectorRaycaster());
        
        private static SceneInspectorRaycaster _inspectorRaycaster;

        public static IInputService InputService =>
            _inputService ?? (_inputService = Services.GetService<IInputService>());

        private static IInputService _inputService;
        
        private static KeyboardShortcut InspectKey { get; set; } = 
            new KeyboardShortcut(KeyCode.Space);
        private static KeyboardShortcut RaycastKey { get; set; } = 
            new KeyboardShortcut(KeyCode.Space, KeyCode.LeftAlt);

        public static void Reset()
        {
            _presentationGO = null;
            _inspectorScreen = null;
            _inspectorRaycaster = null;
            _inputService = null;
        }
        
        public static void Run()
        {
            if (InspectKey.IsDown())
                PrintToConsole();
            
            if (RaycastKey.IsDown())
                PrintRaycastResultsToConsole();
            
        }
        
        public static PresentationEntity[] GetValidEntitiesUnderCursor()
        {
            
            if (!Presentation.HasBeenStarted)
                return Array.Empty<PresentationEntity>();
            // if (InGameUIController.IsMouseCovered)
            //     return Array.Empty<PresentationEntity>();
            if (!(Presentation.PresentationCursorController.HoveredPresentationEntites.Count > 0))
                return Array.Empty<PresentationEntity>();

            return Presentation.PresentationCursorController.HoveredPresentationEntites.ToArray();
        }

        public static void PrintToConsole()
        {
            Loggr.LogAll(Presentation.PresentationCursorController);
            Loggr.Log(GetValidEntitiesUnderCursor());
            Loggr.Log("CurrentCursorType = " + Presentation.PresentationCursorController.CurrentCursor?.GetType().Name, ConsoleColor.Green);
        }

        public static void PrintRaycastResultsToConsole()
        {
            List<PresentationEntity> entities = new List<PresentationEntity>();
            var mousePosition = (Vector3)InputService.MousePosition;
            Loggr.Log("RAYCASTING TO " + mousePosition, ConsoleColor.Magenta);
            Raycaster.FillWithRaycastedCursorTargets(mousePosition, entities);

            Loggr.Log(entities);
            // var printable = entities.Select(entity => entity.GetPath());
            foreach (var entity in entities)
            {
                Loggr.Log("\nENTITY NAME = " + entity.name + ", @PATH = " + entity.GetPath());
                var go = GameObject.Find(entity.GetPath());
                
                Loggr.Log(entity);
                
                Loggr.Log("GO @PAT_H = ");
                Loggr.Log(go);
                Loggr.Log(entity.gameObject);
                Loggr.Log("GetGameObjectPath = " + GetGameObjectPath(entity.gameObject), ConsoleColor.Red);
            }

            var cursorsByType = SceneInspectorRaycaster.GetCursorByType;
            Loggr.Log("\n");
            Loggr.Log(cursorsByType.Keys.Select(k => k.Name + ", LayerMask = " + cursorsByType[k].LayerMask.ToString()).ToArray());
            
            var unitPath = "/[Amplitude.Mercury.Application]/Amplitude.Framework.Manager */[Presentation]/[View]/Views/\"InGame\"/Presentation(Clone)/PresentationWorldController/Patch 21/PresentationArmy #7130 of empire 12/Amplitude.Mercury.Presentation.PresentationArmy";
            var unitGO = GameObject.Find(unitPath);
            Loggr.Log("\nunitGO:");
            Loggr.Log(unitGO);
            
            
        }

        public static string GetGameObjectPath(GameObject gameObject) => string.Join("/",
            gameObject.GetComponentsInParent<Transform>().Select(t => t.name).Reverse().ToArray());


    }
}
