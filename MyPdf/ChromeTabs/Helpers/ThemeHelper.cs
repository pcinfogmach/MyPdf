using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ChromeTabs.Helpers
{
    public static class ThemeHelper
    {
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
