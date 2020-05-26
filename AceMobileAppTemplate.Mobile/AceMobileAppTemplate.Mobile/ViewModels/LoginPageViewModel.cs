using AceMobileAppTemplate.Dependencies;
using AceMobileAppTemplate.Models;
using AceMobileAppTemplate.ViewModels.Auth;
using AceMobileAppTemplate.Views;
using Newtonsoft.Json;
using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AceMobileAppTemplate.ViewModels
{
    public class LoginPageViewModel : LoginViewModelBase
    {
        public ICommand ForgotPasswordCommand { get; set; }

        public LoginPageViewModel()
        {
            LoginCommand = new Command(Login);
            RegisterCommand = new Command(Register);
            ForgotPasswordCommand = new Command<string>(ForgotPassword);
            RegisterCommand = new Command(x => 
            { 
                Application.Current.MainPage = new RegisterPage(); 
            });
        }

        private void ForgotPassword(string email)
        {
            _db.ForgotPassword(email);
            Application.Current.MainPage.DisplayAlert("Reset Password Request Recieved", "Please check the email sent to you for a link to reset you password", "Ok");
        }

        public async void Login()
        {
            Loading = true;
            var loginResponse = await _db.Login(Email, Password);
            if (loginResponse.IsSuccessStatusCode)
            {
                var contentString = await loginResponse.Content.ReadAsStringAsync();
                var content = JsonConvert.DeserializeObject<AuthResult>(contentString);
                Console.WriteLine("Succesfully logged in!");
                var userDataResponse = await _db.GetUserData();
                var userDataJson = await userDataResponse.Content.ReadAsStringAsync();
                CrossSettings.Current.AddOrUpdateValue("UserData", userDataJson);
                CrossSettings.Current.AddOrUpdateValue("RefreshToken", content.RefreshToken);
                CrossSettings.Current.AddOrUpdateValue("Token", content.Token);
                var userData = JsonConvert.DeserializeObject<UserData>(userDataJson);
                var notificationRegistration = DependencyService.Get<INotificationRegistration>();
                notificationRegistration.RegisterForRemoteNotifications(userData.Email, _db);
                Application.Current.MainPage = new NavigationPage(new MainPage())
                {
                    BarBackgroundColor = (Color)Application.Current.Resources["Theme-500"],
                    BarTextColor = (Color)Application.Current.Resources["Theme-100"],
                };
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Authentication Error", "Please make sure you've entered the correct credentials", "Return");
            }
            Loading = false;
        }

        public void Register()
        {
            Application.Current.MainPage = new RegisterPage();
        }
    }
}
