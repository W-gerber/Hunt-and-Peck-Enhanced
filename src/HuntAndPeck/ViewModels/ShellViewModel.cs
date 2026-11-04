using System;
using System.Windows.Forms;
using HuntAndPeck.NativeMethods;
using HuntAndPeck.Services.Interfaces;
using Application = System.Windows.Application;

namespace HuntAndPeck.ViewModels
{
    internal class ShellViewModel
    {
        private readonly Action<OverlayViewModel> _showOverlay;
        private readonly Action<DebugOverlayViewModel> _showDebugOverlay;
        private readonly Action<OptionsViewModel> _showOptions;
        private readonly IHintLabelService _hintLabelService;
        private readonly IHintProviderService _hintProviderService;
        private readonly IDebugHintProviderService _debugHintProviderService;
        private readonly IKeyListenerService _keyListener;

        public ShellViewModel(
            Action<OverlayViewModel> showOverlay,
            Action<DebugOverlayViewModel> showDebugOverlay,
            Action<OptionsViewModel> showOptions,
            IHintLabelService hintLabelService,
            IHintProviderService hintProviderService,
            IDebugHintProviderService debugHintProviderService,
            IKeyListenerService keyListener)
        {
            _showOverlay = showOverlay;
            _showDebugOverlay = showDebugOverlay;
            _showOptions = showOptions;
            _hintLabelService = hintLabelService;
            _keyListener = keyListener;
            _hintProviderService = hintProviderService;
            _debugHintProviderService = debugHintProviderService;

            ApplyHotKeys();
            // Options writes straight to Settings, so re-registering here makes
            // hotkey changes take effect without restarting.
            Properties.Settings.Default.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName.Contains("HotKey"))
                {
                    ApplyHotKeys();
                }
            };

#if DEBUG
            keyListener.DebugHotKey = new HotKey
            {
                Keys = Keys.OemSemicolon,
                Modifier = KeyModifier.Alt | KeyModifier.Shift
            };
#endif

            keyListener.OnHotKeyActivated += _keyListener_OnHotKeyActivated;
            keyListener.OnTaskbarHotKeyActivated += _keyListener_OnTaskbarHotKeyActivated;
            keyListener.OnDebugHotKeyActivated += _keyListener_OnDebugHotKeyActivated;

            ShowOptionsCommand = new DelegateCommand(ShowOptions);
            ExitCommand = new DelegateCommand(Exit);
        }

        private static HotKey ReadHotKey(string key, string modifier)
        {
            return new HotKey
            {
                Keys = (Keys)Enum.Parse(typeof(Keys), key),
                Modifier = (KeyModifier)Enum.Parse(typeof(KeyModifier), modifier)
            };
        }

        private void ApplyHotKeys()
        {
            var settings = Properties.Settings.Default;
            _keyListener.HotKey = ReadHotKey(settings.MainHotKeyKey, settings.MainHotKeyModifier);
            _keyListener.TaskbarHotKey = ReadHotKey(settings.TaskbarHotKeyKey, settings.TaskbarHotKeyModifier);
        }

        public DelegateCommand ShowOptionsCommand { get; }
        public DelegateCommand ExitCommand { get; }

        private void _keyListener_OnHotKeyActivated(object sender, EventArgs e)
        {
            var session = _hintProviderService.EnumHints();
            if (session != null && session.Hints.Count > 0)
            {
                var vm = new OverlayViewModel(session, _hintLabelService);
                _showOverlay(vm);
            }
        }

        private void _keyListener_OnTaskbarHotKeyActivated(object sender, EventArgs e)
        {
            var taskbarHWnd = User32.FindWindow("Shell_traywnd", "");
            var session = _hintProviderService.EnumHints(taskbarHWnd);
            if (session != null && session.Hints.Count > 0)
            {
                var vm = new OverlayViewModel(session, _hintLabelService);
                _showOverlay(vm);
            }
        }

        private void _keyListener_OnDebugHotKeyActivated(object sender, EventArgs e)
        {
            var session = _debugHintProviderService.EnumDebugHints();
            if (session != null)
            {
                var vm = new DebugOverlayViewModel(session);
                _showDebugOverlay(vm);
            }
        }

        public void Exit()
        {
            Application.Current.Shutdown();
        }

        public void ShowOptions()
        {
            var vm = new OptionsViewModel(_keyListener);
            _showOptions(vm);
        }
    }
}
