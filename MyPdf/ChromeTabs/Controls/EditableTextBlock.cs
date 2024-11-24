using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace MyPdf.ChromeTabs.Controls
{
    public class EditableTextBlock : ContentControl
    {
        public static readonly DependencyProperty TextProperty =
           DependencyProperty.Register(
               nameof(Text),
               typeof(string),
               typeof(EditableTextBlock),
               new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty IsTextBoxProperty =
            DependencyProperty.Register(
                nameof(IsTextBox),
                typeof(bool),
                typeof(EditableTextBlock),
                new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnIsTextBoxChanged));

        public bool IsTextBox
        {
            get => (bool)GetValue(IsTextBoxProperty);
            set => SetValue(IsTextBoxProperty, value);
        }

        //public static readonly DependencyProperty ForegroundProperty =
        //    DependencyProperty.Register(
        //        nameof(Foreground),
        //        typeof(Brush),
        //        typeof(EditableTextBlock),
        //        new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        //public Brush Foreground
        //{
        //    get => (Brush)GetValue(ForegroundProperty);
        //    set => SetValue(ForegroundProperty, value);
        //}

        private static void OnIsTextBoxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EditableTextBlock control)
            {
                if ((bool)e.NewValue) control.ShowTextBox();
                else control.ShowTextBlock();
            }
        }

        public EditableTextBlock()
        {
            if (IsTextBox) ShowTextBox();
            else ShowTextBlock();
        }

        private void ShowTextBlock()
        {
            TextBlock textBlock = new TextBlock();
            textBlock.SetBinding(TextBlock.TextProperty, new Binding(nameof(Text)) { Source = this, Mode = BindingMode.TwoWay });
            textBlock.SetBinding(TextBlock.ForegroundProperty, new Binding(nameof(Foreground)) { Source = this, Mode = BindingMode.TwoWay });
            textBlock.MouseDown += (s, e) => { if (e.ClickCount == 2) IsTextBox = true; };
            Content = textBlock;
        }

        private void ShowTextBox()
        {
            TextBox textBox = new TextBox { Background = Brushes.Transparent };
            textBox.SetBinding(TextBox.TextProperty, new Binding(nameof(Text)) { Source = this, Mode = BindingMode.TwoWay });
            textBox.SetBinding(TextBox.ForegroundProperty, new Binding(nameof(Foreground)) { Source = this, Mode = BindingMode.TwoWay });
            textBox.KeyDown += (s, e) => { if (e.Key == Key.Enter) { IsTextBox = false; } };
            textBox.LostFocus += (s, e) => { IsTextBox = false; };
            textBox.Loaded += (s, e) =>
            {
                textBox.Focus();
                textBox.SelectAll();
            };
            Content = textBox;
        }
    }
}
