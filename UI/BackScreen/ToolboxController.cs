﻿using System;
using System.Linq;
using Amplitude.Framework;
using Amplitude.Mercury.Data;
using Amplitude.Mercury.Input;
using BepInEx.Configuration;
using Modding.Humankind.DevTools;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public static class ToolboxController
    {
        public static Rect ToolboxRect { get; private set; } = Rect.zero;
        public static Rect InputFilterRect { get; private set; } = Rect.zero;
        public static BackScreenToolbox Toolbox { get; set; }
        public static KeyboardShortcut ToolboxPreviewKey { get; set; } = new KeyboardShortcut(KeyCode.LeftControl);
        public static KeyboardShortcut StickedToolboxKey { get; set; } = new KeyboardShortcut(KeyCode.Space, KeyCode.LeftControl);
        public static bool IsVisible { get; private set; } = false;
        public static bool IsSticked { get; set; } = true;
        public static string InputFilter { get; set; } = string.Empty;
        
        // IInputService
        
        private static IInputFilterService inputFilterService;
        private static int inputFilterHandle = -1;

        public static void Initialize(BackScreenWindow window)
        {
            if (ToolboxRect.Equals(Rect.zero))
                SetToolboxRect(new Rect(300f, 300f, 300f, 300f));
            
            ConstructibleStore.Rebuild();
            
            Toolbox = new BackScreenToolbox()
            {
                Window = window
            };
            
            CreateInputFilter();
        }

        public static void SetToolboxRect(Rect newRect)
        {
            ToolboxRect = newRect;
            InputFilterRect = new Rect(newRect.x + 12f, newRect.y - 24f, newRect.width, 24f);
        }

        public static bool Draw()
        {
            if (Event.current.type == EventType.Repaint)
            {
                if (StickedToolboxKey.IsDown())
                {
                    IsSticked = !IsSticked;
                }
            }
            
            if (IsSticked || AreKeysHeldDown(ToolboxPreviewKey))
            {
                if (!IsVisible)
                    OnShow();
                
                if (InputFilter.Length > 0)
                    DrawInputFilter();
                
                Toolbox.Draw(ToolboxRect);
                
                if (Event.current.type == EventType.Repaint && Input.inputString.Length > 0)
                    InputFilter = (InputFilter + Input.inputString).TrimStart(' ');
                
                return true;
            }

            InputFilter = string.Empty;
            if (IsVisible)
                OnHide();
            
            return false;
        }

        private static void DrawInputFilter()
        {
            GUILayout.BeginArea(InputFilterRect);
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("<color=#000000FF><b><size=10>FILTER: </size></b></color>");
                    GUILayout.Label("<color=#000000FF><b>" + InputFilter.ToUpper() + "</b></color>");
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
        }

        private static bool AreKeysHeldDown(KeyboardShortcut shortcut) => 
            Input.GetKey(shortcut.MainKey) && shortcut.Modifiers.Count(key => !Input.GetKey(key)) == 0;

        private static void CreateInputFilter()
        {
            if (inputFilterService == null)
                inputFilterService = Services.GetService<IInputFilterService>();
            
            inputFilterHandle = inputFilterService.CreateFilter(
                InputFilterDeviceMask.Keyboard 
                    | InputFilterDeviceMask.Gamepad 
                    | InputFilterDeviceMask.Mouse 
                    | InputFilterDeviceMask.Touch, 
                InputLayer.InputFilter.PauseMenuModalWindow.Group, 
                InputLayer.InputFilter.PauseMenuModalWindow.Priority, 
                false);
        }
        
        private static void OnShow()
        {
            IsVisible = true;
            inputFilterService.SetFilterActive(inputFilterHandle, true);
        }

        private static void OnHide()
        {
            IsVisible = false;
            inputFilterHandle = inputFilterService.DestroyFilter(inputFilterHandle);
            CreateInputFilter();
        }
    }
}
