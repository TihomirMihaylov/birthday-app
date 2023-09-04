using BirthdayApp.Common.CustomExceptions;
using BirthdayApp.Services.Interfaces;
using BirthdayApp.ViewModels;

namespace BirthdayApp.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;

        public UserService(ILogger<UserService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<IList<UserViewModel>> GetUsersAsync(CancellationToken cancellationToken)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error occured getting all users from DB. Error: {Message}", ex.Message);

                throw new DatabaseException("Could not load users from the database");
            }


            throw new NotImplementedException();
        }
    }
}
