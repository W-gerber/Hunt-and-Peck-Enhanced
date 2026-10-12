using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using HuntAndPeck.ViewModels;

namespace HuntAndPeck.Views
{

    public partial class OverlayView
    {
        public OverlayView()
        {
            InitializeComponent();
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
