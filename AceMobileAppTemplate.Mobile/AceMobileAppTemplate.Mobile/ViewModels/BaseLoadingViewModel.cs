using System;
using System.Collections.Generic;
using System.Text;

namespace AceMobileAppTemplate.ViewModels
{
    public class BaseLoadingViewModel : BaseDataViewModel
    {
        private bool _loading = false;
        public bool Loading
        {
            get
            {
                return _loading;
            }
            set
            {
                _loading = value;
                OnPropertyChanged(nameof(Loading));
            }
        }
    }
}
