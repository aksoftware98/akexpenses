using AkExpenses.Api.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace AkExpenses.Api.Services
{
    public interface IUserService
    {

        Task<object> RegisterUser(RegisterViewModel model);

        Task<object> LoginUser(LoginViewModel model); 

    }

    public class UserService : IUserService
    {

        UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager; 
        }

        public Task<object> LoginUser(LoginViewModel model)
        {
            throw new NotImplementedException(); 
        }

        public async Task<object> RegisterUser(RegisterViewModel model)
        {
            // Validate the Models 
            string message = null;
            if (!v.Validation.IsEmail(model.Email))
                message = "Invalid email address";
            if (string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName))
                message = "Invalid first name or last name";
            if (string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.ConfirmPassword))
                message = "Invalid password or confirm password";
            if (model.Password != model.ConfirmPassword)
                message = "Entered password did not match the confirm password";

            if (message == null)
                return new
                {
                    Message = message,
                    IsSuccess = false,
                }; 

            var user = await _userManager.CreateAsync(new ApplicationUser
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
            }, model.Password);

            if (user.Succeeded)
                return new
                {
                    Message = "User has been created successfully!",
                    IsSuccess = true
                };

            return new
            {
                Message = "User did not create, There is something wrong",
                Errors = user.Errors.Select(e => e.Description),
                IsSuccess = false
            }; 
        }
    }
}
