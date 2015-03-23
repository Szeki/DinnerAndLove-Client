using DinnerAndLove.Client.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DinnerAndLove.Client.Wpf
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
    {
        #region Members

        bool _isOpened;

        #endregion

        #region Constructor

        public MainWindow()
		{
			InitializeComponent();

            DataContext = new MainWindowViewModel();
		}

        #endregion

        #region EventHandlers

        private void HeaderDropDownMenu_OnClick(object sender, RoutedEventArgs e)
	    {
            var button = sender as Button;

            if (button == null)
            {
                return;
            }

            if (_isOpened)
            {
                button.ContextMenu.IsEnabled = false;
            }
            else
            {
                _isOpened = true;

                button.ContextMenu.Closed += ContextMenu_OnClosing;
                button.ContextMenu.IsEnabled = true;
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.Placement = PlacementMode.Left;
                button.ContextMenu.HorizontalOffset = button.ActualWidth;
                button.ContextMenu.VerticalOffset = button.ActualHeight;
                button.ContextMenu.IsOpen = true;
            }
	    }

	    private void ContextMenu_OnClosing(object sender, RoutedEventArgs args)
	    {
            _isOpened = false;

            var button = sender as Button;

            if (button == null)
            {
                return;
            }

            button.ContextMenu.Closed -= ContextMenu_OnClosing;
        }

        #endregion
    }
}
