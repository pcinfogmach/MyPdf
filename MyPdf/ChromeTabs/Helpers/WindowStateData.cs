﻿using System.Globalization;
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
                WindowWidth = window.Width,
                WindowHeight = window.Height,
                WindowState = window.WindowState
            };

            if (window.WindowState == WindowState.Normal)
            {
                windowState.WindowTop = window.Top; windowState.WindowLeft = window.Left;
            }

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

            bool isDarkTheme = IsDarkThemeEnabled();
            window.Background = new SolidColorBrush(isDarkTheme ? Color.FromRgb(34, 34, 34) : Colors.White);
            window.Foreground = new SolidColorBrush(isDarkTheme ? Color.FromRgb(200, 200, 200) : Color.FromRgb(30, 30, 30));
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
