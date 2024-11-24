using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace MyPdf.HistoryAndUserTags
{
    /// <summary>
    /// Interaction logic for InputBox.xaml
    /// </summary>
    public partial class UserTagInputBox : ThemedWindow.Controls.ThemedToolWindow
    {
        string _filePath;
        public UserTagInputBox(string filePath, int? pageNumber)
        {
            InitializeComponent();
            
            _filePath = filePath;
            FileNameTextBlock.Text = Path.GetFileName(filePath);
            PageNumberTextBox.Text = pageNumber.ToString();
            bool isHebrewCulture = (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "he");
            UserTagNameTextBox.Text = isHebrewCulture ? "אנא הזן שם עבור הסימניה..." : "Please enter a name for the bookmark...";
            this.Title = isHebrewCulture ? "סימניה חדשה" : "New bookmark";
            PageNumberLabelBox.Text = isHebrewCulture ? "- מספר עמוד:" : "- Page Number:";

            StateChanged += (s, e) => { if (WindowState == WindowState.Maximized) WindowState = WindowState.Normal; };
        }

        private void TWindow_ContentRendered(object sender, EventArgs e)
        {
            UserTagNameTextBox.SelectAll();
            UserTagNameTextBox.Focus();
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public UserTagItem? Result
        {
            get 
            {
                if (string.IsNullOrEmpty(PageNumberTextBox.Text) || string.IsNullOrEmpty(UserTagNameTextBox.Text)) return null;
                return new UserTagItem
                {
                    Path = _filePath,
                    FileName = Path.GetFileName(_filePath),
                    Name = UserTagNameTextBox.Text,
                    Position = int.Parse(PageNumberTextBox.Text),
                };
            }
        }

        private void PageNumberTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("^[0-9]+$");
            e.Handled = !regex.IsMatch(e.Text);
        }
    }
}
