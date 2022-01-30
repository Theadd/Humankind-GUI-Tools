using Amplitude;
using Amplitude.Framework;
using Amplitude.Wwise.Audio;

namespace DevTools.Humankind.GUITools.UI
{
    public static class AudioPlayer
    {
        public static StaticString DistrictPlacement { get; set; } =
            new StaticString("DistrictPlacementCursorClick");
        public static StaticString MoveArmy { get; set; } =
            new StaticString("MoveArmyCursorClick");
        public static StaticString CutForest { get; set; } =
            new StaticString("CutForestCursorClick");
        public static StaticString AttackArmy { get; set; } =
            new StaticString("AttackArmyCursorClick");
        public static StaticString Ransack { get; set; } =
            new StaticString("RansackCursorClick");
        
        public static IAudioModulesService AudioModulesService { get; set; }


        public static void Play(StaticString audioName)
        {
            if (AudioModulesService == null)
                AudioModulesService = Services.GetService<IAudioModulesService>();
            
            AudioModulesService.SendAudioEvent(audioName);
        }
    }
}
