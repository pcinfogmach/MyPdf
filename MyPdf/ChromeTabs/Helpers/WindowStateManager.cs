using MyPdf.Assets;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Media;

namespace ChromeTabs.Helpers
{
    public static class WindowStateManager
    {
        class WindowStateModel
        {
            public double? WindowTop { get; set; }
            public double? WindowLeft { get; set; }
            public double? WindowWidth { get; set; }
            public double? WindowHeight { get; set; }
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public WindowState WindowState { get; set; }
        }

        static string windowStatePath = "chromeTabsWindowState.json";

        public async static void SaveState(Window window)
        {
            var windowState = new WindowStateModel
            {
                WindowWidth = window.Width,
                WindowHeight = window.Height,
                WindowState = window.WindowState
            };

            if (window.WindowState == WindowState.Normal)
            {
                windowState.WindowTop = window.Top; windowState.WindowLeft = window.Left;
            }

            await Task.Run(async () =>
            {
                string json = JsonSerializer.Serialize(windowState, new JsonSerializerOptions { WriteIndented = true });
                await AssetsManager.WriteAssetAsync(windowStatePath, json);
            });
        }

        public async static void LoadState(Window window)
        {
            string json = await AssetsManager.GetAssetAsync(windowStatePath);
            if (!string.IsNullOrEmpty(json))
            {
                var windowState = JsonSerializer.Deserialize<WindowStateModel>(json);

                if (windowState != null)
                {
                    window.Top = windowState.WindowTop ?? window.Top;
                    window.Left = windowState.WindowLeft ?? window.Left;
                    window.Width = windowState.WindowWidth ?? window.Width;
                    window.Height = windowState.WindowHeight ?? window.Height;
                    window.WindowState = windowState.WindowState;
                }
            }


            bool isDarkTheme = IsDarkThemeEnabled();
            window.Background = isDarkTheme ? new SolidColorBrush(Color.FromRgb(34, 34, 34)) : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF9F9FA"));
            window.Foreground = new SolidColorBrush(isDarkTheme ? Color.FromRgb(200, 200, 200) : Colors.DarkSlateGray);
            window.FlowDirection = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "he" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        }

        public static bool IsDarkThemeEnabled()
        {
            const string registryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            const string registryValueName = "AppsUseLightTheme";

            using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(registryKeyPath))
            {
                if (key?.GetValue(registryValueName) is int value)
                {
                    return value == 0; // 0 means Dark Theme is enabled, 1 means Light Theme
                }
            }
            return false; // Default to light theme if the registry value is not found
        }
    }
}
