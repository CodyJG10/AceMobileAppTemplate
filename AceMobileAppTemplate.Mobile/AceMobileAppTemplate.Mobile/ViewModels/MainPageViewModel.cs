using AceMobileAppTemplate.Dependencies;
using AceMobileAppTemplate.Entities;
using AceMobileAppTemplate.Views;
using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AceMobileAppTemplate.ViewModels
{
    public class MainPageViewModel : BaseDataViewModel
    {
        public MainPageViewModel() : base() { }

        private void SignOut()
        {
            CrossSettings.Current.Clear();
            Application.Current.MainPage = new LoginPage();
            Application.Current.MainPage.DisplayAlert("", "You've been signed out", "Ok");
        }
    }
}