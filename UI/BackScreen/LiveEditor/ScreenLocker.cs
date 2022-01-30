using Amplitude.Framework;
using Amplitude.Mercury.Data;
using Amplitude.Mercury.Input;

namespace DevTools.Humankind.GUITools.UI
{
    public static class ScreenLocker
    {
        private static IInputFilterService InputFilterService { get; set; }
        private static int InputFilterHandle { get; set; } = -1;
        private static bool _isFilterOnInputDevicesEnabled = false;

        public static void Unlock()
        {
            LockFullScreenMouseEvents = false;
            EnableFilterOnInputDevices = false;
        }

        public static bool LockFullScreenMouseEvents
        {
            get => MainTools.BackScreen.ScreenOverlay.LockFullScreenMouseEvents;
            set => MainTools.BackScreen.ScreenOverlay.LockFullScreenMouseEvents = value;
        }

        public static bool EnableFilterOnInputDevices
        {
            get => _isFilterOnInputDevicesEnabled;
            set
            {
                if (InputFilterService == null)
                    CreateInputFilter();

                if (_isFilterOnInputDevicesEnabled == value || InputFilterService == null)
                    return;

                _isFilterOnInputDevicesEnabled = value;

                InputFilterService.SetFilterActive(InputFilterHandle, _isFilterOnInputDevicesEnabled);
            }
        }
        
        private static void CreateInputFilter()
        {
            if (InputFilterService == null)
                InputFilterService = Services.GetService<IInputFilterService>();
            
            InputFilterHandle = InputFilterService.CreateFilter(
                InputFilterDeviceMask.Keyboard 
                | InputFilterDeviceMask.Gamepad 
                | InputFilterDeviceMask.Mouse 
                | InputFilterDeviceMask.Touch, 
                InputLayer.InputFilter.PauseMenuModalWindow.Group, 
                InputLayer.InputFilter.PauseMenuModalWindow.Priority, 
                false);
        }

        public static void Unload()
        {
            Unlock();
            InputFilterHandle = InputFilterService.DestroyFilter(InputFilterHandle);
        }
    }
}