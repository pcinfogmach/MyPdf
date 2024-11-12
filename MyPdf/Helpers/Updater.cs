using System.Net.Http;
using System.Text.Json;

namespace MyPdf.Helpers
{
    string result = await updateChecker.CheckForUpdates(repoOwner, repoName, currentVersion);
    public class UpdateChecker
    {
        private static readonly HttpClient client = new HttpClient();

        public async Task<string> CheckForUpdates(string repoOwner, string repoName, string currentVersion)
        {
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
                    return $"New version available: {latestVersion}. Download it here: {downloadUrl}";
                }
                else
                {
                    return "You are using the latest version.";
                }
            }
            catch (Exception ex)
            {
                return $"Error checking for updates: {ex.Message}";
            }
        }
    }
}
