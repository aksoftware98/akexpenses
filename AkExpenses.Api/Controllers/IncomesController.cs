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
using Microsoft.EntityFrameworkCore;

namespace AkExpenses.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IncomesController : ControllerBase
    {
        private const int PAGE_SIZE = 10;
        private readonly ApplicationDbContext db;

        public IncomesController(ApplicationDbContext db)
        {
            this.db = db;
        }

        #region Get

        //Gets all incomes for the logged in account
        [HttpGet]
        public async Task<IActionResult> Get(string query, int? page)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                query = "";
            }

            if (!page.HasValue || page <= 0)
            {
                page = 1;
            }

            //Get the logged in account
            var account = await getAccount();
            var totalIncomes = db.Incomes.Where(i => i.AccountId == account.Id).Count();

            //Get all incomes related to that account
            var incomes = db.Incomes.Where(i => i.AccountId == account.Id)
                .Include(i => i.Category)
                .Include(i => i.MoneyType)
                .OrderByDescending(o => o.PayDate)
                .ThenByDescending(o => o.CreatedDate)
                .Skip(PAGE_SIZE * (page.Value - 1))
                .Take(PAGE_SIZE);

            return Ok(new HttpCollectionResponse<object>
            {
                IsSuccess = true,
                Message = "Incomes have been retrieved successfully.",
                Count = totalIncomes,
                Page = page.Value,
                PageSize = PAGE_SIZE,
                SearchQuery = query,
                TotalPages = DataHelper.GetTotalPages(totalIncomes, PAGE_SIZE),
                Values = incomes.Select(i => new
                {
                    i.Id,
                    i.Title,
                    i.Description,
                    i.PayDate,
                    i.Amount,
                    i.MoneyTypeId,
                    i.CategoryId,
                    MoneyType = i.MoneyType.Name,
                    Category = i.Category.Name,
                    i.ProvidedId
                })
            });
        }

        //Get a specific incomes by id
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            //Validate the id
            if (string.IsNullOrWhiteSpace(id))
            {
                NotFound();
            }

            var income = await db.Incomes.FindAsync(id);

            if (income == null)
            {
                return NotFound();
            }

            return Ok(new HttpSingleResponse<Income>
            {
                IsSuccess = true,
                Message = "Income has been retrieved successfully.",
                Value = income
            });
        }

        #endregion

        #region Post

        //Adds a new income to the database
        [HttpPost]
        public async Task<IActionResult> Post(IncomeViewModel model)
        {
            //Validate the model
            if (ModelState.IsValid)
            {
                //Get money type
                var moneyType = await db.MoneyTypes.FindAsync(model.MoneyTypeId);

                if (moneyType == null)
                {
                    return NotFound();
                }

                //Get category
                var category = await db.Categories.FindAsync(model.CategoryId);

                if (category == null)
                {
                    return NotFound();
                }

                //Get the provider
                var provider = await db.Providers.FindAsync(model.ProviderId);

                if (provider == null)
                {
                    return NotFound();
                }

                //Get the logged in account
                var account = await getAccount();

                //Create the new income
                var newIncome = new Income
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = model.Title,
                    Description = model.Description,
                    Amount = model.Amount,
                    PayDate = model.PayDate,
                    CreatedDate = DateTime.UtcNow,
                    AccountId = account.Id,
                    CategoryId = model.CategoryId,
                    ProvidedId = model.ProviderId,
                    MoneyTypeId = model.MoneyTypeId
                };

                //Add the new income to the database
                await db.Incomes.AddAsync(newIncome);
                await db.SaveChangesAsync();

                return Ok(new HttpSingleResponse<Income>
                {
                    IsSuccess = true,
                    Message = "Income has been added successfully.",
                    Value = newIncome
                });
            }

            return this.FixedBadRequest("Model sent has some errors.");
        }

        #endregion

        #region Put

        //Update the info of a particular income
        [HttpPut]
        public async Task<IActionResult> Put(IncomeViewModel model)
        {
            //Validate the model
            if (ModelState.IsValid)
            {
                //Get the income
                var income = await db.Incomes.FindAsync(model.Id);

                if (income == null)
                {
                    return NotFound();
                }

                //Get the money type
                var moneyType = await db.MoneyTypes.FindAsync(model.MoneyTypeId);

                if (moneyType == null)
                {
                    return NotFound();
                }

                //Get the category
                var category = await db.Categories.FindAsync(model.CategoryId);
                if (category == null)
                {
                    return NotFound();
                }

                //Get the provider
                var provider = await db.Providers.FindAsync(model.ProviderId);

                if (provider == null)
                {
                    return NotFound();
                }

                //Update income data
                income.Title = model.Title;
                income.Description = model.Description;
                income.Amount = model.Amount;
                income.PayDate = model.PayDate;
                income.ProvidedId = provider.Id;
                income.CategoryId = category.Id;
                income.MoneyTypeId = moneyType.Id;

                //Save changes to the database
                await db.SaveChangesAsync();

                return Ok(new HttpSingleResponse<Income>
                {
                    IsSuccess = true,
                    Message = "Income has been updated successfully.",
                    Value = income
                });
            }

            return this.FixedBadRequest("Model sent has some errors.");
        }

        #endregion

        #region Delete

        //Deletes a specific income
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return this.FixedBadRequest("Id sent is invalid.");
            }

            //Get income
            var income = await db.Incomes.FindAsync(id);

            if (income == null)
            {
                return NotFound();
            }

            //Delete the income
            db.Incomes.Remove(income);

            await db.SaveChangesAsync();

            return Ok(new HttpSingleResponse<object>
            {
                IsSuccess = true,
                Message = "Income has been deleted successfully."
            });
        }

        #endregion

        #region Helper Functions

        private async Task<Account> getAccount()
        {
            var id = User.Claims.SingleOrDefault(c => c.Type == "AccountId").Value;
            return await db.Accounts.FindAsync(id);
        }

        #endregion
    }
}