using AceMobileAppTemplate.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace AceMobileAppTemplate.Dependencies
{
    public interface INotificationRegistration
    {
        void RegisterForRemoteNotifications(string username, IDatabaseManager db);
    }
}
