using AkExpenses.Models.Interfaces;
using AkExpenses.Models.Shared;
using AKSoftware.WebApi.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AkExpenses.Services
{
    public class Auth
    {

        private readonly ServiceClient _service;
        private readonly IConfiguration _configuration;

        private readonly string _serviceUrl = null; 
        public Auth(ServiceClient service, IConfiguration configuration)
        {
            _service = service;
            this._configuration = configuration;
            _serviceUrl = _configuration.Dictionary["ApiUri"].ToString();
        }

        /// <summary>
        /// Register a new user into AK Expenses
        /// </summary>
        /// <param name="model">Data object contains the required data to create a new account</param>
        /// <returns></returns>
        public async Task<UserManageResponse> RegisterUserAsync(RegisterViewModel model)
        {
            try
            {
                return await _service.PostAsync<UserManageResponse>($"{_serviceUrl}/auth/register", model);
            }
            catch (Exception ex)
            {
                return new UserManageResponse
                {
                    Message = $"{ex.Message}, Please try again later",
                    IsSuccess = false
                }; 
            }
            //await Task.Delay(500); 
            //return new UserManageResponse
            //{
            //    IsSuccess = true,
            //    Message = "Your account has been created successfully",
            //    AccountId = "ahmad-4353"
            //};
        }

        /// <summary>
        /// Login an existing user a get an access token
        /// </summary>
        /// <param name="model">Login object that contains the username and password</param>
        /// <returns></returns>
        public async Task<bool> LoginUserAsync(string username, string password)
        {
            var result = await _service.PostAsync<UserManageResponse>($"{_serviceUrl}/auth/login", new LoginViewModel
            {
                Email = username,
                Password = password
            });

            if (result.IsSuccess)
            {
                // Save the AccessToken
                _configuration.SaveValue("AccessToken", result.Message);
                _configuration.SaveValue("AccessTokenExpire", result.ExpireDate);
                return true;
            }

            return false; 
        }


    }
}
