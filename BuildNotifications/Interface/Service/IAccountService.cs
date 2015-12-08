﻿using System.Collections.Generic;
using System.Threading.Tasks;
using BuildNotifications.Model;

namespace BuildNotifications.Interface.Service
{
    public interface IAccountService
    {
        Task UpdateAccount(VsoAccount account);
        IList<VsoAccount> GetAccounts();
        void SaveAccounts(IList<VsoAccount> accounts);
        
        //TODO these will be saved per build later - here temporarily
        bool GetNotifyOnStart();
        bool GetNotifyOnFinish();
        void SaveNotifyOptions(bool? notifyOnStart, bool? notifyOnFinish);
    }
}
