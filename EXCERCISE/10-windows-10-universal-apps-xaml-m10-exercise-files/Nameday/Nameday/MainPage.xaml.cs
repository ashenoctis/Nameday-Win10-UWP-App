using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Nameday
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainPageData Logic => DataContext as MainPageData;

        public MainPage()
        {
            this.InitializeComponent();
            Logic.PropertyChanged += Logic_PropertyChanged;
        }

        private void Logic_PropertyChanged(object sender, 
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainPageData.LoadingState))
                VisualStateManager.GoToState(this, Logic.LoadingState.ToString(), true);
        }

        private async void btEmail_Click(object sender, RoutedEventArgs e)
        {
            var contact = ((FrameworkElement)sender).DataContext as ContactEx;

            if (contact != null)
                await ((MainPageData)this.DataContext).SendEmailAsync(contact.Contact);
        }

        private void appBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }

        private void appBarButton1_Click(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = true;
        }
    }
}
