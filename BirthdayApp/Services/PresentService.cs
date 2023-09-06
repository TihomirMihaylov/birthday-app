using BirthdayApp.Common.CustomExceptions;
using BirthdayApp.Data.Models;
using BirthdayApp.Data.Repositories;
using BirthdayApp.Services.Interfaces;
using BirthdayApp.ViewModels;

namespace BirthdayApp.Services
{
    public class PresentService : IPresentService
    {
        private readonly ILogger<PresentService> _logger;
        private readonly IRepository<Present> _presentRepository;

        public PresentService(ILogger<PresentService> logger, IRepository<Present> presentRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _presentRepository = presentRepository ?? throw new ArgumentNullException(nameof(presentRepository));
        }

        public async Task<IList<PresentViewModel>> GetPresentsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var allPresents = await _presentRepository.AllAsNoTrackingAsync(cancellationToken);
                return allPresents.Select(x => new PresentViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected error occured getting all presents from DB. Error: {Message}", ex.Message);

                throw new DatabaseException("Could not load presents from the database");
            }
        }
    }
}
