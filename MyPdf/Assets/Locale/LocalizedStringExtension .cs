using System.Windows.Markup;

namespace MyPdf.Assets.Locale
{
    public class LocalizedStringExtension : MarkupExtension
    {
        public string Key { get; set; }

        public LocalizedStringExtension() { }
        public LocalizedStringExtension(string key) { Key = key; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(Key)) return "[Missing Key]"; 
            else  return LocaleHelper.LocaleDictionary.TryGetValue(Key, out var value) ? value : $"[{Key}]";
        }
    }
}
