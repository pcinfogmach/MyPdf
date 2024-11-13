﻿using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Windows;

namespace MyPdf.Helpers
{   
    public static class UpdateChecker
    {
        private static readonly HttpClient client = new HttpClient();

        public static void CheckForUpdates()
        {
            string updateUrl = CheckForUpdatesTask().Result;
            if (!string.IsNullOrEmpty(updateUrl))
            {
                // Check the current culture
                if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "he")
                {
                    // Show a MessageBox with Hebrew message (RTL support)
                    MessageBoxResult result = MessageBox.Show("האם ברצונך להוריד את הגרסה החדשה?", "עדכון גרסה",
                        MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes,
                        MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);

                    // Check user's response
                    if (result == MessageBoxResult.Yes)
                    {
                        // Open the URL in the default browser
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = updateUrl,
                            UseShellExecute = true
                        });
                    }
                }
                else
                {
                    // Show a MessageBox in default language
                    MessageBoxResult result = MessageBox.Show("Do you want to download the new version?", "Update Available",
                        MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);

                    // Check user's response
                    if (result == MessageBoxResult.Yes)
                    {
                        // Open the URL in the default browser
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = updateUrl,
                            UseShellExecute = true
                        });
                    }
                }
            }
        }

        async static Task<string> CheckForUpdatesTask()
        {
            string repoOwner = "pcinfogmach";
            string repoName = "MyPdf";
            string currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            try
            {
                // GitHub API URL for the latest release
                string url = $"https://api.github.com/repos/{repoOwner}/{repoName}/releases/latest";

                // Set up request headers
                client.DefaultRequestHeaders.UserAgent.ParseAdd("MyApp"); // GitHub requires a User-Agent

                // Send request to GitHub API
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // Parse JSON response
                string jsonResponse = await response.Content.ReadAsStringAsync();
                JsonDocument jsonDocument = JsonDocument.Parse(jsonResponse);
                JsonElement root = jsonDocument.RootElement;

                // Get latest version tag
                string latestVersion = root.GetProperty("tag_name").GetString();

                // Compare with the current version
                if (string.Compare(latestVersion, currentVersion, StringComparison.OrdinalIgnoreCase) > 0)
                {
                    string downloadUrl = root.GetProperty("html_url").GetString();
                    return downloadUrl;
                }
                else { return string.Empty; }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}