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

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjU4NDI1QDMxMzgyZTMxMmUzMHBFVEtTMWVrSStFQ0xmdVZKcllWWFl3amUwVTZSV3Y4bjEzNC9sT3hUZ009");

            InitServices();

            MainPage = new LoadingPage();

            if (CrossSettings.Current.GetValueOrDefault("RefreshToken", "_") == "_")
            {
                MainPage = new LoginPage();
            }
            else
            { 
                RefreshToken();
            }
        }

        private async void RefreshToken()
        {
            string refreshToken = CrossSettings.Current.GetValueOrDefault("RefreshToken", "_");
            string token = CrossSettings.Current.GetValueOrDefault("Token", "_");
            var db = Container.Resolve<IDatabaseManager>();
            var response = await db.RefreshToken(token, refreshToken);
            if (response.IsSuccessStatusCode)
            {
                var contentString = await response.Content.ReadAsStringAsync();
                var content = JsonConvert.DeserializeObject<AuthResult>(contentString);
                var newToken = content.Token;
                var newRefreshToken = content.RefreshToken;
                db.SetToken(token);
                CrossSettings.Current.AddOrUpdateValue("RefreshToken", newRefreshToken);
                CrossSettings.Current.AddOrUpdateValue("Token", newToken);
                MainPage = new NavigationPage(new MainPage())
                {
                    BarBackgroundColor = (Color)Current.Resources["Theme-500"],
                    BarTextColor = (Color)Current.Resources["Theme-100"],
                };
            }
            else
            {
                MainPage = new LoginPage();
                await MainPage.DisplayAlert("Authentication Error", "You have been logged out", "Ok");
            }
        }

        private void InitServices()
        {
            var builder = new ContainerBuilder();
            builder.RegisterInstance(new DatabaseManager("https://AceMobileAppTemplate.azurewebsites.net/"))
                .As<IDatabaseManager>()
                .SingleInstance();
            Container = builder.Build();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
