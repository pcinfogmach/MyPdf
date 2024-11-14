using System.Windows.Controls;

namespace ChromeTabs
{
    public class ChromeTabControl :TabControl
    {
        public void Add(TabItem tabItem)
        {
            this.Items.Add(tabItem);
            this.SelectedItem = tabItem;
        }
    }
}
