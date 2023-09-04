using BirthdayApp.Common.CustomExceptions;
using BirthdayApp.Data.Models;
using BirthdayApp.Services.Interfaces;
using BirthdayApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BirthdayApp.Controllers
{
    [Authorize]
    public class VotingsController : BaseController
    {
        private readonly IVotingService _votingService;
        private readonly UserManager<ApplicationUser> _userManager;

        public VotingsController(IVotingService votingService, UserManager<ApplicationUser> userManager)
        {
            _votingService = votingService ?? throw new ArgumentNullException(nameof(votingService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [HttpGet]
        public async Task<IActionResult> AllActive()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            try
            {
                var activeVotings = await _votingService.GetActiveVotingsAsync(Cts.Token);

                var model = new ActiveVotingPageViewModel()
                {
                    //Do not show votings for the current user on the list.
                    Votings = activeVotings.Where(v => v.BirthdayPersonId != currentUser.Id).ToList()
                };

                return View(model);
            }
            catch (DatabaseException dbEx)
            {
                return StatusCode(500, dbEx.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> AllFinished()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            try
            {
                var finishedVotings = await _votingService.GetFinishedVotingsAsync(Cts.Token);

                var model = new FinishedVotingPageViewModel()
                {
                    //Do not show votings for the current user on the list.
                    Votings = finishedVotings.Where(v => v.BirthdayPersonId != currentUser.Id).ToList()
                };

                return View(model);
            }
            catch (DatabaseException dbEx)
            {
                return StatusCode(500, dbEx.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> StartVoting(string birthdayPersonId)
        {
            await _votingService.StartVotingAsync(birthdayPersonId, Cts.Token);
            return RedirectToAction(nameof(this.AllActive));
        }

        [HttpPost]
        public async Task<IActionResult> EndVoting(int votingId)
        {
            await _votingService.EndVotingAsync(votingId, Cts.Token);
            return RedirectToAction(nameof(this.AllFinished));
        }

        [HttpPost]
        public async Task<IActionResult> Vote(int votingId, int presentId)
        {
            await _votingService.VoteAsync(votingId, presentId, Cts.Token);
            return Ok("Voted successfully");
        }
    }
}
