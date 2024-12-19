using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;

namespace MyPdf.Assets.Locale
{
    public static class LocaleHelper
    {
        private static readonly string LocaleDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Locale");
        private static string LocalePath =>
            Path.Combine(LocaleDir, $"{CultureInfo.CurrentCulture.TwoLetterISOLanguageName}.json") is string path && File.Exists(path)
                ? path
                : Path.Combine(LocaleDir, "en.json");

        private static Dictionary<string, string>? _localeDictionary;
        public static Dictionary<string, string> LocaleDictionary => _localeDictionary ??= LoadLocaleDictionary();

        private static Dictionary<string, string> LoadLocaleDictionary()
        {
            if (!File.Exists(LocalePath))
                return new Dictionary<string, string>();

            try
            {
                string json = File.ReadAllText(LocalePath);
                return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
            }
            catch
            {
                // Return an empty dictionary if reading or deserialization fails
                return new Dictionary<string, string>();
            }
        }

        public static MessageBoxResult LocalizedMessage(string messageId,
            MessageBoxButton buttons, MessageBoxImage icon, MessageBoxResult defaultResult)
        {
            if (!LocaleDictionary.TryGetValue("Message." + messageId, out string? jsonValue) || string.IsNullOrEmpty(jsonValue))
                return MessageBoxResult.None;

            try
            {
                var messageData = JsonSerializer.Deserialize<KeyValuePair<string, string>>(jsonValue);
                string message = messageData.Key ?? string.Empty;
                string title = messageData.Value ?? "Application";

                if (string.IsNullOrEmpty(message))
                    return MessageBoxResult.None;

                var options = Regex.IsMatch(message, @"\p{IsHebrew}|\p{IsArabic}")
                    ? MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading
                    : MessageBoxOptions.None;

                return MessageBox.Show(message, title, buttons, icon, defaultResult, options);
            }
            catch
            {
                return MessageBoxResult.None; // Fallback if deserialization fails
            }
        }

        public static MessageBoxResult LocalizedYesNoMessage(string messageId, MessageBoxResult defaultResult)
        {
            if (!LocaleDictionary.TryGetValue("Message." + messageId, out string? jsonValue) || string.IsNullOrEmpty(jsonValue))
                return MessageBoxResult.None;

            try
            {
                var messageData = JsonSerializer.Deserialize<KeyValuePair<string, string>>(jsonValue);
                string message = messageData.Key ?? string.Empty;
                string title = messageData.Value ?? "Application";

                if (string.IsNullOrEmpty(message))
                    return MessageBoxResult.None;

                var options = Regex.IsMatch(message, @"\p{IsHebrew}|\p{IsArabic}")
                    ? MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading
                    : MessageBoxOptions.None;

                if (defaultResult != MessageBoxResult.None && defaultResult != MessageBoxResult.Yes && defaultResult != MessageBoxResult.No) defaultResult = MessageBoxResult.None;
                return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question, defaultResult, options);
            }
            catch
            {
                return MessageBoxResult.None; // Fallback if deserialization fails
            }
        }

        public static MessageBoxResult LocalizedErrorMessage(string messageId)
        {
            if (!LocaleDictionary.TryGetValue("Message." + messageId, out string? jsonValue) || string.IsNullOrEmpty(jsonValue))
                return MessageBoxResult.None;

            try
            {
                var messageData = JsonSerializer.Deserialize<KeyValuePair<string, string>>(jsonValue);
                string message = messageData.Key ?? string.Empty;
                string title = messageData.Value ?? "Application";

                if (string.IsNullOrEmpty(message))
                    return MessageBoxResult.None;

                var options = Regex.IsMatch(message, @"\p{IsHebrew}|\p{IsArabic}")
                    ? MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading
                    : MessageBoxOptions.None;

                return MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, options);
            }
            catch
            {
                return MessageBoxResult.None; // Fallback if deserialization fails
            }
        }
    }
}
