using System.Collections.Generic;
using System.Windows.Media;

namespace SyncPdf.Models
{
    public class ThemeList
    {     
        public static List<string> GetThemeList()
        {
            List<string> themeList = new List<string>()
            {
                "Windows11 Light",
                "Windows11 Dark",
                "Material Light",
                "Material Dark",
                "Material Light Blue",
                "Material Dark Blue",
                "Office 2019 Colorful",
                "Office 2019 Black",
                "Office 2019 White",
                "Office 2019 Dark Gray",
                "Office 2019 High Contrast"
            };
            return themeList;
        }
    }

    public class Palette
    {
        private string name;
        /// <summary>
        /// Denotes the palette name
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string theme;
        /// <summary>
        /// Denotes the Theme Name
        /// </summary>
        public string Theme
        {
            get { return theme; }
            set { theme = value; }
        }

        private Brush primaryBackground;
        /// <summary>
        /// Denotes the palette primary background brush
        /// </summary>
        public Brush PrimaryBackground
        {
            get { return primaryBackground; }
            set { primaryBackground = value; }
        }

        private Brush primaryForeground;
        /// <summary>
        /// Denotes the palette primary foreground brush
        /// </summary>
        public Brush PrimaryForeground
        {
            get { return primaryForeground; }
            set { primaryForeground = value; }
        }

        private Brush primaryBackgroundAlt;
        /// <summary>
        /// Denotes the palette primay alternate background brush
        /// </summary>
        public Brush PrimaryBackgroundAlt
        {
            get { return primaryBackgroundAlt; }
            set { primaryBackgroundAlt = value; }
        }

        private string displayName;
        /// <summary>
        /// denotes the name to be displayed in the UI
        /// </summary>
        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; }
        }

        private Brush primaryBorderColor;
        /// <summary>
        /// denotes the name to be displayed in the UI
        /// </summary>
        public Brush PrimaryBorderColor
        {
            get { return primaryBorderColor; }
            set { primaryBorderColor = value; }
        }
    }
}
