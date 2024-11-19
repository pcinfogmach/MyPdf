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
            if (File.Exists(logFilePath)) { File.Delete(logFilePath); }
            File.Create(logFilePath);

            AppDomain.CurrentDomain.FirstChanceException += (sender, e) =>
            {
                if (e?.Exception == null)
                {
                    return;
                }

                LogException(e.Exception);
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                if (e?.ExceptionObject is Exception exception)
                {
                    LogException(exception);
                }
            };

            // Capture unobserved task exceptions
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                LogException(e.Exception);
                e.SetObserved(); // Prevent process termination.
            };

            // Capture WPF Dispatcher exceptions (if using WPF)
            if (System.Windows.Application.Current != null)
            {
                System.Windows.Application.Current.DispatcherUnhandledException += (sender, e) =>
                {
                    LogException(e.Exception);
                    e.Handled = true; // Prevent default shutdown.
                };
            }
        }

        private static void LogException(Exception ex)
        {
            try
            {
                using (var sw = File.AppendText(logFilePath))
                {
                    string message = $"[{DateTime.Now}] An error occurred:\n" +
                                     $"Message: {ex.Message}\n" +
                                     $"Source: {ex.Source}\n" +
                                     $"Stack Trace: {ex.StackTrace}\n" +
                                     $"Inner Exception: {ex.InnerException?.Message ?? "None"}\n" +
                                     $"Target Site: {ex.TargetSite}\n";

                    sw.WriteLine(message);
                }
            }
            catch
            {
                // Optionally log errors related to logging or leave it empty.
            }
        }

    }
}
