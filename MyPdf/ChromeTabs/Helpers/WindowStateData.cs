using System;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Media;

namespace ChromeTabs.Helpers
{
    public static class WindowStateData
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

        static string windowStatePath
        {
            get
            {
                string assetsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
                if (!Directory.Exists(assetsPath)) Directory.CreateDirectory(assetsPath);
                return Path.Combine(assetsPath, "windowState.json");
            }           
        }

        public async static void SaveState(Window window)
        {
            var windowState = new WindowStateModel
            {
                WindowTop = window.Top,
                WindowLeft = window.Left,
                WindowWidth = window.Width,
                WindowHeight = window.Height,
                WindowState = window.WindowState
            };

            await Task.Run(() =>
            {
                string json = JsonSerializer.Serialize(windowState, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(windowStatePath, json);
            });
        }

        public static void LoadState(Window window)
        {
            if (File.Exists(windowStatePath))
            {
                string json = File.ReadAllText(windowStatePath);
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

            bool isDarkTheme = ThemeHelper.IsDarkThemeEnabled();
            window.Background = new SolidColorBrush(isDarkTheme ? Color.FromRgb(30, 30, 30) : Color.FromRgb(200, 200, 200));
            window.Foreground = new SolidColorBrush(isDarkTheme ? Color.FromRgb(200, 200, 200) : Color.FromRgb(30, 30, 30));
            window.FlowDirection = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "he" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        }
    }
}
