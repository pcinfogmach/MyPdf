using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace MyPdf.HistoryAndUserTags
{
    public class UserTagManager : INotifyPropertyChanged
    {
        private readonly string _saveFilePath = "UserTags.json";

        private ObservableCollection<UserTagBase> _userTags = new ObservableCollection<UserTagBase>();
        public ObservableCollection<UserTagBase> UserTags
        {
            get => _userTags;
            private set
            {
                _userTags = value;
                OnPropertyChanged();
            }
        }

        public UserTagManager()
        {
            LoadTags();
        }

        private async void LoadTags()
        {
            UserTags = await Assets.AssetsManager.GetJsonAssetAsync<ObservableCollection<UserTagBase>>(_saveFilePath) ?? new ObservableCollection<UserTagBase>();
            Application.Current.Exit += (s, e) => SaveTags();
        }

        private async void SaveTags()
        {
            await Assets.AssetsManager.WriteJsonAssetAsync<ObservableCollection<UserTagBase>>(_saveFilePath, UserTags);
        }

        public void AddTag(UserTagBase newTag, UserTagGroup targetGroup)
        {
            if (targetGroup != null) targetGroup.Add(newTag);
            else UserTags.Add(newTag);
        }

        public void AddTag(UserTagBase newTag)
        {
            UserTags.Add(newTag);
        }

        public void RemoveTag(UserTagBase tag)
        {
            if (!UserTags.Remove(tag))
            {
                foreach (var group in UserTags)
                {
                    if (group is UserTagGroup userTagGroup && userTagGroup.Remove(tag))
                    {
                        break;
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
