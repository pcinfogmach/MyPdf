using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyPdf.HistoryAndUserTags
{
    public class UserTagItem : UserTagBase
    {
        private string _path;
        private string _fileName;
        private int? _position;

        public string Path { get => _path; set { if (_path != value) { _path = value; FileName = (System.IO.Path.GetFileName(value)); OnPropertyChanged(); } } }
        public string FileName { get => _fileName; set { if (_fileName != value) { _fileName = value; OnPropertyChanged(); } } }
        public int? Position {  get => _position ?? 0;  set { if (_position != value) { _position = value; OnPropertyChanged(); } } }
    }
}
