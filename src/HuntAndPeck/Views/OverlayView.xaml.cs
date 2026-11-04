using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using HuntAndPeck.Properties;
using HuntAndPeck.ViewModels;

namespace HuntAndPeck.Views
{

    public partial class OverlayView
    {
        public OverlayView()
        {
            InitializeComponent();

            var settings = Settings.Default;
            Resources["ActiveBg"] = HintBrushes.Background(settings.BackgroundColor);
            Resources["InactiveBg"] = HintBrushes.Background(settings.InactiveBackgroundColor);
            Resources["ActiveBorder"] = HintBrushes.Border(settings.BackgroundColor);
            Resources["InactiveBorder"] = HintBrushes.Border(settings.InactiveBackgroundColor);
            HintsControl.Foreground = HintBrushes.Solid(settings.TextColor);
            HintsControl.FontFamily = new FontFamily(settings.FontFamily);
            if (double.TryParse(settings.FontSize, NumberStyles.Float, CultureInfo.InvariantCulture, out var size))
            {
                HintsControl.FontSize = size;
            }
        }

        private void OverlayView_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Drop keys that don't continue any label, so a typo doesn't blank
            // every hint and need a backspace.
            if (!((OverlayViewModel)DataContext).HasPrefix(MatchStringControl.Text + e.Text))
            {
                e.Handled = true;
            }
        }

        private void OverlayView_OnLoaded(object sender, RoutedEventArgs e)
        {
            var m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
            var scaleX = m.M11;
            var scaleY = m.M22;

            layoutGrid.LayoutTransform = new ScaleTransform(1/scaleX, 1/scaleY);

            var vm = DataContext as OverlayViewModel;
            Left = vm.Bounds.Left / scaleX;
            Top = vm.Bounds.Top / scaleY;
            Width = vm.Bounds.Width / scaleX;
            Height = vm.Bounds.Height / scaleY;
        }

        private void OverlayView_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}
