using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MyPdf.HistoryAndUserTags
{
    public class UserTagGroup : UserTagBase
    {
        private ObservableCollection<UserTagBase> _children = new ObservableCollection<UserTagBase>();
        public ObservableCollection<UserTagBase> Children  { get => _children;  set { if (_children != value) { _children = value; OnPropertyChanged();}}}

        private bool _isInitialView;
        public bool IsInitialView { get => _isInitialView; set { if (_isInitialView != value) { _isInitialView = value; OnPropertyChanged();}}}

        private bool _isExpanded;
        public bool IsExpanded { get => _isExpanded; set { if (_isExpanded != value) { _isExpanded = value; OnPropertyChanged(); } } }

        public UserTagGroup()
        {
            if (Name == null)
                Name = "אנא הזן שם";
        }

        public void Add(UserTagBase item) => Children.Add(item);
       
        public bool Remove(UserTagBase item)
        {
            return Children.Remove(item);
        }
    }
}
