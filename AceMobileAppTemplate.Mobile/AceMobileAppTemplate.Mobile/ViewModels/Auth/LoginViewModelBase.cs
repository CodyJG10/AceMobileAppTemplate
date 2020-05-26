using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace AceMobileAppTemplate.ViewModels.Auth
{
    public class LoginViewModelBase : BaseLoadingViewModel
    {
        #region Fields

        private string email;
        private bool isInvalidEmail;

        #endregion

        #region Property

        public string Email
        {
            get
            {
                return this.email;
            }

            set
            {
                if (this.email == value)
                {
                    return;
                }

                this.email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public bool IsInvalidEmail
        {
            get
            {
                return this.isInvalidEmail;
            }

            set
            {
                if (this.isInvalidEmail == value)
                {
                    return;
                }

                this.isInvalidEmail = value;
                OnPropertyChanged(nameof(IsInvalidEmail));
            }
        }

        public ICommand LoginCommand { get; set; }

        public ICommand RegisterCommand { get; set; }

        #endregion
    }
}
