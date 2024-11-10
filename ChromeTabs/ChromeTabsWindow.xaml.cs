using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChromeTabs
{
    /// <summary>
    /// Interaction logic for ChromeTabsWindow.xaml
    /// </summary>
    public partial class ChromeTabsWindow : Window
    {
        public ChromeTabsWindow()
        {
            ////ThemeHelper.ApplyTheme(this);
            InitializeComponent();
        }

        private void ScrollLeft_Click(object sender, RoutedEventArgs e)
        {
            if (ChromeTabStrip.SelectedIndex > 0) ChromeTabStrip.SelectedIndex--;
        }

        private void ScrollRight_Click(object sender, RoutedEventArgs e)
        {
            if (ChromeTabStrip.SelectedIndex < ChromeTabStrip.Items.Count) ChromeTabStrip.SelectedIndex++;
        }

        private void DropDown_Opened(object sender, EventArgs e)
        {
            DropDownListBox.ItemsSource = ChromeTabStrip.Items.Cast<TabItem>().Select(t => t.Header);
        }

        private void DropDownListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DropDownListBox.Items.Count == 0) { return; }
            ChromeTabStrip.SelectedIndex = DropDownListBox.SelectedIndex;
            DropDown.IsOpen = false;
        }

        private void ChromeTab_XButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button &&
                    button.Tag is TabItem tabItem &&
                        tabItem.Parent is TabControl tabControl)
            {
                int selectedIndex = tabControl.SelectedIndex;
                tabControl.Items.Remove(tabItem);
                tabControl.SelectedIndex = selectedIndex >= tabControl.Items.Count ? selectedIndex - 1 : selectedIndex;
            }
                           
        }



        private void ListBoxItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!(sender is ListBoxItem listBoxItem))
            {
                return;
            }

            if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(listBoxItem, listBoxItem, DragDropEffects.All);
            }
        }

        private void ListBoxItem_Drop(object sender, DragEventArgs e)
        {
            if (sender is ListBoxItem listBoxItemTarget &&
                e.Data.GetData(typeof(ListBoxItem)) is ListBoxItem listBoxItemSource &&
                listBoxItemTarget.DataContext is TabItem tabItemTarget &&
                listBoxItemSource.DataContext is TabItem tabItemSource &&
                !tabItemTarget.Equals(tabItemSource) &&
                tabItemTarget.Parent is TabControl tabControl)
            {
                int targetIndex = tabControl.Items.IndexOf(tabItemTarget);

                tabControl.Items.Remove(tabItemSource);
                tabControl.Items.Insert(targetIndex, tabItemSource);
                tabControl.SelectedIndex = targetIndex;
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void maximizeRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyFullScreen();
        }

        void ApplyFullScreen()
        {
            this.WindowStyle = WindowStyle.None;
            this.WindowState = WindowState.Normal;
            this.WindowState = WindowState.Maximized;
            TitleBarGrid.Visibility = Visibility.Collapsed;
            this.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == Key.Escape)
                {
                    this.WindowStyle = WindowStyle.SingleBorderWindow;
                    this.WindowState = WindowState.Normal;
                    TitleBarGrid.Visibility = Visibility.Visible;
                }
            };
        }

        private void window_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                if (e.Key == Key.W)
                {
                    int selectedIndex = ChromeTabControl.SelectedIndex;
                    ChromeTabControl.Items.Remove(ChromeTabControl.SelectedItem);
                    ChromeTabControl.SelectedIndex = selectedIndex >= ChromeTabControl.Items.Count ? selectedIndex - 1 : selectedIndex;
                }
                else if (e.Key == Key.X)
                {
                    var tabItems = ChromeTabControl.Items.Cast<TabItem>().ToList();
                    foreach (var item in tabItems)
                        ChromeTabControl.Items.Remove(item);
                }
                else if (e.Key == Key.Tab)
                {
                    ChromeTabControl.SelectedIndex = ChromeTabControl.SelectedIndex >= ChromeTabControl.Items.Count - 1 ? 0 : ChromeTabControl.SelectedIndex + 1;
                }
                e.Handled = true;
            }
        }
    }
}
