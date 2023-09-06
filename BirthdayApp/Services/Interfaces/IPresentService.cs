using BirthdayApp.ViewModels;

namespace BirthdayApp.Services.Interfaces
{
    public interface IPresentService
    {
        Task<IList<PresentViewModel>> GetPresentsAsync(CancellationToken cancellationToken);
    }
}
