using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
/* ==============================================================================
* 功能描述：DelegateCommand  
* 创 建 者：LY
* 创建日期：2015/11/4 16:56:05
* 说明：事件委派控制,直接使用官方文档中代码
* ==============================================================================*/
namespace LYGISDEMO
{
    class DelegateCommand:System.Windows.Input.ICommand
    {
        /// <summary>
        /// 直接使用官方文档中代码
        /// </summary>
        // a var to store the command's execute logic (button click, for example)
        private readonly Action<object> execute;

        // a var to store the command's logic for enabling/disabling
        private readonly Func<object, bool> canExecute;

        // an event for when the value of "CanExecute" changes (not implemented)
        public event EventHandler CanExecuteChanged;

        // constructor: store the logic for executing and enabling the command
        public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecuteFunc = null)
        {
            this.canExecute = canExecuteFunc;
            this.execute = executeAction;
        }

        // if it was passed in, execute the enabling logic for the command
        public bool CanExecute(object parameter)
        {
            if (this.canExecute == null) { return true; }

            return this.canExecute(parameter);
        }

        // execute the command logic 
        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}
