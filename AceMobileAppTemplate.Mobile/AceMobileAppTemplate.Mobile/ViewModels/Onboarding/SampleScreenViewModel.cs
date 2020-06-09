using AceMobileAppTemplate.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AceMobileAppTemplate.Mobile.ViewModels.Onboarding
{
    public class SampleScreenViewModel : OnboardingViewModelBase
    {
        public SampleScreenViewModel() : base() { }

        protected override void InitializeOnBoarding()
        {
            Items = new ObservableCollection<OnboardingOverviewModel>
            {
                new OnboardingOverviewModel
                {
                    Title = "Welcome to \n My App!",
                    Content = "This is my app's description",
                    ImageUrl = "my_logo.png"
                },
                new OnboardingOverviewModel
                {
                    Title = "A cool feature",
                    Content = "this is the cool feature's description",
                    ImageUrl = "onboarding_image.png"
                },
                new OnboardingOverviewModel
                {
                    Title = "Another Feature",
                    Content = "Here's another feature",
                    ImageUrl = "another_image.png"
                }
            };
        }
    }
}