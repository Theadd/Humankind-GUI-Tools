using Amplitude.Framework.Overlay;
using System.Threading;
using Amplitude;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.DeveloperTools.UI;
using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public class FramerateToolWindow : FloatingToolWindow
    {
    private bool limitFrameDuration;
    private float targetFrameDuration;
    private float lastFrameDuration;
    private float smoothLastFrameDuration;
    private Stopwatch stopwatch;

    public override string WindowTitle { get; set; } = "FRAMERATE";

    public override string WindowGUIStyle { get; set; } = "PopupWindow";

    public override bool ShouldBeVisible => true;

    public override bool ShouldRestoreLastWindowPosition => true;

    public override Rect WindowRect { get; set; } = new Rect(130f, 260f, 240f, 500f);

    public override void OnGUIStyling()
    {
      base.OnGUIStyling();
      GUI.backgroundColor = new Color32(255, 255, 255, 230);
    }

    public override void OnDrawUI()
    {
      WindowUtils.DrawWindowTitleBar(this);

      OnDrawWindowContent();
    }
    
    protected void OnDrawWindowContent()
    {
      GUILayout.BeginVertical((GUIStyle) "Widget.ClientArea");
      GUILayout.Label(string.Format("Instant: {0:000.00}ms", (object) (float) ((double) this.lastFrameDuration * 1000.0)), (GUIStyle) "PopupWindow.Heading1");
      GUILayout.Label(string.Format("Smooth: {0:000.00}ms", (object) (float) ((double) this.smoothLastFrameDuration * 1000.0)), (GUIStyle) "PopupWindow.Heading1");
      this.limitFrameDuration = GUILayout.Toggle(this.limitFrameDuration, "Limit?");
      if (this.limitFrameDuration)
      {
        GUILayout.Label(string.Format("Wanted: {0:000.00}ms", (object) (float) ((double) this.targetFrameDuration * 1000.0)));
        this.targetFrameDuration = GUILayout.HorizontalSlider(this.targetFrameDuration, 0.008333334f, 0.5f);
      }
      QualitySettings.vSyncCount = GUILayout.Toggle((uint) QualitySettings.vSyncCount > 0U, "VSync") ? 1 : 0;
      GUILayout.EndVertical();
      GUILayout.Space(6f);
    }

    private void OnEnable() => this.targetFrameDuration = 0.016f;

    private void Update()
    {
      if (this.limitFrameDuration)
      {
        float num = (float) (this.stopwatch.ElapsedMilliseconds / 1000.0);
        if ((double) this.targetFrameDuration > (double) num)
          Thread.Sleep(Mathf.RoundToInt((float) (((double) this.targetFrameDuration - (double) num) * 1000.0)));
      }
      this.lastFrameDuration = (float) (this.stopwatch.ElapsedMilliseconds / 1000.0);
      this.smoothLastFrameDuration = Mathf.Lerp(this.smoothLastFrameDuration, this.lastFrameDuration, 1f - Mathf.Exp(-this.lastFrameDuration / 0.5f));
      this.stopwatch.Start();
    }
  }
}
