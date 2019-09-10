using System;
using System.Collections.Generic;
using System.IO;
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
    [Authorize]
    [ApiController]
    public class DebtsController : ControllerBase
    {

        const int PAGE_SIZE = 10;

        private readonly ApplicationDbContext _db;

        public DebtsController(ApplicationDbContext db)
        {
            _db = db;
        }

        #region HelperMethod

        private async Task<Account> getAccount()
        {
            var accountId = User.Claims.SingleOrDefault(c => c.Type == "AccountId").Value;
            return await _db.Accounts.FindAsync(accountId);
        }


        #endregion

        #region GET
        // Get all the bills of a specific user 
        // api/bills?query=&page=1
        [HttpGet]
        public async Task<IActionResult> Get(string query, int? page)
        {
            if (string.IsNullOrWhiteSpace(query))
                query = "";

            if (!page.HasValue || page <= 0)
                page = 1;

            var account = await getAccount();
            int totalCount = _db.Debts.Where(b => b.AccountId == account.Id).Count();

            var bills = _db.Debts.Where(b => b.Title.Contains(query) && b.AccountId == account.Id)
                            .OrderByDescending(b => b.DebtDate)
                            .ThenByDescending(b => b.CreatedDate)
                            .Skip(PAGE_SIZE * (page.Value - 1))
                            .Take(PAGE_SIZE);

            return Ok(new HttpCollectionResponse<object>
            {
                Count = bills.Count(),
                IsSuccess = true,
                Message = "Debts retrieved",
                Page = page.Value,
                PageSize = PAGE_SIZE,
                SearchQuery = query,
                TotalPages = DataHelper.GetTotalPages(totalCount, PAGE_SIZE),
                Values = bills.Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.CreatedDate,
                    b.Description,
                    b.DebtDate,
                    b.AccountId,
                    b.IsPaid
                })
            });
        }

        // Get specific debt 
        //  GET: api/bills/435g
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var debt = await _db.Debts.FindAsync(id);
            if (debt == null)
                return NotFound();

            return Ok(new HttpSingleResponse<Debt>
            {
                IsSuccess = true,
                Message = "Debt retrieved",
                Value = debt,
            });
        }
        #endregion

        #region Create

        // POST: api/bills
        [HttpPost]
        public async Task<IActionResult> Post(DebtViewModel model)
        {
            if (ModelState.IsValid)
            {
                var account = await getAccount();

                // Check if there is a file 


                var debt = await _db.AddAsync(new Debt
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = model.Title,
                    AccountId = account.Id,
                    DebtDate = model.DebtDate.ToUniversalTime(),
                    CreatedDate = DateTime.UtcNow,
                    Description = model.Description,
                    Provider = model.Provider,
                    IsPaid = model.IsPaid,
                    Amount = model.Amount
                });

                await _db.SaveChangesAsync();

                return Ok(new HttpSingleResponse<Debt>
                {
                    IsSuccess = true,
                    Message = "Debt added",
                    Value = debt.Entity
                });

            }

            return BadRequest(new HttpSingleResponse<object>
            {
                IsSuccess = false,
                Message = "Some fields are not valid",
            });
        }

        #endregion

        #region Edit
        // PUT api/bills
        [HttpPut]
        public async Task<IActionResult> Put(DebtViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(model.Id))
                    return NotFound();

                var debt = await _db.Debts.FindAsync(model.Id);
                if (debt == null)
                    return NotFound();

                debt.Title = model.Title;
                debt.Description = model.Description;
                debt.DebtDate = model.DebtDate.ToUniversalTime();
                debt.Provider = model.Provider;
                debt.IsPaid = model.IsPaid;
                debt.Amount = model.Amount;
                await _db.SaveChangesAsync();

                return Ok(new HttpSingleResponse<Debt>
                {
                    IsSuccess = true,
                    Message = "Debt edited",
                    Value = debt
                });
            }

            // Invalid object
            return BadRequest(new
            {
                Message = "Some propreties are not valid",
                IsSuccess = false
            });
        }

        #endregion

        #region Delete

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var debt = await _db.Debts.FindAsync(id);
            if (debt == null)
                return NotFound();

            _db.Debts.Remove(debt);
            await _db.SaveChangesAsync();

            return Ok(new HttpSingleResponse<Debt>
            {
                IsSuccess = true,
                Value = debt,
                Message = "Debt deleted"
            });
        }
        #endregion 

    }
}