using BirthdayApp.Common.CustomExceptions;
using BirthdayApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BirthdayApp.Controllers
{
    [Authorize]
    public class PresentsController : BaseController
    {
        private readonly IPresentService _presentService;

        public PresentsController(IPresentService presentService)
        {
            _presentService = presentService ?? throw new ArgumentNullException(nameof(presentService));
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            try
            {
                var model = await _presentService.GetPresentsAsync(Cts.Token);

                return View(model);
            }
            catch (DatabaseException dbEx)
            {
                return StatusCode(500, dbEx.Message);
            }
        }
    }
}
