using System.Windows;
using System.Windows.Media;
using HuntAndPeck.ViewModels;

namespace HuntAndPeck.Views
{

    public partial class DebugOverlayView
    {
        public DebugOverlayView()
        {
            InitializeComponent();
        }

        private void DebugOverlayView_OnLoaded(object sender, RoutedEventArgs e)
        {
            var m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
            var scaleX = m.M11;
            var scaleY = m.M22;

            layoutGrid.LayoutTransform = new ScaleTransform(1/scaleX, 1/scaleY);

            var vm = DataContext as DebugOverlayViewModel;
            Left = vm.Bounds.Left / scaleX;
            Top = vm.Bounds.Top / scaleY;
            Width = vm.Bounds.Width / scaleX;
            Height = vm.Bounds.Height / scaleY;
        }
    }
}
