using AceMobileAppTemplate.Dependencies;
using AceMobileAppTemplate.Models;
using AceMobileAppTemplate.ViewModels.Auth;
using AceMobileAppTemplate.Views;
using Newtonsoft.Json;
using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AceMobileAppTemplate.ViewModels
{
    public class RegisterPageViewModel : LoginViewModelBase
    {
        public RegisterPageViewModel() : base()
        {
            RegisterCommand = new Command(Register);
            LoginCommand = new Command(x => Application.Current.MainPage = new LoginPage());
        }

        private async void Register()
        {
            if (Loading)
                return;
            Loading = true;
            var result = await _db.Register(Email, Password, ConfirmPassword);
            if (result.IsSuccessStatusCode)
            {
                var loginResponse = await _db.Login(Email, Password);
                if (loginResponse.IsSuccessStatusCode)
                {
                    try
                    {
                        DependencyService.Get<INotificationRegistration>().RegisterForRemoteNotifications(_user.Email, _db);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    var contentString = await loginResponse.Content.ReadAsStringAsync();
                    var content = JsonConvert.DeserializeObject<AuthResult>(contentString);
                    var userDataResponse = await _db.GetUserData();
                    var userDataJson = await userDataResponse.Content.ReadAsStringAsync();
                    CrossSettings.Current.AddOrUpdateValue("UserData", userDataJson);
                    CrossSettings.Current.AddOrUpdateValue("RefreshToken", content.RefreshToken);
                    CrossSettings.Current.AddOrUpdateValue("Token", content.Token);
                    Application.Current.MainPage = new NavigationPage(new MainPage())
                    {
                        BarBackgroundColor = (Color)Application.Current.Resources["Theme-500"],
                        BarTextColor = (Color)Application.Current.Resources["Theme-100"],
                    };
                }
            }
            else
            {
                var content = await result.Content.ReadAsStringAsync();
                var errors = JsonConvert.DeserializeObject<ErrorResult>(content);
                await Application.Current.MainPage.DisplayAlert("Error", errors.Errors[0], "Return");
            }
            Loading = false;
        }

        struct ErrorResult 
        {
            [JsonProperty("")]
            public List<string> Errors { get; set; }
        }
    }
}