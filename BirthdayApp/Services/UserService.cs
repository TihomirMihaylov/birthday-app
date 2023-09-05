using BirthdayApp.Common.CustomExceptions;
using BirthdayApp.Data.Models;
using BirthdayApp.Data.Repositories;
using BirthdayApp.Services.Interfaces;
using BirthdayApp.ViewModels;

namespace BirthdayApp.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IRepository<ApplicationUser> _userRepository;

        public UserService(ILogger<UserService> logger, IRepository<ApplicationUser> userRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<IList<UserViewModel>> GetUsersAsync(CancellationToken cancellationToken)
        {
            try
            {
                var allUsers = await _userRepository.AllAsNoTrackingAsync(cancellationToken);
                return allUsers.Select(x => new UserViewModel
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Birthday = x.Birthday
                }).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error occured getting all users from DB. Error: {Message}", ex.Message);

                throw new DatabaseException("Could not load users from the database");
            }
        }
    }
}
