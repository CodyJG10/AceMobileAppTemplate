using AceMobileAppTemplate.ViewModels;
using Syncfusion.XForms.Buttons;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AceMobileAppTemplate.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void ForgotPasswordLabel_Tapped(object sender, EventArgs e)
        {
            var popupLayout = new SfPopupLayout();
            popupLayout.PopupView.AnimationMode = AnimationMode.Zoom;
            popupLayout.PopupView.ShowHeader = false;
            popupLayout.PopupView.ShowFooter = false;

            var templateView = new DataTemplate(() =>
            {
                var label = new Label
                {
                    Text = "Please enter your email in the field below, then check your inbox for a link to reset your password.",
                    HorizontalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                    FontSize = 18,
                };

                var stackLayout = new StackLayout()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.CenterAndExpand
                };

                var buttonLayout = new StackLayout()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    Orientation = StackOrientation.Horizontal
                };

                Entry emailEntry = new Entry()
                {
                    Placeholder = "Email",
                    HorizontalTextAlignment = TextAlignment.Center,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Margin = new Thickness(5, 0, 5, 0)
                };

                var sendButton = new SfButton()
                {
                    Text = "Send",
                    WidthRequest = 65d,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                };

                var cancelButton = new SfButton()
                {
                    Text = "Cancel",
                    WidthRequest = 65d,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                };

                sendButton.Clicked += (s, a) =>
                {
                    string email = emailEntry.Text;
                    (BindingContext as LoginPageViewModel).ForgotPasswordCommand.Execute(email);
                    popupLayout.IsOpen = false;
                };
                cancelButton.Clicked += (s, a) =>
                {
                    popupLayout.IsOpen = false;
                };

                buttonLayout.Children.Add(sendButton);
                buttonLayout.Children.Add(cancelButton);

                stackLayout.Children.Add(label);
                stackLayout.Children.Add(emailEntry);
                stackLayout.Children.Add(buttonLayout);

                return stackLayout;
            });

            popupLayout.PopupView.ContentTemplate = templateView;

            popupLayout.Show();
        }
    }
}