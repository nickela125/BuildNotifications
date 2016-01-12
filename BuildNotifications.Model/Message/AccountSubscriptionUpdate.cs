using System.Collections.Generic;

namespace BuildNotifications.Model.Message
{
    public class AccountSubscriptionUpdate
    {
        public IList<Account> Accounts { get; set; }
    }
}
