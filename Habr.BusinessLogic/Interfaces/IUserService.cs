using Habr.DataAccess.Entities;

namespace Habr.BusinessLogic.Interfaces
{
    public interface IUserService
    {
        Task<User> RegisterAsync(string email, string password, bool isEmailConfirmed);
        Task ConfirmEmailAsync(string email, bool isEmailConfirmed);
        Task<User> AuthenticateAsync(string email, string password);
    }
}
