using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AkExpenses.Api.Data;
using AkExpenses.Models;
using AkExpenses.Models.Shared.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AkExpenses.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MoneyTypesController : ControllerBase
    {
        private readonly ApplicationDbContext db;

        public MoneyTypesController(ApplicationDbContext context)
        {
            this.db = context;
        }



        #region Get

        /// <summary>
        /// Gets all money types in the database
        /// </summary>
        /// <returns></returns>
        [Route("Get")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            //Get logged in account
            var account = await GetAccount();

            //Get money types
            var moneyTypes = db.MoneyTypes.Where(t => t.AccountId == account.Id).OrderByDescending(t => t.Name);

            //Create response
            var response = new AkExpenses.Models.Shared.HttpResponse<IEnumerable<MoneyType>>
            {
                Count = moneyTypes.Count(),
                IsSuccess = true,
                Message = "Money types have been retrieved successfully.",
                Value = moneyTypes
            };

            //Return result
            return Ok(response);
        }

        /// <summary>
        /// Gets a specific money type
        /// </summary>
        /// <param name="id">Id of the money type to get</param>
        /// <returns></returns>
        [Route("Get/{id}")]
        [HttpGet]
        public IActionResult Get(string id)
        {
            //Validate the id
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new AkExpenses.Models.Shared.HttpResponse<object>
                {
                    IsSuccess = false,
                    Message = "Id sent can't be empty."
                });
            }

            //Get the account
            var type = db.MoneyTypes.Find(id);

            if (type == null)
            {
                return NotFound(new AkExpenses.Models.Shared.HttpResponse<object>
                {
                    IsSuccess = false,
                    Message = "Money Type does not exist."
                });
            }

            return Ok(new AkExpenses.Models.Shared.HttpResponse<MoneyType>
            {
                Count = 1,
                IsSuccess = true,
                Message = $"Money type has been retrieved successfully.",
                Value = type
            });
        }

        #endregion

        #region Post

        [Route("Post")]
        [HttpPost]
        public async Task<IActionResult> Post(MoneyTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Get the account
                
            }

            return BadRequest(new AkExpenses.Models.Shared.HttpResponse<object>
            {
                IsSuccess = false,
                Message = "Model sent has some errors."
            });
        }

        #endregion

        #region Put



        #endregion

        #region Delete

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            //Validate the id
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            //Get Money type
            var type = await db.MoneyTypes.FindAsync(id);

            if (type == null)
            {
                return NotFound();
            }

            db.MoneyTypes.Remove(type);
            await db.SaveChangesAsync();

            return Ok(new
            {

            });
        }

        #endregion

        #region Helper Functions

        private async Task<Account> GetAccount()
        {
            string id = User.Claims.SingleOrDefault(s => s.Type == "AccountId").Value;
            return await db.Accounts.FindAsync(id);
        }

        #endregion
    }
}