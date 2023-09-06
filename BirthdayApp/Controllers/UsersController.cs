using BirthdayApp.Common.CustomExceptions;
using BirthdayApp.Data.Models;
using BirthdayApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BirthdayApp.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(IUserService userService, UserManager<ApplicationUser> userManager)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            try
            {
                var users = await _userService.GetUsersAsync(Cts.Token);
                //Do not show current user on the list. They shouldn't be able to start a voting for themselves
                var model = users.Where(x => x.Id != currentUser.Id).ToList();

                return View(model);
            }
            catch (DatabaseException dbEx)
            {
                return StatusCode(500, dbEx.Message);
            }
        }
    }
}
