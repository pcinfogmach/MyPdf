using ChromeTabs.Helpers;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;

namespace ChromeTabs
{
    internal class LocaleViewModel : INotifyPropertyChanged
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

        #region Serialization
        //public async void SaveState()
        //{
        //    await Task.Run(() =>
        //    {
        //        string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        //        File.WriteAllText(StateFilePath(), json);
        //    });
        //}

        public void LoadState()
        {
            string stateFilePath = StateFilePath();
            if (File.Exists(stateFilePath))
            {
                string json = File.ReadAllText(stateFilePath);
                LocaleViewModel loadedState = JsonSerializer.Deserialize<LocaleViewModel>(json);

                if (loadedState != null)
                {
                    // Use reflection to assign each property from loadedState to this instance
                    foreach (PropertyInfo property in typeof(LocaleViewModel).GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (property.CanWrite)
                            property.SetValue(this, property.GetValue(loadedState));
                    }
                }
            }
        }

        private string StateFilePath()
        {
            string localeDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ChromeTabs", "Locale");
            string cultureInfo = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            string path = Path.Combine(localeDir, $"{cultureInfo}.json");
            if (File.Exists(path)) return path;

            // Fallback to en
            path = Path.Combine(localeDir, "en.json");
            if (File.Exists(path)) return path;

            // Try to find any file that matches a two-letter code format (e.g., "fr.json", "es.json")
            var matchingFile = Directory.EnumerateFiles(localeDir, "*.json")
                                        .FirstOrDefault(file => Path.GetFileName(file).Length == 7);

            if (matchingFile != null) return matchingFile;
            else return string.Empty;
        }
        #endregion

        #region Members
        private string _maximizeButtonToolTip;
        private string _fullScreenButtonToolTip;
        private string _minimizeButtonToolTip;
        private string _xButtonToolTip;
        private string _toggleSideBarButtonToolTip;
        private string _screenCaptureButtonToolTip;
        private string _openFilePanelButtonTooltip;
        private string _historyPanelButtonTooltip;
        private string _tagsPanelButtonTooltip;
        private string _searchDataPanelButtonTooltip;
        private string _screenCapturePanelButtonTooltip;
        private string _helpPanelButtonTooltip;
        private string _webSitePanelButtonTooltip;
        private string _addUserTagButtonToolTip;
        private string _editUserTagButtonToolTip;
        private string _addUserTagGroupButtonToolTip;
        private string _deleteUserTagButtonToolTip;
        private string _extractTextPanelButtonTooltip;

        public string MinimizeButtonTooltip { get => _minimizeButtonToolTip; set => SetProperty(ref _minimizeButtonToolTip, value); }
        public string FullScreenButtonTooltip { get => _fullScreenButtonToolTip; set => SetProperty(ref _fullScreenButtonToolTip, value); }
        public string MaximizeButtonTooltip { get => _maximizeButtonToolTip; set => SetProperty(ref _maximizeButtonToolTip, value); }
        public string XButtonTooltip { get => _xButtonToolTip; set => SetProperty(ref _xButtonToolTip, value); }
        public string ToggleSideBarButtonTooltip { get => _toggleSideBarButtonToolTip; set => SetProperty(ref _toggleSideBarButtonToolTip, value); }
        public string ScreenCaptureButtonTooltip { get => _screenCaptureButtonToolTip; set => SetProperty(ref _screenCaptureButtonToolTip, value); }
        public string OpenFilePanelButtonTooltip { get => _openFilePanelButtonTooltip; set => SetProperty(ref _openFilePanelButtonTooltip, value); }
        public string HistoryPanelButtonTooltip { get => _historyPanelButtonTooltip; set => SetProperty(ref _historyPanelButtonTooltip, value); }
        public string TagsPanelButtonTooltip { get => _tagsPanelButtonTooltip; set => SetProperty(ref _tagsPanelButtonTooltip, value); }
        public string SearchDataPanelButtonTooltip { get => _searchDataPanelButtonTooltip; set => SetProperty(ref _searchDataPanelButtonTooltip, value); }
        public string ScreenCapturePanelButtonTooltip { get => _screenCapturePanelButtonTooltip; set => SetProperty(ref _screenCapturePanelButtonTooltip, value); }
        public string ExtractTextPanelButtonTooltip { get => _extractTextPanelButtonTooltip; set => SetProperty(ref _extractTextPanelButtonTooltip, value); }
        public string HelpPanelButtonTooltip { get => _helpPanelButtonTooltip; set => SetProperty(ref _helpPanelButtonTooltip, value); }
        public string WebSitePanelButtonTooltip { get => _webSitePanelButtonTooltip; set => SetProperty(ref _webSitePanelButtonTooltip, value); }
        public string AddUserTagButtonToolTip { get => _addUserTagButtonToolTip; set => SetProperty(ref _addUserTagButtonToolTip, value); }
        public string EditUserTagButtonToolTip { get => _editUserTagButtonToolTip; set => SetProperty(ref _editUserTagButtonToolTip, value); }
        public string AddUserTagGroupButtonToolTip { get => _addUserTagGroupButtonToolTip; set => SetProperty(ref _addUserTagGroupButtonToolTip, value); }
        public string DeleteUserTagButtonToolTip { get => _deleteUserTagButtonToolTip; set => SetProperty(ref _deleteUserTagButtonToolTip, value); }
       
        

        #endregion

        #region Commands
        public ICommand LoadStateCommand { get => new RelayCommand(LoadState); }
        //public ICommand SaveStateCommand { get => new RelayCommand(SaveState); }
        #endregion
    }
}
