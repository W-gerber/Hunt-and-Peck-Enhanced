using System.Windows;
using HuntAndPeck.ViewModels;
using System.Linq;
using HuntAndPeck.Services;
using HuntAndPeck.Views;
using HuntAndPeck.NativeMethods;

namespace HuntAndPeck
{

    public partial class App : Application
    {
        private readonly SingleLaunchMutex _singleLaunchMutex = new SingleLaunchMutex();
        private readonly UiAutomationHintProviderService _hintProviderService = new UiAutomationHintProviderService();
        private readonly HintLabelService _hintLabelService = new HintLabelService();
        private KeyListenerService _keyListenerService;

        private void ShowOverlay(OverlayViewModel vm)
        {
            var view = new OverlayView
            {
                DataContext = vm
            };
            vm.CloseOverlay = () => view.Close();
            view.ShowDialog();
        }

        private void ShowDebugOverlay(DebugOverlayViewModel vm)
        {
            var view = new DebugOverlayView
            {
                DataContext = vm
            };
            view.ShowDialog();
        }

        private void ShowOptions(OptionsViewModel vm)
        {
            var view = new OptionsView
            {
                DataContext = vm
            };
            view.ShowDialog();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Contains("/hint"))
            {

                var session = _hintProviderService.EnumHints();
                var overlayWindow = new OverlayView()
                {
                    DataContext = new OverlayViewModel(session, _hintLabelService)
                };
                overlayWindow.Show();
            }
            else if (e.Args.Contains("/tray"))
            {

                var taskbarHWnd = User32.FindWindow("Shell_traywnd", "");
                var session = _hintProviderService.EnumHints(taskbarHWnd);
                var overlayWindow = new OverlayView()
                {
                    DataContext = new OverlayViewModel(session, _hintLabelService)
                };
                overlayWindow.Show();
            }
            else
            {

                if (_singleLaunchMutex.AlreadyRunning)
                {
                    Current.Shutdown();
                    return;
                }

                _keyListenerService = new KeyListenerService();

                var shellViewModel = new ShellViewModel(
                    ShowOverlay,
                    ShowDebugOverlay,
                    ShowOptions,
                    _hintLabelService,
                    _hintProviderService,
                    _hintProviderService,
                    _keyListenerService);

                var shellView = new ShellView
                {
                    DataContext = shellViewModel
                };
                shellView.Show();
            }
            base.OnStartup(e);
        }
    }
}
