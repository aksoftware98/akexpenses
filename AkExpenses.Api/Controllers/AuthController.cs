using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AkExpenses.Api.Models;
using AkExpenses.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AkExpenses.Api.Controllers
{
    [Route("api/{controller}")]
    public class AuthController : Controller
    {

        private readonly IMailService _mail;
        private readonly IUserService _userService;

        public AuthController(IMailService mail, IUserService userService)
        {
            _mail = mail;
            _userService = userService;
        }

        // POST: auth/register
        [Route("Register")]
        [HttpPost()]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                var result = await _userService.RegisterUser(model);

                if (result.IsSuccess)
                {
                    // Send an email to the user 
                    await _mail.SendMailAsync("Welcome to AK Expenses", 
                                               Utitlity.HtmlGenerator.GetRegisterHTML($"{model.FirstName} {model.LastName}", result.AccountId), 
                                               model.Email);
                    return Ok(result);
                }

                return BadRequest();
            }

            return BadRequest(new UserManageResponse
            {
                Message = "Please enter a valid data",
                IsSuccess = false
            });
        }

        // POST: auth/login
        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.LoginUser(model);

                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result); 
            }

            return BadRequest(new UserManageResponse
            {
                Message = "Username or password is invalid",
                IsSuccess = false
            });
        }
    }
}