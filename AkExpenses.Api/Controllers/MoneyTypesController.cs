using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AkExpenses.Api.Data;
using AkExpenses.Models;
using AkExpenses.Models.Shared;
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
       
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            //Get logged in account
            var account = await getAccount();

            //Get money types
            var moneyTypes = db.MoneyTypes.Where(t => t.AccountId == account.Id).OrderByDescending(t => t.Name);

            //Create response
            var response = new HttpCollectionResponse<object>
            {
                Count = moneyTypes.Count(),
                IsSuccess = true,
                Message = "Money types have been retrieved successfully.",
                Values = moneyTypes.Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.Description
                })
            };

            //Return result
            return Ok(response);
        }

        /// <summary>
        /// Gets a specific money type
        /// </summary>
        /// <param name="id">Id of the money type to get</param>
        /// <returns></returns>
        [Route("{id}")]
        [HttpGet]
        public IActionResult Get(string id)
        {
            //Validate the id
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new HttpSingleResponse<object>
                {
                    IsSuccess = false,
                    Message = "Id sent can't be empty."
                });
            }

            //Get the account
            var type = db.MoneyTypes.Find(id);

            if (type == null)
            {
                return NotFound();
            }

            return Ok(new HttpSingleResponse<MoneyType>
            {
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
                var account = await getAccount();

                var oldType = db.MoneyTypes.SingleOrDefault(t => t.Name == model.Name.Trim());

                if (oldType != null)
                {
                    return BadRequest(new HttpSingleResponse<object>
                    {
                        IsSuccess = false,
                        Message = "Type already exists."
                    });
                }

                //Create new Money type
                var newType = new MoneyType
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = model.Name.Trim(),
                    Description = model.Description == null ? null : model.Description.Trim(),
                    AccountId = account.Id
                };

                await db.MoneyTypes.AddAsync(newType);
                await db.SaveChangesAsync();

                return Ok(new HttpSingleResponse<MoneyType>
                {
                    IsSuccess = true,
                    Message = "Type has been added successfully.",
                    Value = newType
                });
            }

            return BadRequest(new HttpSingleResponse<object>
            {
                IsSuccess = false,
                Message = "Model sent has some errors."
            });
        }

        #endregion

        #region Put

        //Edits the data of a specfic money type
        [HttpPut]
        public async Task<IActionResult> Put(MoneyTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                //Get type
                var type = db.MoneyTypes.Find(model.MoneyTypeId);

                if (type == null)
                {
                    return NotFound();
                }

                var oldType = db.MoneyTypes
                    .SingleOrDefault(t => t.Name == model.Name.Trim() && t.Id != model.MoneyTypeId);

                if (oldType != null)
                {
                    return BadRequest(new HttpSingleResponse<object>
                    {
                        IsSuccess = false,
                        Message = $"Type already exists with the name: {oldType.Name}."
                    });
                }

                //Edit type info
                type.Name = model.Name.Trim();
                type.Description = model.Description == null ? null : model.Description.Trim();

                await db.SaveChangesAsync();

                return Ok(new HttpSingleResponse<MoneyType>
                {
                    IsSuccess = true,
                    Message = "Money type has been edited successfully.",
                    Value = type
                });
            }

            return BadRequest(new HttpSingleResponse<object>
            {
                IsSuccess = false,
                Message = "Model sent has some errors."
            });
        }

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

            return Ok(new HttpSingleResponse<object>
            {
                IsSuccess = true,
                Message = "Money type has been deleted successfully."
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