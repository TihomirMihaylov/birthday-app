using BirthdayApp.ViewModels;

namespace BirthdayApp.Services.Interfaces
{
    public interface IUserService
    {
        Task<IList<UserViewModel>> GetUsersAsync(CancellationToken cancellationToken);
    }
}
