using System.Threading.Tasks;
using BuildNotifications.Model;

namespace BuildNotifications.Interface.Service
{
    public interface IAccountService
    {
        Task UpdateAccount(VsoAccount account);
    }
}
