using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace ChromeTabs
{
    internal class ChromeTabsViewModel : INotifyPropertyChanged
    {
        #region InterFace
        public event PropertyChangedEventHandler? PropertyChanged;

        // Helper method to set a property and notify if it changes
        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public ChromeTabsViewModel()
        {
            Application.Current.Exit += async (s, e) =>
                SaveState();
        }

        #region Localization
        private string StateFilePath
        {
            get
            {
                string localeDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Locale");
                string cultureFileName = $"{CultureInfo.CurrentUICulture.TwoLetterISOLanguageName}.json";
                string path = Path.Combine(localeDir, cultureFileName);
                if (File.Exists(path))  return path;

                // Fallback to en
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Locale", "en.json");
                if (File.Exists(path)) return path;

                // Try to find any file that matches a two-letter code format (e.g., "fr.json", "es.json")
                var matchingFile = Directory.EnumerateFiles(localeDir, "*.json")
                                            .FirstOrDefault(file => Path.GetFileName(file).Length == 7);
                if (matchingFile != null) return matchingFile;
                else return path;
            }
        }



        public void SaveState()
        {
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(StateFilePath, json);
        }
        public void LoadState()
        {
            if (File.Exists(StateFilePath))
            {
                string json = File.ReadAllText(StateFilePath);
                ChromeTabsViewModel loadedState = JsonSerializer.Deserialize<ChromeTabsViewModel>(json);

                if (loadedState != null)
                {
                    // Use reflection to assign each property from loadedState to this instance
                    foreach (PropertyInfo property in typeof(ChromeTabsViewModel).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (property.CanWrite)
                        {
                            property.SetValue(this, property.GetValue(loadedState));
                        }
                    }
                }
            }
        }
        #endregion

        #region Members
        private string _maximizeButtonToolTip;
        private string _fullScreenButtonToolTip;
        private string _minimizeButtonToolTip;
        private string _xButtonToolTip;


        public string MinimizeButtonTooltip
        {
            get
            {
                if (_minimizeButtonToolTip == null) LoadState();
                return _minimizeButtonToolTip;
            }
            set => SetProperty(ref _minimizeButtonToolTip, value);
        }
        public string FullScreenButtonTooltip { get => _fullScreenButtonToolTip; set => SetProperty(ref _fullScreenButtonToolTip, value); }
        public string MaximizeButtonTooltip { get => _maximizeButtonToolTip; set => SetProperty(ref _maximizeButtonToolTip, value); }
        public string XButtonTooltip { get => _xButtonToolTip; set => SetProperty(ref _xButtonToolTip, value); }

        #endregion
    }
}
