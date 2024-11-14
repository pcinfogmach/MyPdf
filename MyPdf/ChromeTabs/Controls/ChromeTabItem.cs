using ChromeTabs.Helpers;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChromeTabs
{
    public class ChromeTabItem : TabItem
    {
        private ICommand _closeTabCommand;
        public ICommand CloseTabCommand => _closeTabCommand ??= new RelayCommand(Dispose);

        public virtual void Dispose()
        {
            if (this.Parent is TabControl tabControl)
            {
                int tabIndex = tabControl.Items.IndexOf(this);
                if (tabIndex != tabControl.SelectedIndex) tabIndex = -1;
                tabControl.Items.Remove(this);                
                if (tabIndex != -1) tabControl.SelectedIndex = tabIndex >= tabControl.Items.Count ? tabIndex - 1 : tabIndex;
            }
        }
    }
}
