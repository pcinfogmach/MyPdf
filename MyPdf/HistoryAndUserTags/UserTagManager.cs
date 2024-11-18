using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace MyPdf.HistoryAndUserTags
{
    public class UserTagManager : INotifyPropertyChanged
    {
        private readonly string _saveFilePath;
        private ObservableCollection<UserTagItem> _tags;

        public ObservableCollection<UserTagItem> Tags
        {
            get => _tags;
            private set
            {
                if (_tags != value)
                {
                    _tags = value;
                    OnPropertyChanged();
                }
            }
        }

        public UserTagManager(string saveFilePath)
        {
            _saveFilePath = saveFilePath;
            Tags = new ObservableCollection<UserTagItem>();
            LoadTags();
            Tags.CollectionChanged += (_, __) => SaveTags(); // Auto-save on collection changes
        }

        public void AddTag(string fileName, string filePath, int pageNumber)
        {
            if (Tags.Any(tag => tag.Path == filePath && tag.PageNumber == pageNumber))
                return;

            Tags.Add(new UserTagItem
            {
                Name = $"Page {pageNumber}",
                Path = filePath,
                PageNumber = pageNumber
            });
        }

        public void RemoveTag(UserTagItem tag)
        {
            Tags.Remove(tag);
        }

        private void SaveTags()
        {
            var json = JsonSerializer.Serialize(Tags);
            File.WriteAllText(_saveFilePath, json);
        }

        private void LoadTags()
        {
            if (File.Exists(_saveFilePath))
            {
                var json = File.ReadAllText(_saveFilePath);
                var loadedTags = JsonSerializer.Deserialize<ObservableCollection<UserTagItem>>(json);
                if (loadedTags != null)
                {
                    foreach (var tag in loadedTags)
                        Tags.Add(tag);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
