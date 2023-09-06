﻿using BirthdayApp.Common.CustomExceptions;
using BirthdayApp.Data.Models;
using BirthdayApp.Services.Interfaces;
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
                //Do not show votings for the current user on the list.
                var model = activeVotings.Where(v => v.BirthdayPersonId != currentUser.Id).ToList();

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
                //Do not show votings for the current user on the list.
                var model = finishedVotings.Where(v => v.BirthdayPersonId != currentUser.Id).ToList();

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
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            try
            {
                await _votingService.StartVotingAsync(birthdayPersonId, currentUser.Id, Cts.Token);

                return RedirectToAction(nameof(this.AllActive));
            }
            catch (ValidationException validationEx)
            {
                return BadRequest(validationEx.Message);
            }
            catch (EntityNotFoundException notFoundEx)
            {
                return NotFound(notFoundEx.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EndVoting(int votingId)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            try
            {
                await _votingService.EndVotingAsync(votingId, currentUser.Id, Cts.Token);

                return RedirectToAction(nameof(this.AllFinished));
            }
            catch (ValidationException validationEx)
            {
                return BadRequest(validationEx.Message);
            }
            catch (EntityNotFoundException notFoundEx)
            {
                return NotFound(notFoundEx.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Vote(int votingId, int presentId)
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            try
            {
                await _votingService.VoteAsync(votingId, presentId, currentUser.Id, Cts.Token);

                return Ok("Voted successfully");
            }
            catch (ValidationException validationEx)
            {
                return BadRequest(validationEx.Message);
            }
            catch (EntityNotFoundException notFoundEx)
            {
                return NotFound(notFoundEx.Message);
            }
        }
    }
}
