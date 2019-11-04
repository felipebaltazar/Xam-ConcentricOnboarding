using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace XamConcentricOnboarding.ViewModel
{
    public sealed class MainPageViewModel : BaseViewModel
    {
        private ObservableCollection<Message> itemSource;

        public ObservableCollection<Message> ItemSource
        {
            get => itemSource;
            set => SetProperty(ref itemSource, value);
        }

        public MainPageViewModel()
        {
            ItemSource = new ObservableCollection<Message>
            {
                new Message
                {
                    Title = "Hello Xamarin.Forms + SkiaSharp",
                    Text = "Tap in the button bellow to scroll the CarouselView",
                    Image = ImageSource.FromResource("XamConcentricOnboarding.Images.skia.png")
                },
                new Message
                {
                    Title = "Background color",
                    Text = "Each time the button is clicked, bacground color will change..",
                    Image = ImageSource.FromResource("XamConcentricOnboarding.Images.background.png")
                },
                new Message
                {
                    Title = "CarouselView",
                    Text = "Here im using carouselview to scroll theses messages, but you can use any scrollable element",
                    Image = ImageSource.FromResource("XamConcentricOnboarding.Images.xamarin.png")
                }
            };
        }
    }

    public struct Message
    {
        public string Title { get; set; }


        public string Text { get; set; }

        public ImageSource Image { get; set; }
    }
}
