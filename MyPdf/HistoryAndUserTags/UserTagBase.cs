using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyPdf.HistoryAndUserTags
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(UserTagBase), "base")]
    [JsonDerivedType(typeof(UserTagGroup), "group")]
    [JsonDerivedType(typeof(UserTagItem), "item")]
    public class UserTagBase : INotifyPropertyChanged
    {
        private string _name;
        public string Name { get => _name; set { if (_name != value) { _name = value; OnPropertyChanged(); } } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
