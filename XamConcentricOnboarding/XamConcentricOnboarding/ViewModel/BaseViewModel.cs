using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using XamConcentricOnboarding.Helpers;

namespace XamConcentricOnboarding.ViewModel
{
    public abstract class BaseViewModel : ObservableObject
    {
        #region Fields

        private bool isBusy;

        #endregion

        #region Properties

        public bool IsNotBusy => !IsBusy;

        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value, onChanged: () => OnPropertyChanged(nameof(IsNotBusy)));
        }

        #endregion

        #region Protected Methods

        protected async Task ExecuteBusyActionAsync(Func<Task> theBusyAction)
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;
                await theBusyAction?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected void ExecuteBusyAction(Action theBusyAction)
        {
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;
                theBusyAction?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion
    }
}