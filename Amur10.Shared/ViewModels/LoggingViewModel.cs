using Amur10.Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amur10.Shared.ViewModels
{
    public class LoggingViewModel : BaseViewModel
    {
        private static LoggingViewModel _instance;
        public static LoggingViewModel Instance
        {
            get
            {
                return _instance ?? (new LoggingViewModel());
            }
        }

        private static ObservableCollection<Models.LogMessage> _logMessages = new ObservableCollection<LogMessage>();
        public static ObservableCollection<LogMessage> LogMessages
        {
            get
            {
                return _logMessages;
            }            
        }

        public LoggingViewModel()
        {
           
        }

        public void AddMessage(LogMessage msg)
        {           
            LogMessages.Add(msg);
        }
    }
}
