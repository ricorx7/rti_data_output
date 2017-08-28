using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTI
{
    /// <summary>
    /// Main Window with Tabs.
    /// </summary>
    class MainViewModel : Caliburn.Micro.Screen
    {
        /// <summary>
        /// Input Terminal View Model.
        /// </summary>
        public InputTerminalViewModel InputVM { get; set; }

        /// <summary>
        /// OUtput Data ViewModel.
        /// </summary>
        public OutputDataViewModel OutputVM { get; set; }

        /// <summary>
        /// Initialize the ViewModel.
        /// </summary>
        public MainViewModel()
        {
            // Set the 2 ViewModels
            InputVM = IoC.Get<InputTerminalViewModel>();
            OutputVM = IoC.Get<OutputDataViewModel>();
        }

    }
}
