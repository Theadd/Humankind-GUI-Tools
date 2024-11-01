# **HUMANKIND GUI TOOLS**

**A MOD FOR THE AMPLITUDE STUDIOS <a href="https://humankind.game/" title="Humankind Game Official Website" target="_blank"><sub><sub><img src="https://cdn.sega.co.uk/humankind-game/public/content/media/images/vector/Humankind%20logo.svg" alt="HUMANKIND" height="22px"></sub></sub></a> GAME**

###### **PROJECT PAGE [ON GITHUB](https://github.com/Theadd/Humankind-GUI-Tools)**

<br/>

Primarily intended for mod developers, this modding-tools are a set of panels and windows originally developed by **Amplitude Studios** itself to make the debugging, testing and game development much easier.

They allow you to spawn any unit, anywhere. The same with constructibles, you can paint any district directly on the map in a simple point and click. You can allow the AI to control your empire and watch the game evolve at full speed, only a few seconds per turn. Switch to another empire and take full control. Infinite unit movement, unlock, cancel and complete all civics, technologies, resources, achievements, etc.

And that's not all, it's also a way to bring GUIs to your mods in a few lines of code, and with "live-reloading" by the way, seeing the changes as you write them.

<br/>

### **DOWNLOAD AND INSTALL**

> This mod requires **BepInEx** to be installed in your game directory. If you don't have it, you can follow [this guide](https://gc2021.com/showthread.php?tid=4).

* Download and unpack [Humankind.GUITools.zip](https://github.com/Theadd/Humankind-GUI-Tools/releases/latest/download/Humankind.GUITools.zip) into your game directory.
* Run the game.

<br/>

### **SCREENSHOTS**

![SCREENSHOT](./images/GameOverview.png)

  

![SCREENSHOT](./images/Showcase.png)

<br/>

### **PERSISTENT SETTINGS**

![GAMEMENU](./images/GameMenu.png)

<br/>

### **SUMMARY**

When decompiling the Humankind assemblies back to source code we found **100+ hidden utility tools** used by Amplitude Studios to test, profile and develop the game.

Many of them are **a must have for mod developers** in order to test their mod in many different scenarios since they allow us to advance to a desired game state in no time.

From those 100+ hidden tools we only had the time to test 20-25 of them, of which only ~15 of them are successfully adapted and available right now. There are still many tools to check!

<h5>
<details><summary><strong>NAMES OF THE 100+ HIDDEN TOOLS FOUND</strong></summary>
<blockquote>

```
  LocalizationFloatingWindow, Widget, ClockWidget, MemoryProfilerWindow, ProfilerWindow, 
  FxComponentEvolveProfilingWindow, FxOutputLayerProfilingWindow, GPUProfilerWindow, 
  GraphicsTools, UnityProfilerLoggingWindow, RenderPipelineAuditWindow, AffinityUtils, 
  AI, AICursor, AirStrikeReport, Archetypes, AudioEvents, AudioScale, Avatar, BattleAI, 
  BattleExternalSupport, BattleParticipants, BattleReport, BiomeAudio, BlackList, 
  CameraMover, CameraSequence, ChallengeRewards, ChallengeViewer, Chat, ChoreographyActions, 
  CityAudio, CivicsUtils, Collectible, Diplomacy, DiplomaticLog, DisplaySettings, 
  DistrictPainter, DownloadableContent, Droppables, EffectMapperDebugger, 
  CostModifierEvaluationDebugger, DescriptorEvaluationDebugger, DistrictEvaluationDebugger, 
  EndGameFlow, EndTurnTimer, FameRankingUtility, FameUtils, Framerate, GameClientConnections, 
  GameClientServer, Ideology, InputFilter, MapEditor, MapSharing, Marketing, Metadata, 
  MilitaryCheats, MinorFaction, ModPlaylist, NarrativeEventDebugger, NetworkStatistics, 
  NetworkSynchronization, OrderStatistics, Physics, Ping, PlayerProfile, Pollution, 
  PresentationCursor, RansackCursor, ReligionUtils, RemoteSandboxes, ResourcesUtils, 
  SessionSlots, SessionUsers, SettlementUtils, SnapshotStatistics, Squadrons, 
  StateShareUtils, StatisticsAndAchievements, Surrender, TechnologyUtils, 
  TerrainAudioModule, TerrainPicking, TimemapDebugger, Trade, TradeNodeUtils, Trophies, 
  TutorialFakeWindow, TutorialInfo, TwitchExtension, WorldGeneration, WorldLifeDebugger, 
  CustomMap, ScenarioEditor, BattleDebug, PawnAnimation, Pawns, CameraLayersWindow, 
  ProceduralTerrainRendererWindow, UIVirtualScreenTester, UISettingsOverlayWindow, UIUtils, 
  AudioOptions, AutoTurn, Dump, EmpireWideConstructionUtils, GameInfo, NetworkDebugger, 
  SandboxStatus, TerritoryUtils
```

</blockquote>
</details>
</h5>

<br/>

### **CONTRIBUTING**

Would you like to help? [Check this out](/CONTRIBUTING_GUIDE.md).

<br/>

### **KNOWN ISSUES**

* Not checked recently, but the [Modding.Humankind.DevTools](https://github.com/Theadd/Modding.Humankind.DevTools) libary played along with the [AOM.Humankind.Teams](https://gc2021.com/showthread.php?tid=43) mod was making the game to crash on start.
