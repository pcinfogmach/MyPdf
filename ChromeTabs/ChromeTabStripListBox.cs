using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ChromeTabs
{
    internal class ChromeTabStripListBox : ListBox
    {
        public ChromeTabStripListBox() 
        {
            SelectionChanged += (s, e) => { ScrollIntoView(SelectedItem); };
        }
    }
}
