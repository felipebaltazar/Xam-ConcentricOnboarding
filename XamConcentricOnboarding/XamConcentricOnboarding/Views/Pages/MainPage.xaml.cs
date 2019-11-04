using AsyncAwaitBestPractices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using XamConcentricOnboarding.ViewModel;

namespace XamConcentricOnboarding
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            var vm = new MainPageViewModel();
            this.BindingContext = vm;
            InitializeComponent();
        }

        private void OnAnimationStart(object sender, EventArgs e)
        {
            if (carouselItem?.ItemsSource == null) return;

            var currentItemSource = carouselItem.ItemsSource.Cast<object>();
            var total = currentItemSource.Count();
            var selectedItem = carouselItem.CurrentItem;
            var currentIndex = currentItemSource.IndexOf(selectedItem);
            var nextItem = currentIndex + 1 < total ? currentIndex + 1 : 0;

            ScrollDelayedAsync(nextItem)
                .SafeFireAndForget(onException: ex => Debug.WriteLine(ex.Message));
        }

        private async Task ScrollDelayedAsync(int nextItem)
        {
            await Task.Delay(600);
            carouselItem.ScrollTo(nextItem);
        }
    }
}
