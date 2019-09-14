using AkExpenses.Api.Data;
using AkExpenses.Api.Models;
using AkExpenses.Models.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace AkExpenses.Api.Services
{
    public interface IUserService
    {

        Task<UserManageResponse> RegisterUser(RegisterViewModel model);

        Task<UserManageResponse> LoginUser(LoginViewModel model); 

    }

    public class UserService : IUserService
    {

        UserManager<ApplicationUser> _userManager;
        IConfiguration _configuration;
        ApplicationDbContext _db; 

        public UserService(UserManager<ApplicationUser> userManager, IConfiguration configuration, ApplicationDbContext db)
        {
            _userManager = userManager;
            _configuration = configuration;
            _db = db; 
        }

        /// <summary>
        /// Login the user via its Email and Password and generate the access token required to access protected resources
        /// </summary>
        /// <param name="model">Model that wraps the Username and Password of the user</param>
        /// <returns></returns>
        public async Task<UserManageResponse> LoginUser(LoginViewModel model)
        {
            // Get the user by name 
            var user = await _userManager.FindByNameAsync(model.Email); 

            if(user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                // Get the account of the user 
                var account = _db.Accounts.SingleOrDefault(a => a.UserId == user.Id); 

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.FirstName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("AccountId", account.Id)
                };

                var authSettings = _configuration.GetSection("AuthSettings");
                var signInKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authSettings["Key"]));

                // Generate the token 
                var token = new JwtSecurityToken(
                    issuer: "http://ahmadmozaffar.net",
                    audience: "http://ahmadmozaffar.net",
                    expires: DateTime.Now.AddDays(30),
                    claims: claims,
                    signingCredentials: new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256Signature));

                return new UserManageResponse
                {
                    Message = new JwtSecurityTokenHandler().WriteToken(token),
                    IsSuccess = true,
                    ExpireDate = token.ValidTo
                };
            }

            return new UserManageResponse
            {
                IsSuccess = false,
                Message = "Username or password is invalid",
            }; 
        }

        /// <summary>
        /// Register a new user into AK Expenses and generate an account id 
        /// </summary>
        /// <param name="model">Model that wraps the requried user data of the user</param>
        /// <returns></returns>
        public async Task<UserManageResponse> RegisterUser(RegisterViewModel model)
        {
            // Validate the Models 
            string message = null;
            //if (!v.Validation.IsEmail(model.Email))
            //    message = "Invalid email address";
            if (string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName))
                message = "Invalid first name or last name";
            if (string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.ConfirmPassword))
                message = "Invalid password or confirm password";
            if (model.Password != model.ConfirmPassword)
                message = "Entered password did not match the confirm password";

            if (message != null)
                return new UserManageResponse
                {
                    Message = message,
                    IsSuccess = false,
                };

            var user = new ApplicationUser
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var random = new Random(DateTime.Now.Millisecond);
                string accountName = $"{model.FirstName}-{model.LastName}{random.Next(10000, 99999)}";
                // Create a new Account 
                await _db.Accounts.AddAsync(new AkExpenses.Models.Account
                {
                    Id = Guid.NewGuid().ToString(), 
                    Name = accountName, 
                    Description = "Financial Account",
                    UserId = user.Id
                });
                await _db.SaveChangesAsync(); 

                return new UserManageResponse
                {
                    Message = $"Account has been created successfully with the ID: {accountName}!",
                    IsSuccess = true,
                    AccountId = accountName
                };
            }

            return new UserManageResponse
            {
                Message = "User did not create, There is something wrong",
                Errors = result.Errors.Select(e => e.Description),
                IsSuccess = false
            }; 
        }
    }
}
