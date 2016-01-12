using System.Collections.Generic;
using System.Threading.Tasks;
using BuildNotifications.Model;

namespace BuildNotifications.Interface.Service
{
    public interface IAccountService
    {
        Task UpdateAccountDetails(Account account);
        void UpdateAccountSubsciptions(IList<Account> updatedAccounts);
        IList<Account> GetAccounts();
        void RemoveAccount(Account account);
    }
}
