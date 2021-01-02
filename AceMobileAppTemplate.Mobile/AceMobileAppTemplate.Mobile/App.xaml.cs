using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using AceMobileAppTemplate.Views;
using AceMobileAppTemplate.Styles;
using Plugin.Settings;
using AceMobileAppTemplate.Api;
using Newtonsoft.Json;
using AceMobileAppTemplate.Models;
using Autofac;

namespace AceMobileAppTemplate
{
    public partial class App : Application
    {
        public static IContainer Container { get; private set; }

        public App()
        {
            InitializeComponent();

            MainPage = new LoadingPage();

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(AceMobileAppTemplate.Mobile.Properties.Resources.SYNCFUSION_LICENSE);

            InitServices();

            try
            {
                Container.Resolve<IDatabaseManager>().Authenticate();
            }
            catch (Exception e)
            {
                MainPage.DisplayAlert("Error", "There was an error authenticating with the server: " + e.Message, "Close");
                Quit();
            }

            if (CrossSettings.Current.GetValueOrDefault("Username", "_") != "_")
            {
                AutoLogin();
            }
            else
            {
                MainPage = new LoginPage();
            }
        }

        private async void AutoLogin()
        {
            string username = CrossSettings.Current.GetValueOrDefault("Username", "");
            string password = CrossSettings.Current.GetValueOrDefault("Password", "");
            var db = Container.Resolve<IDatabaseManager>();
            var loginResponse = await db.Login(username, password);
            if (loginResponse.IsSuccessStatusCode)
            {
                MainPage = new NavigationPage(new MainPage())
                {
                    BarBackgroundColor = (Color)Current.Resources["Theme-500"],
                    BarTextColor = (Color)Current.Resources["Theme-100"],
                };
            }
            else
            {
                MainPage = new LoginPage();
                MainPage = new LoginPage();
                await MainPage.DisplayAlert("Authentication Error", "You have been logged out", "Ok");
            }
        }

        private void InitServices()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(new DatabaseManager(Mobile.Properties.Resources.WEB_API_URL))
                .As<IDatabaseManager>()
                .SingleInstance();
            Container = builder.Build();
        }

        protected override void OnStart()
        {

        }

        protected override void OnSleep()
        {
            //(Container.Resolve<AutoUpdater>()).Stop();
        }

        protected override void OnResume()
        {
            //(Container.Resolve<AutoUpdater>()).Start();
        }
    }
}
