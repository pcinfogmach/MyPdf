using System;
using System.IO;

namespace MyPdf.Helpers
{
    internal static class ErrorLog
    {
        private static readonly string logFilePath = @".\logs\exceptions.txt";

        public static void Initialize()
        {
            // Ensure the log directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
            if (File.Exists(logFilePath)) {File.Delete(logFilePath);}
            File.Create(logFilePath);

            AppDomain.CurrentDomain.FirstChanceException += (sender, e) =>
            {
                if (e?.Exception == null)
                {
                    return;
                }

                LogException(e.Exception.ToString());
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                if (e?.ExceptionObject is Exception exception)
                {
                    LogException(exception.ToString());
                }
            };
        }

        private static void LogException(string message)
        {
            try
            {
                using (var sw = File.AppendText(logFilePath))
                {
                    sw.WriteLine($"[{DateTime.Now}] {message}");
                }
            }
            catch
            {
                // Consider handling logging errors if necessary.
            }
        }
    }
}
