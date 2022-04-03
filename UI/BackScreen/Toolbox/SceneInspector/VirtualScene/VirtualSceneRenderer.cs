using System;
using Amplitude.Mercury.Presentation;
using Amplitude.UI;
using Modding.Humankind.DevTools;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI.SceneInspector
{
    public interface IVirtualSceneRenderer
    {
        bool RenderHexOverlayOnWorldPositionableComponents { get; set; }

        bool CaptureOnMouseHover { get; }

        IVirtualSceneRenderer Using(IRenderableScene renderable);

        void OnMouseHoverComponent(VirtualComponent virtualComponent);

        void Finish();
    }

    public class VirtualSceneRenderer : IVirtualSceneRenderer
    {
        // private IRenderableScene _renderable;
        private bool _clearOnRepaint = true;
        public bool RenderHexOverlayOnWorldPositionableComponents { get; set; } = true;
        public bool CaptureOnMouseHover { get; private set; } = false;

        public IVirtualSceneRenderer Using(IRenderableScene renderable)
        {
            var r = GUILayoutUtility.GetAspectRect(float.MaxValue);
            var pointer = Event.current.mousePosition;
            
            CaptureOnMouseHover = RenderHexOverlayOnWorldPositionableComponents
                                  && Event.current.type == EventType.Repaint
                                  // && InGameUIController.IsMouseCovered
                                  && pointer.y >= r.y
                                  && pointer.x >= r.x
                                  && pointer.x <= r.x + r.width;

            if (_clearOnRepaint && Event.current.type == EventType.Repaint)
            {
                SceneInspectorController.HexPainter.IsVisible = false;
                ToolboxController
                    .GetBackScreenUIOverlay()?
                    .SetUIMarkerVisibility(false);
                _clearOnRepaint = false;
            }

            return this;
        }

        public void OnMouseHoverComponent(VirtualComponent virtualComponent)
        {
            var positionable = virtualComponent.Instance.gameObject.GetComponent<IWorldPositionable>();
            if (positionable is PresentationEntity entity)
            {
                SceneInspectorController.HexPainter.IsVisible = true;
                SceneInspectorController.HexPainter.SetNextTileIndex(entity.WorldPosition.ToTileIndex());
                CaptureOnMouseHover = false;
            }
            else
            {
                var uiTransform = virtualComponent.Instance.gameObject.GetComponent<UITransform>();

                if (uiTransform != null)
                {
                    ToolboxController
                        .GetBackScreenUIOverlay()?
                        .SetUIMarkerRect(uiTransform.GlobalRect, false)
                        .SetUIMarkerVisibility(true);
                    CaptureOnMouseHover = false;
                }
            }
        }

        public void Finish()
        {
            SceneInspectorController.HexPainter.Draw();
            _clearOnRepaint = true;
        }
    }
}
