using Amplitude.Framework;
using Amplitude.Framework.Overlay;
using Amplitude.Mercury.Overlay;
using Amplitude.Mercury.Data.Simulation;
using Amplitude.Mercury.Interop;
using Amplitude.Mercury.Runtime;
using Amplitude.Mercury.Sandbox;
using System;
using UnityEngine;
using Modding.Humankind.DevTools;
using Modding.Humankind.DevTools.Core;
using Modding.Humankind.DevTools.DeveloperTools.UI;

namespace DevTools.Humankind.GUITools.UI
{
  public class ResourceToolsWindow : FloatingToolWindow
  {
    private Vector2 scrollPosition;

    protected Amplitude.Framework.Runtime.IRuntimeService RuntimeService { get; private set; }
    
    public override string WindowTitle { get; set; } = "RESOURCE TOOLS";

    public override string WindowGUIStyle { get; set; } = "PopupWindow";

    public override bool ShouldBeVisible => HumankindGame.IsGameLoaded;

    public override bool ShouldRestoreLastWindowPosition => true;

    public override Rect WindowRect { get; set; } = new Rect(130f, 260f, 520f, 500f);

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
      using (new GUILayout.VerticalScope((GUIStyle) "Widget.ClientArea", new GUILayoutOption[1]
      {
        GUILayout.ExpandWidth(true)
      }))
      {
        if (this.RuntimeService != null)
        {
          if (this.RuntimeService.Runtime != null && this.RuntimeService.Runtime.HasBeenLoaded)
          {
            if (this.RuntimeService.Runtime.FiniteStateMachine.CurrentState != null)
            {
              if (this.RuntimeService.Runtime.FiniteStateMachine.CurrentState.GetType() == typeof (RuntimeState_InGame))
              {
                if (Amplitude.Mercury.Presentation.Presentation.HasBeenStarted)
                {
                  using (new GUILayout.VerticalScope(new GUILayoutOption[1]
                  {
                    GUILayout.Height(400f)
                  }))
                  {
                    using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
                    {
                      GUILayout.Label("MONEY");
                      GUILayout.FlexibleSpace();
                      if (GUILayout.Button("<size=11><b>+250</b></size>", (GUIStyle) "PopupWindow.ToolbarButton", GUILayout.Width(56f)))
                        SandboxManager.PostOrder((Order) new OrderGainMoney()
                        {
                          Gain = 250
                        });
                      if (GUILayout.Button("<size=11><b>+5K</b></size>", (GUIStyle) "PopupWindow.ToolbarButton", GUILayout.Width(56f)))
                        SandboxManager.PostOrder((Order) new OrderGainMoney()
                        {
                          Gain = 5000
                        });
                      GUILayout.Space(8f);
                    }
                    using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
                    {
                      GUILayout.Label("Wonder cultural stock".ToUpper());
                      GUILayout.FlexibleSpace();
                      if (GUILayout.Button("<size=11><b>+1000</b></size>", (GUIStyle) "PopupWindow.ToolbarButton", GUILayout.Width(56f)))
                        SandboxManager.PostOrder((Order) new OrderGainInfluence()
                        {
                          Gain = 1000
                        });
                      GUILayout.Space(8f);
                    }
                    using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
                    {
                      GUILayout.Label("Resources".ToUpper());
                      GUILayout.FlexibleSpace();
                      if (GUILayout.Button("<size=11><b>+100</b></size>", (GUIStyle) "PopupWindow.ToolbarButton", GUILayout.Width(80f)))
                        SandboxManager.PostOrder((Order) new OrderGiveGodResource()
                        {
                          ResourceType = ResourceType.Count,
                          GiveGodAccess = true,
                          QuantityOfAccess = 100U
                        });
                      if (GUILayout.Button("<size=10><b>Remove All</b></size>".ToUpper(), (GUIStyle) "PopupWindow.ToolbarButton", GUILayout.Width(90f)))
                        SandboxManager.PostOrder((Order) new OrderGiveGodResource()
                        {
                          ResourceType = ResourceType.Count,
                          GiveGodAccess = false
                        });
                      if (GUILayout.Button("<size=10><b>Discover All</b></size>".ToUpper(), (GUIStyle) "PopupWindow.ToolbarButton", GUILayout.Width(95f)))
                        SandboxManager.PostOrder((Order) new OrderForceDiscoverResource()
                        {
                          ResourceType = ResourceType.Count
                        });
                      if (GUILayout.Button("<size=10><b>Unlock All</b></size>".ToUpper(), (GUIStyle) "PopupWindow.ToolbarButton", GUILayout.Width(90f)))
                        SandboxManager.PostOrder((Order) new OrderForceUnlockResourceTechnology()
                        {
                          ResourceType = ResourceType.Count
                        });
                      GUILayout.Space(8f);
                    }
                    
                    GUILayout.Space(6f);
                    GUILayout.Label("R E S O U R C E S", "PopupWindow.SectionHeader");
                    // <size=11><b>XXX</b></size>
                    this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition);
                    int empireIndex = (int) Snapshots.GameSnapshot.PresentationData.LocalEmpireInfo.EmpireIndex;
                    int length = Snapshots.GameSnapshot.PresentationData.EmpireInfo[empireIndex].ResourceInfo.Length;
                    for (int index = 0; index < length; ++index)
                    {
                      ResourceDefinition resourceDefinition = Amplitude.Mercury.Presentation.Presentation.PresentationUIController.ResourcesDefinition[index];
                      if (!((UnityEngine.Object) resourceDefinition == (UnityEngine.Object) null))
                      {
                        ResourceType resourceType = (ResourceType) index;
                        ResourceInfo resourceInfo = Snapshots.GameSnapshot.PresentationData.EmpireInfo[empireIndex].ResourceInfo[index];
                        using (new GUILayout.HorizontalScope(Array.Empty<GUILayoutOption>()))
                        {
                          string localizedTitle = R.Text.GetLocalizedTitle(resourceDefinition.Name);
                          GUILayout.Label("<size=11><b>" + localizedTitle.ToUpper() + "</b>  <color=#99999988>(" + resourceDefinition.Name + ")</color></size>");
                          GUILayout.FlexibleSpace();
                          GUILayout.Label("<size=11><b>" + ((int) resourceInfo.AccessCount).ToString() + "</b></size>", (GUIStyle) "PopupWindow.MonospacedLabel");
                          if (GUILayout.Button("<size=11><b>+1</b></size>", (GUIStyle) "PopupWindow.ToolbarButton"))
                            SandboxManager.PostOrder((Order) new OrderGiveGodResource()
                            {
                              ResourceType = resourceType,
                              GiveGodAccess = true,
                              QuantityOfAccess = 1U
                            });
                          GUI.enabled = resourceInfo.HaveGodAccess;
                          if (GUILayout.Button("<size=10><b>REMOVE</b></size>", (GUIStyle) "PopupWindow.ToolbarButton"))
                            SandboxManager.PostOrder((Order) new OrderGiveGodResource()
                            {
                              ResourceType = resourceType,
                              GiveGodAccess = false
                            });
                          GUI.enabled = !resourceInfo.IsDiscovered;
                          if (GUILayout.Button("<size=10><b>DISCOVER</b></size>", (GUIStyle) "PopupWindow.ToolbarButton"))
                            SandboxManager.PostOrder((Order) new OrderForceDiscoverResource()
                            {
                              ResourceType = resourceType
                            });
                          GUI.enabled = !resourceInfo.IsExtractionUnlocked;
                          if (GUILayout.Button("<size=10><b>UNLOCK</b></size>", (GUIStyle) "PopupWindow.ToolbarButton"))
                            SandboxManager.PostOrder((Order) new OrderForceUnlockResourceTechnology()
                            {
                              ResourceType = resourceType
                            });
                          GUI.enabled = true;
                        }
                      }
                    }
                    GUILayout.EndScrollView();
                  }
                }
                else
                  GUILayout.Label("Waiting for the presentation...");
              }
              else
                GUILayout.Label("Waiting for the runtime state...");
            }
            else
              GUILayout.Label("Waiting for the runtime...");
          }
          else
            GUILayout.Label("Waiting for the runtime...");
        }
        else
        {
          GUILayout.Label("Waiting for the runtime service...");
          if (Event.current.type != UnityEngine.EventType.Repaint)
            return;
          this.RuntimeService = Services.GetService<Amplitude.Framework.Runtime.IRuntimeService>();
        }
      }
    }
  }
}
