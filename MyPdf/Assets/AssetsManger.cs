using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyPdf.Assets
{
    public static class AssetsManager
    {
        private static string _assetsDir;
        public static string AssetsDir
        {
            get
            {
                if (_assetsDir == null)
                {
                    _assetsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
                    if (!Directory.Exists(_assetsDir))
                    {
                        Directory.CreateDirectory(_assetsDir);
                    }
                }
                return _assetsDir;
            }
        }

        /// <summary>
        /// Writes the given asset data to a file in the Assets directory.
        /// </summary>
        /// <param name="assetName">The name of the asset file.</param>
        /// <param name="data">The data to write to the file.</param>
        public static async Task WriteAssetAsync(string assetName, string data)
        {
            string filePath = Path.Combine(AssetsDir, assetName);
            await File.WriteAllTextAsync(filePath, data);
        }

        /// <summary>
        /// Reads the content of the specified asset file from the Assets directory.
        /// </summary>
        /// <param name="assetName">The name of the asset file.</param>
        /// <returns>The content of the file as a byte array.</returns>
        public static async Task<string> GetAssetAsync(string assetName)
        {
            string filePath = Path.Combine(AssetsDir, assetName);
            if (!File.Exists(filePath)) return string.Empty;
            return await File.ReadAllTextAsync(filePath);
        }
        /// <summary>
        /// Serializes the given object to JSON asynchronously and writes it to a file in the Assets directory.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="assetName">The name of the asset file.</param>
        /// <param name="data">The object to serialize.</param>
        public static async Task WriteJsonAssetAsync<T>(string assetName, T data)
        {
            string filePath = Path.Combine(AssetsDir, assetName);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true, // Optional for readability
                IncludeFields = true // Ensure fields are serialized if necessary
            };
            await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await JsonSerializer.SerializeAsync(fileStream, data, options);
        }

        /// <summary>
        /// Reads and deserializes the specified JSON file asynchronously from the Assets directory into an object of type T.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <param name="assetName">The name of the asset file.</param>
        /// <returns>An instance of the specified type containing the deserialized data.</returns>
        public static async Task<T> GetJsonAssetAsync<T>(string assetName)
        {
            string filePath = Path.Combine(AssetsDir, assetName);
            if (!File.Exists(filePath)) return default;

            var options = new JsonSerializerOptions
            {
                IncludeFields = true // Ensure fields are deserialized if necessary
            };
            await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return await JsonSerializer.DeserializeAsync<T>(fileStream, options);
        }

    }
}
