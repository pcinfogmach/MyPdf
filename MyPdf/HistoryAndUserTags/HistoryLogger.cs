using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace MyPdf.HistoryAndUserTags
{
    public class HistoryLogger : INotifyPropertyChanged
    {
        private const int MaxHistoryRetentionDays = 14;
        private readonly string _historyFilePath;

        private ObservableCollection<HistoryGroup> _historyGroups = new ObservableCollection<HistoryGroup>();
        public ObservableCollection<HistoryGroup> HistoryGroups
        {
            get => _historyGroups;
            private set
            {
                _historyGroups = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public HistoryLogger()
        {
            var appDomainPath = AppDomain.CurrentDomain.BaseDirectory;
            var assetsPath = Path.Combine(appDomainPath, "Assets");
            if (!Directory.Exists(assetsPath)) Directory.CreateDirectory(assetsPath);
            _historyFilePath = Path.Combine(assetsPath, "history.json");

            if (File.Exists(_historyFilePath))
                LoadHistory();
            else
                InitializeGroups();
        }

        public void AddHistoryItem(string title, string path)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(path))
                return;

            var now = DateTime.Now;
            var newItem = new HistoryItem
            {
                Timestamp = now,
                Title = title,
                Path = path
            };

            // Remove earlier entries with the same Title and Path
            foreach (var group in _historyGroups)
            {
                var existingItem = group.Items.FirstOrDefault(item => item.Title == title && item.Path == path);
                if (existingItem != null)
                {
                    if (existingItem.Timestamp < newItem.Timestamp)
                    {
                        group.Items.Remove(existingItem);
                    }
                    else
                    {
                        return; // Skip adding the new one
                    }
                }
            }

            // Determine appropriate group
            var groupName = now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday
                ? "Weekend"
                : CultureInfo.CurrentCulture.DateTimeFormat.DayNames[(int)now.DayOfWeek];

            var targetGroup = _historyGroups.FirstOrDefault(g => g.Name == groupName);
            if (targetGroup == null)
            {
                targetGroup = new HistoryGroup(groupName) { IsExpanded = true };
                _historyGroups.Add(targetGroup);
            }

            targetGroup.Items.Insert(0, newItem); // Add the new item at the beginning
            CleanupOldItems();
            SortAllGroups();
            SaveHistory();
        }

        private void SortAllGroups()
        {
            foreach (var group in _historyGroups)
            {
                var sortedItems = group.Items.OrderByDescending(item => item.Timestamp).ToList();
                group.Items.Clear();
                foreach (var item in sortedItems)
                {
                    group.Items.Add(item);
                }
            }
        }

        private void CleanupOldItems()
        {
            var now = DateTime.Now;
            var lastWeekGroup = _historyGroups.FirstOrDefault(g => g.Name == GetLocalizedLastWeekName(CultureInfo.CurrentCulture));

            // Move items older than 7 days to "Last Week" group
            foreach (var group in _historyGroups.ToList())
            {
                var itemsToMove = group.Items.Where(item => (now - item.Timestamp).TotalDays > 7).ToList();
                foreach (var item in itemsToMove)
                {
                    group.Items.Remove(item);
                    lastWeekGroup?.Items.Add(item);
                }
            }

            // Remove items older than MaxHistoryRetentionDays from "Last Week"
            if (lastWeekGroup != null)
            {
                var itemsToRemove = lastWeekGroup.Items.Where(item => (now - item.Timestamp).TotalDays > MaxHistoryRetentionDays).ToList();
                foreach (var item in itemsToRemove)
                {
                    lastWeekGroup.Items.Remove(item);
                }
            }

            // Remove empty groups
            foreach (var group in _historyGroups.ToList())
            {
                if (group.Items.Count == 0)
                {
                    _historyGroups.Remove(group);
                }
            }
        }


        private void InitializeGroups()
        {
            var now = DateTime.Now;

            for (var i = 0; i < 7; i++)
            {
                var day = now.AddDays(-i);
                if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
                {
                    if (_historyGroups.All(g => g.Name != "Weekend"))
                        _historyGroups.Add(new HistoryGroup("Weekend") { IsExpanded = true });
                }
                else
                {
                    var groupName = CultureInfo.CurrentCulture.DateTimeFormat.DayNames[(int)day.DayOfWeek];
                    if (_historyGroups.All(g => g.Name != groupName))
                        _historyGroups.Add(new HistoryGroup(groupName) { IsExpanded = day.Date == now.Date });
                }
            }

            // Add "Last Week" group
            _historyGroups.Add(new HistoryGroup(GetLocalizedLastWeekName(CultureInfo.CurrentCulture)));
        }

        private string GetLocalizedLastWeekName(CultureInfo culture)
        {
            return culture.TwoLetterISOLanguageName == "he" ? "שבוע שעבר" : "Last Week";
        }

        private void SaveHistory()
        {
            try
            {
                var json = JsonSerializer.Serialize(_historyGroups, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_historyFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving history to file: {_historyFilePath}. Exception: {ex.Message}");
            }
        }

        private void LoadHistory()
        {
            try
            {
                var json = File.ReadAllText(_historyFilePath);
                var loadedHistoryGroups = JsonSerializer.Deserialize<ObservableCollection<HistoryGroup>>(json);
                if (loadedHistoryGroups != null)
                {
                    _historyGroups = loadedHistoryGroups;
                    OnPropertyChanged(nameof(HistoryGroups));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading history from file: {_historyFilePath}. Exception: {ex.Message}");
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
