using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChromeTabs.Helpers
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action commandTask;
        public RelayCommand(Action action) { commandTask = action; }
        public virtual bool CanExecute(object parameter) {  return true; }
        protected void OnCanExecuteChanged()  {   CanExecuteChanged?.Invoke(this, new EventArgs());}
        public void Execute(object parameter)  { commandTask(); }              
    }
}
