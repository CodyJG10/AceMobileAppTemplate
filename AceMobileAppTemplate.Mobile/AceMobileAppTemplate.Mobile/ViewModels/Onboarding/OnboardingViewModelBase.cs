using AceMobileAppTemplate.Mobile.Models;
using AceMobileAppTemplate.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace AceMobileAppTemplate.Mobile.ViewModels.Onboarding
{
    public class OnboardingViewModelBase : BaseViewModel
    {
        private int position;
        public int Position
        {
            get => position;
            set
            {
                position = value;
                OnPropertyChanged(nameof(Position));
                UpdateSkipButtonText();
            }
        }

        public ObservableCollection<OnboardingOverviewModel> Items { get; set; }

        private string skipButtonText;
        public string SkipButtonText
        {
            get => skipButtonText;
            set
            {
                skipButtonText = value;
                OnPropertyChanged(nameof(SkipButtonText));
            }
        }

        public ICommand SkipCommand { get; private set; }

        public OnboardingViewModelBase()
        {
            SkipButtonText = "SKIP";
            InitializeOnBoarding();
            InitializeSkipCommand();
        }

        protected virtual void InitializeOnBoarding() { }

        private void InitializeSkipCommand()
        {
            SkipCommand = new Command(() =>
            {
                if (LastPositionReached())
                {
                    ExitOnBoarding();
                }
                else
                {
                    MoveToNextPosition();
                }
            });
        }

        private static void ExitOnBoarding()
            => Application.Current.MainPage.Navigation.PopModalAsync();

        private void MoveToNextPosition()
        {
            var nextPosition = ++Position;
            Position = nextPosition;
        }

        private bool LastPositionReached()
            => Position == Items.Count - 1;

        private void UpdateSkipButtonText()
        {
            if (LastPositionReached())
            {
                SkipButtonText = "GOT IT";
            }
            else
            {
                SkipButtonText = "SKIP";
            }
        }
    }
}