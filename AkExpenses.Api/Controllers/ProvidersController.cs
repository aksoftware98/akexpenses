using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AkExpenses.Api.Data;
using AkExpenses.Api.Utitlity;
using AkExpenses.Models;
using AkExpenses.Models.Shared;
using AkExpenses.Models.Shared.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AkExpenses.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProvidersController : ControllerBase
    {
        private readonly ApplicationDbContext db;

        public ProvidersController(ApplicationDbContext context)
        {
            this.db = context;
        }

        #region Get

        //Gets all providers for currently logged in account
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            //Get the account
            var account = await getAccount();

            var providers = db.Providers.Where(p => p.AccountId == account.Id).OrderByDescending(p => p.Name);

            return Ok(new HttpCollectionResponse<object>
            {
                IsSuccess = true,
                Message = $"Providers for account {account.Name} have been retrieved.",
                Count = providers.Count(),
                Values = providers.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.CreatedDate
                })
            });
        }

        //Gets a spcecifc provider
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            //Get the provider
            var provider = await db.Providers.FindAsync(id);

            if (provider == null)
            {
                return NotFound();
            }

            return Ok(new HttpSingleResponse<Provider>
            {
                IsSuccess = true,
                Message = "Provider has been retrieved successfully.",
                Value = provider
            });
        }

        #endregion

        #region Post

        //Creates a new provider for the logged in account
        [HttpPost]
        public async Task<IActionResult> Post(ProviderViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Get the account
                var account = await getAccount();

                var oldProvider = db.Providers.SingleOrDefault(p => p.Name == model.Name.Trim());

                if (oldProvider != null)
                {
                    return this.FixedBadRequest($"Provider with name {oldProvider.Name} already exists.");
                }

                var newProvider = new Provider
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = model.Name.Trim(),
                    AccountId = account.Id
                };

                await db.Providers.AddAsync(newProvider);
                await db.SaveChangesAsync();

                return Ok(new HttpSingleResponse<Provider>
                {
                    IsSuccess = true,
                    Message = "Provide has been added successfully.",
                    Value = newProvider
                });
            }

            return this.FixedBadRequest("Sent model has some error.");
        }

        #endregion

        #region Put

        //Edit the data of an existing provider
        [HttpPut]
        public async Task<IActionResult> Put(ProviderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var provider = await db.Providers.FindAsync(model.ProviderId);

                if (provider == null)
                {
                    return NotFound();
                }

                var oldProvider = db.Providers
                    .SingleOrDefault(p => p.Name == model.Name.Trim() && p.Id != model.ProviderId);

                if (oldProvider != null)
                {
                    return this.FixedBadRequest($"Provider with name: {oldProvider.Name} already exists.");
                }

                provider.Name = model.Name.Trim();

                return Ok(new HttpSingleResponse<Provider>
                {
                    IsSuccess = true,
                    Message = "Provider has been updated successfully.",
                    Value = provider
                });
            }

            return this.FixedBadRequest("Model sent has some errors.");
        }

        #endregion

        #region Delete

        //Deletes a specific provider
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var provider = await db.Providers.FindAsync(id);

            if (provider == null)
            {
                return NotFound();
            }

            db.Providers.Remove(provider);
            await db.SaveChangesAsync();

            return Ok(new HttpSingleResponse<Provider>
            {
                IsSuccess = true,
                Message = "Provider has been deleted successfully.",
                Value = provider
            });
        }

        #endregion

        #region Helper Functions

        private async Task<Account> getAccount()
        {
            string id = User.Claims.SingleOrDefault(s => s.Type == "AccountId").Value;
            return await db.Accounts.FindAsync(id);
        }

        #endregion
    }
}