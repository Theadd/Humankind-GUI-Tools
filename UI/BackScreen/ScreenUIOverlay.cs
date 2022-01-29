using Amplitude.Framework.Overlay;
using Amplitude.Mercury.UI;
using Amplitude.UI;
using Amplitude.UI.Interactables;
using Modding.Humankind.DevTools;
using UnityEngine;
using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI
{
    [RequireComponent(typeof(UITransform))]
    [RequireComponent(typeof(UIButton))]
    public class ScreenUIOverlay : MonoBehaviour
    {
        public UIButton Control { get; protected set; }
        public UIButton InnerControl { get; protected set; }
        public Color BackgroundColor { get; set; } = new Color(0, 0, 0, 0.4f);
        public UITransform UITransform { get; protected set; }
        public UITransform InnerUITransform { get; protected set; }
        public SquircleBackgroundWidget InnerCanvas { get; protected set; }
        public SquircleBackgroundWidget Canvas { get; protected set; }
        public bool IsVisibleSelf { get; private set; } = true;
        private static GameObject _container = null;
        public static GameObject Container
        {
            get
            {
                if (_container == null)
                {
                    var parentContainer = GameObject.Find("/WindowsRoot/SystemOverlays");
                    var count = parentContainer.transform.childCount;
                    
                    for (int i = count - 1; i >= 0; i--)
                    {
                        var child = parentContainer.transform.GetChild(i);

                        if (child.name == "ScreenUIOverlayContainer") {
                            Destroy(child.gameObject);
                        }
                    }
                    
                    _container = new GameObject("ScreenUIOverlayContainer");
                    _container.transform.parent = parentContainer != null ? parentContainer.transform : null;
                    _container.AddComponent<UITransform>();
                }

                return _container;
            }
        }
        
        public bool LockFullScreenMouseEvents
        {
            get => Control?.enabled ?? false;
            set => SetLockFullScreenMouseEvents(value);
        }
        
        public bool EnableFullScreenBackgroundColor
        {
            get => Canvas?.enabled ?? false;
            set => SetEnableFullScreenBackgroundColor(value);
        }
        

        public static ScreenUIOverlay Find(string uuid)
        {
            var t = Container.transform.Find(uuid);

            return (t == null) ? Create(uuid) : t.GetComponent<ScreenUIOverlay>();
        }

        public Rect ApplyRelativeResolution(Rect rect)
        {
            Rect uiRect = UITransform.Parent.Parent.GlobalRect;
            
            //Loggr.Log(rect);
            //Loggr.Log(uiRect);
            // Loggr.Log("Screen: " + Screen.width + " , " + Screen.height);
            //Loggr.Log(UITransform.Parent.Parent);

            return new Rect(
                (uiRect.width * rect.x) / Screen.width,
                (uiRect.height * rect.y) / Screen.height,
                (uiRect.width * rect.width) / Screen.width,
                (uiRect.height * rect.height) / Screen.height
            );
        }
        // 0 0 2400 1500    = uiRect
        // 300 300 300 300  = rect
        // 1440 900         = Screen
        
        // 500 500 
        

        public ScreenUIOverlay FullScreenSync()
        {
            var rect = ApplyRelativeResolution(new Rect(0, 0, Screen.width, Screen.height));
            
            UITransform.X = rect.x;
            UITransform.Y = rect.y;
            UITransform.Width = rect.width;
            UITransform.Height = rect.height;
            UITransform.VisibleSelf = true;
            IsVisibleSelf = true;

            return this;
        }

        protected static ScreenUIOverlay Create(string uuid)
        {
            var go = new GameObject(uuid);
            go.transform.parent = Container.transform;
            var overlay = go.AddComponent<ScreenUIOverlay>();
            overlay.Setup();

            return overlay;
        }

        public void Setup()
        {
            UITransform = GetComponent<UITransform>();
            Control = GetComponent<UIButton>();
            Control.LoadIfNecessary();
            Control.enabled = false;

            if (gameObject.GetComponent<SquircleBackgroundWidget>() == null)
            {
                Canvas = gameObject.AddComponent<SquircleBackgroundWidget>();
                Canvas.BackgroundColor = BackgroundColor;
                Canvas.OuterBorderColor = Color.green;
                Canvas.BorderColor = Color.clear;
                Canvas.CornerRadius = 0f;
                Canvas.enabled = false;
            }

            var go = new GameObject("InnerOverlayRect");
            go.transform.parent = gameObject.transform;
            InnerUITransform = go.AddComponent<UITransform>();
            InnerControl = go.AddComponent<UIButton>();
            InnerControl.LoadIfNecessary();
            
            InnerCanvas = go.AddComponent<SquircleBackgroundWidget>();
            InnerCanvas.BackgroundColor = BackgroundColor;
            InnerCanvas.OuterBorderColor = Color.clear;
            InnerCanvas.BorderColor = Color.clear;
            InnerCanvas.CornerRadius = 7f;
        }

        public ScreenUIOverlay SetInnerRect(Rect innerRect)
        {
            if (InnerUITransform == null)
                return this;
            
            var rect = ApplyRelativeResolution(innerRect);
            
            InnerUITransform.X = rect.x;
            InnerUITransform.Y = rect.y;
            InnerUITransform.Width = rect.width;
            InnerUITransform.Height = rect.height;

            return this;
        }

        public ScreenUIOverlay SetInnerRectAsVisible(bool shouldBeVisible)
        {
            if (InnerUITransform == null)
                return this;
            
            InnerUITransform.VisibleSelf = shouldBeVisible;

            return this;
        }
        
        public bool SetLockFullScreenMouseEvents(bool shouldBeLocked = true) => 
            Control != null && (Control.enabled = shouldBeLocked);

        public bool SetEnableFullScreenBackgroundColor(bool shouldBeEnabled = true) =>
            Canvas != null && (Canvas.enabled = shouldBeEnabled);

        public static void Unload()
        {
            var c = GameObject.Find("/WindowsRoot/SystemOverlays/ScreenUIOverlayContainer");
            
            if (c != null)
            {
                c.transform.parent = null;
                Destroy(c);
            }
        }
    }
}
