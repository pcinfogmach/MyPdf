using ChromeTabs.Helpers;
using MyPdf.HistoryAndUserTags;
using System.Diagnostics;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shell;

namespace ChromeTabs
{
    /// <summary>
    /// Interaction logic for ChromeTabsWindow.xaml
    /// </summary>
    public partial class ChromeTabsWindow : Window
    {
        public HistoryLogger HistoryLogger { get; } = new HistoryLogger();

        public ChromeTabsWindow()
        {
            InitializeComponent();

            localeViewModel.LoadState();
            WindowStateData.LoadState(this);
            this.Closing += (s, e) => WindowStateData.SaveState(this);
        }

        private void TitleBarGrid_TouchDown(object sender, TouchEventArgs e) => DragMove();

        private void TitleBarGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            else if (e.LeftButton == MouseButtonState.Pressed)  
                DragMove();
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
            if (!(sender is ListBoxItem listBoxItem) || e.OriginalSource is Button)
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
            ToggleFullScreen();
        }

       

        private void ChromeTabStrip_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            else if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                if (e.Key == Key.W)
                {
                    int selectedIndex = ChromeTabControl.SelectedIndex;
                    ChromeTabControl.Items.Remove(ChromeTabControl.SelectedItem);
                    ChromeTabControl.SelectedIndex = selectedIndex >= ChromeTabControl.Items.Count ? selectedIndex - 1 : selectedIndex;
                    e.Handled = true;
                }
                else if (e.Key == Key.X)
                {
                    var tabItems = ChromeTabControl.Items.Cast<TabItem>().ToList();
                    foreach (var item in tabItems)
                        ChromeTabControl.Items.Remove(item);
                    e.Handled = true;
                }
                else if (e.Key == Key.Tab)
                {
                    ChromeTabControl.SelectedIndex = ChromeTabControl.SelectedIndex >= ChromeTabControl.Items.Count - 1 ? 0 : ChromeTabControl.SelectedIndex + 1;
                    e.Handled = true;
                }
                else if (e.Key == Key.O)
                {
                    OpenFile();
                    e.Handled = true;
                }
                else if (e.Key == Key.S)
                {
                    SaveFile();
                    e.Handled = true;
                }
                else if (e.Key == Key.S && Keyboard.Modifiers == (ModifierKeys.Shift))
                {
                    SaveFileAS();
                    e.Handled = true;
                }
                else if (e.Key == Key.S && Keyboard.Modifiers == (ModifierKeys.Alt))
                {
                    CaptureScreen();
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.F10 || e.Device is KeyboardDevice keyboardDeviceF10 && keyboardDeviceF10.IsKeyDown(Key.F10))
            {
                ToggleSideBar();
                e.Handled = true;
            }
            else if (e.Key == Key.F12 || e.Device is KeyboardDevice keyboardDeviceF12 && keyboardDeviceF12.IsKeyDown(Key.F12))
            {
                SaveFileAS();
                e.Handled = true;
            }
            else if (e.Key == Key.F11 || (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Alt) || e.Device is KeyboardDevice keyboardDeviceF11 && keyboardDeviceF11.IsKeyDown(Key.F11))
            {
                ToggleFullScreen();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                ExitFullScreen();
                e.Handled = true;
            }
        }

        #region SideBar
        private void ToggleSideBarButton_Click(object sender, RoutedEventArgs e) => ToggleSideBar();

        private void SidePanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SidePanel.SelectedItem == OpenFile_PanelButton) OpenFile();
            else if (SidePanel.SelectedItem == ScreenCapture_PanelButton) CaptureScreen();
            else if (SidePanel.SelectedItem == Help_PanelButton) ShowHelp();
            else if (SidePanel.SelectedItem == WebSite_PanelButton) { Process.Start(new ProcessStartInfo { FileName = "https://mitmachim.top/post/869071", UseShellExecute = true });}
        }
        #endregion

        #region methods
        public virtual void OpenFile() { OpenFile_PanelButton.IsSelected = false; }
        public virtual void SaveFile() { }
        public virtual void SaveFileAS() { }
        public virtual void ShowHelp() { Help_PanelButton.IsSelected = false; }
        void CaptureScreen()
        {
            var captureWindow = new ScreenCaptureLib.ScreenCaptureWindow(false)
            {
                WindowState = this.WindowState,
                Height = this.ActualHeight,
                Width = this.ActualWidth,
                Left = this.Left,
                Top = this.Top,
                Owner = this,
            };
            captureWindow.Show();
            ScreenCapture_PanelButton.IsSelected = false;
        }
        void ToggleFullScreen()
        {
            if (this.WindowStyle == WindowStyle.None) { ExitFullScreen();  return; }
            this.WindowStyle = WindowStyle.None;
            this.WindowState = WindowState.Normal;
            this.WindowState = WindowState.Maximized;
            WindowChrome.SetWindowChrome(this, new WindowChrome { CaptionHeight = 0});
            TitleBarGrid.Visibility = Visibility.Collapsed;
        }
        void ExitFullScreen()
        {
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.WindowState = WindowState.Normal;

            var windowChrome = new WindowChrome
            {
                CornerRadius = SystemParameters.WindowCornerRadius, // Setting the CornerRadius
                ResizeBorderThickness = SystemParameters.WindowResizeBorderThickness, // Setting the Resize Border Thickness
                UseAeroCaptionButtons = false, 
                CaptionHeight = TitleBarGrid.ActualHeight
            };
            WindowChrome.SetWindowChrome(this, windowChrome);

            TitleBarGrid.Visibility = Visibility.Visible;
        }
        void ToggleSideBar()
        {
            SidePanel.Visibility = (SidePanel.Visibility == Visibility.Collapsed) ? Visibility.Visible : Visibility.Collapsed;
            SidePanelHostTabControl.Visibility = (SidePanelHostTabControl.Visibility == Visibility.Collapsed) ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion
    }
}
