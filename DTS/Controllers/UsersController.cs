﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL.Data;
using DAL.Models;
using DAL.Repositories;
using DTS.API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using DTS.API.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace DTS.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRepositoryWrapper repository;
        private readonly IUserService userService;
        private readonly ILogger<UsersController> logger;

        public UsersController(IRepositoryWrapper repository, IUserService userService, ILogger<UsersController> logger)
        {
            this.repository = repository;
            this.userService = userService;
            this.logger = logger;
        }

        private void LogBeginOfRequest()
        {
            logger.LogInformation("User id: {userId} type: {userType}, start request handling.",
                GetUserIdFromToken(),
                GetUserTypeFromToken()
                );
        }

        private void LogEndOfRequest(string message, int status)
        {
            logger.LogInformation("status: {status} : {message}",
                status,
                message
                );
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            LogBeginOfRequest();
            try
            {
                var query = new GetUsersQuery();
                var users = await userService.GetUsersQuery.HandleAsync(query);
                LogEndOfRequest($"Success {users.Count} elements found", 200);
                return Ok(users);
            }
            catch (InvalidOperationException)
            {
                LogEndOfRequest($"Failed user list is empty", 404);
                return NotFound();
            }
        }

        [HttpGet("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUser([FromRoute] int id)
        {
            LogBeginOfRequest();
            if (!ModelState.IsValid)
            {
                LogEndOfRequest("Bad request", 400);
                return BadRequest(ModelState);
            }
            try
            {
                var query = new GetUserByIdQuery(id);
                var user = await userService.GetUserByIdQuery.HandleAsync(query);
                LogEndOfRequest($"Success return {user}", 200);
                return Ok(user);
            }
            catch (KeyNotFoundException e)
            {
                LogEndOfRequest($"Failed user with id {id} not found", 404);
                return NotFound(e.Message);
            }
        }

        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsersByStatus(string status)
        {
            LogBeginOfRequest();
            try
            {
                var query = new GetUsersByStatusQuery(status);
                var users = await userService.GetUsersByStatusQuery.HandleAsync(query);
                LogEndOfRequest($"Success {users.Count} elements found", 200);
                return Ok(users);
            }
            catch (InvalidOperationException)
            {
                string errorMessage = $"No users with {status} status or is invalid";
                LogEndOfRequest(errorMessage, 404);
                return NotFound(errorMessage);
            }
        }

        [HttpGet("type/{type}")]
        [Authorize(Roles = "Admin, Editor")]
        public async Task<IActionResult> GetUsersByType(string type)
        {
            try
            {
                var query = new GetUsersByTypeQuery(type);
                var users = await userService.GetUsersByTypeQuery.HandleAsync(query);
                return Ok(users);
            }
            catch (InvalidOperationException)
            {
                return NotFound($"No users with {type} type or is invalid");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ChangeUserPersonalData([FromRoute] int id, [FromBody] UserDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!VerifyIfUserIdEqualsTokenClaimName(id) && !IsUserAdmin())
            {
                return BadRequest();
            }

            var command = new ChangeUserPersonalDataCommand(
                id,
                user.Name,
                user.Surname,
                user.Email
                );

            try
            {
                await userService.ChangeUserPersonalDataCommand.HandleAsync(command);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(503, "Server overload try again later");
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        private bool IsUserAdmin()
        {
            var userType = GetUserTypeFromToken();
            return userType.Equals("Admin");
        }

        private bool VerifyIfUserIdEqualsTokenClaimName(int id)
        {
            var userId = GetUserIdFromToken();
            if (userId == 0 || userId != id)
            {
                return false;
            }
            return true;
        }


        [HttpPut("{id}/type/{type}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeUserType(int id, string type)
        {
            var command = new ChangeUserTypeCommand(id, type);

            if (!VerifyIfUserIdEqualsTokenClaimName(id))
            {
                return BadRequest();
            }

            try
            {
                await userService.ChangeUserTypeCommand.HandleAsync(command);
                return NoContent();
            } catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPut("{id}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var command = new ActivateUserCommand(id);
                await userService.ActivateUserCommand.HandleAsync(command);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BlockUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!VerifyIfUserIdEqualsTokenClaimName(id))
            {
                return BadRequest();
            }

            try
            {
                var command = new BlockUserCommand(id);
                await userService.BlockUserCommand.HandleAsync(command);
                return Ok();
            } catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        private int GetUserIdFromToken()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            var idString = claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault()?.Value;
            if (idString != null)
            {
                return int.Parse(idString);
            }
            return 0;
        }

        private string GetUserTypeFromToken()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;
            var type = claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault()?.Value;
            if (type != null)
            {
                return type;
            }
            return null;
        }
    }
}