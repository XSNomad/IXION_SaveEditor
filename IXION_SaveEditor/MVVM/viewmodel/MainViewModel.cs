
using IXION_SaveEditor.core;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System;
using System.Linq;

namespace IXION_SaveEditor.MVVM.viewmodel
{

    internal class MainViewModel : ObservableObject
    {
        internal SettingsViewModel SettingsVM { get; set; }
        internal CheatsViewModel CheatsVM { get; set; }
        internal CitizensViewModel CitizensVM { get; set; }
        internal StabilityViewModel StabilityVM { get; set; }

        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set 
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            SettingsVM = new SettingsViewModel();
            CheatsVM = new CheatsViewModel();
            CitizensVM = new CitizensViewModel();
            StabilityVM = new StabilityViewModel();
            CurrentView = SettingsVM;
        }
    }
}
