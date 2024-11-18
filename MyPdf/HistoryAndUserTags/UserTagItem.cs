using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyPdf.HistoryAndUserTags
{
    public class UserTagItem : INotifyPropertyChanged
    {
        private string _name;
        private string _path;
        private int? _pageNumber;

        public string Name { get => _name; set { if (_name != value) { _name = value; OnPropertyChanged(); } } }
        public string Path { get => _path; set { if (_path != value) { _path = value; OnPropertyChanged(); } } }
        public int PageNumber {  get => _pageNumber ?? 0;  set { if (_pageNumber != value) { _pageNumber = value; OnPropertyChanged(); } } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
