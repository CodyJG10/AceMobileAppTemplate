using Autofac;
using AceMobileAppTemplate.Api;
using AceMobileAppTemplate.Models;
using Newtonsoft.Json;
using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace AceMobileAppTemplate.ViewModels
{
    public class BaseDataViewModel : BaseViewModel
    {
        protected IDatabaseManager _db;
        protected UserData _user;

        public BaseDataViewModel()
        {
            _db = App.Container.Resolve<IDatabaseManager>();
            _user = JsonConvert.DeserializeObject<UserData>(CrossSettings.Current.GetValueOrDefault("UserData", ""));
        }
    }
}