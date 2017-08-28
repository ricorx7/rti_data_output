using Caliburn.Micro;
namespace RTI {

    public class ShellViewModel : Conductor<object>, IShell, IDeactivate
    {
        public ShellViewModel()
        {
            base.DisplayName = "RoweTech Inc. Format Output";

            // Set the view
            var vm = IoC.Get<MainViewModel>();
            ActivateItem(vm);
        }

        /// <summary>
        /// Shutdown the view model.
        /// </summary>
        /// <param name="close"></param>
        void IDeactivate.Deactivate(bool close)
        {
            var vm = IoC.Get<InputTerminalViewModel>();
            if (vm != null)
            {
                vm.Dispose();
            }

            var vmOutput = IoC.Get<OutputDataViewModel>();
            if(vmOutput != null)
            {
                vmOutput.Dispose();
            }
        }
    }
}