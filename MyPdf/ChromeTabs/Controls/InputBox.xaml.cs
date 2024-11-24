using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MyPdf.ChromeTabs.Controls
{
    /// <summary>
    /// Interaction logic for InputBox.xaml
    /// </summary>
    public partial class InputBox : ThemedWindow.Controls.ThemedToolWindow
    {
        public InputBox(string message, string defaultText)
        {
            this.Owner = Application.Current.MainWindow;
            InitializeComponent();
            MessageTextBlock.Text = message;
            ResultTextBox.Text = defaultText;
            StateChanged += (s, e) => { if (WindowState == WindowState.Maximized) WindowState = WindowState.Normal; };
        }

        private void TWindow_ContentRendered(object sender, EventArgs e)
        {
            ResultTextBox.SelectAll();
            ResultTextBox.Focus();
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public string Result
        {
            get { return ResultTextBox.Text; }
        }
    }
}
