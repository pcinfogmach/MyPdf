using System;
using System.Windows.Controls;

namespace ChromeTabs
{
    public class ChromeTabControl : TabControl
    {
        // Define a custom event
        public event EventHandler<TabItemEventArgs>? TabAdded;

        // Method to add a tab and raise the TabAdded event
        public void Add(TabItem tabItem)
        {
            this.Items.Add(tabItem);
            this.SelectedItem = tabItem;

            // Raise the event
            OnTabAdded(new TabItemEventArgs(tabItem));
        }

        // Helper method to raise the TabAdded event
        protected virtual void OnTabAdded(TabItemEventArgs e)
        {
            TabAdded?.Invoke(this, e);
        }
    }

    // Custom EventArgs class to hold the added TabItem
    public class TabItemEventArgs : EventArgs
    {
        public TabItem TabItem { get; }

        public TabItemEventArgs(TabItem tabItem)
        {
            TabItem = tabItem;
        }
    }
}
