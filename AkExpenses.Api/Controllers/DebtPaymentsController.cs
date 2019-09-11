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
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class DebtPaymentsController : ControllerBase
    {

        private readonly ApplicationDbContext _db;

        public DebtPaymentsController(ApplicationDbContext db)
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

        #region Get
        // GET: api/debtpayments/45345
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            // Get the debt 
            var debt = await _db.Debts.FindAsync(id);
            if (debt == null)
                return NotFound();

            // Get all the payments of the debt 
            var debtPayments = _db.DebtPayment.Where(d => d.DebtId == id).OrderByDescending(d => d.PaymentDate)
                                                                        .ThenByDescending(d => d.CreatedDate);

            return Ok(new HttpCollectionResponse<DebtPayment>
            {
                Count = debtPayments.Count(),
                IsSuccess = true, 
                Message = "Debt payment retrieved", 
                Values = debtPayments, 
            });
        }
        #endregion

        #region Create
        // POST: api/debtpayments
        [HttpPost]
        public async Task<IActionResult> Post(DebtPaymentViewModel model)
        {
            if(ModelState.IsValid)
            {
                // Check the debt 
                var debt = await _db.Debts.FindAsync(model.DebtId);
                if (debt == null)
                    return NotFound();

                // Check the total of the debt 
                var totalPayments = _db.DebtPayment.Where(d => d.DebtId == model.DebtId).Sum(d => d.Amount);
                if ((totalPayments + model.Amount) > debt.Amount)
                    return BadRequest(new
                    {
                        Message = $"Total amount of the debt is {debt.Amount} and with the current payment the total will be higher than the debt amount",
                        IsSuccess = false
                    });

                var debtPayment = await _db.DebtPayment.AddAsync(new DebtPayment
                {
                    Amount = model.Amount,
                    CreatedDate = DateTime.UtcNow,
                    DebtId = model.DebtId,
                    PaymentDate = model.PaymentDate.ToUniversalTime(),
                    Id = Guid.NewGuid().ToString()
                });

                if ((model.Amount + totalPayments) == debt.Amount)
                    debt.IsPaid = true;

                await _db.SaveChangesAsync();

                debtPayment.Entity.Debt = null;

                return Ok(new HttpSingleResponse<DebtPayment>
                {
                    Message = "Debt payment added successfully",
                    IsSuccess = true,
                    Value = debtPayment.Entity
                }); 
            }

            return BadRequest(new
            {
                Message = "Some properties are not valid",
                IsSuccess = false
            });
        }
        #endregion

        #region Edit
        // PUT: api/debtpayment
        [HttpPut]
        public async Task<IActionResult> Put(DebtPaymentViewModel model)
        {
            if(ModelState.IsValid)
            {
                // Get the debt 
                var debt = await _db.Debts.FindAsync(model.DebtId);
                if (debt == null)
                    return NotFound();

                // Check the total of the debt 
                var totalPayments = _db.DebtPayment.Where(d => d.DebtId == model.DebtId && d.Id != model.Id).Sum(d => d.Amount);
                if ((totalPayments + model.Amount) > debt.Amount)
                    return BadRequest(new
                    {
                        Message = $"Total amount of the debt is {debt.Amount} and with the current payment the total will be higher than the debt amount",
                        IsSuccess = false
                    });

                var payment = await _db.DebtPayment.FindAsync(model.Id);
                if (payment == null)
                    return NotFound();

                if ((payment.Amount + totalPayments) == debt.Amount)
                    debt.IsPaid = true; 

                // update the property 
                payment.Amount = model.Amount;
                payment.PaymentDate = model.PaymentDate.ToUniversalTime();

                await _db.SaveChangesAsync();

                return Ok(new HttpSingleResponse<DebtPayment>
                {
                    IsSuccess = true, 
                    Message = "Debt payment edited", 
                    Value = payment
                });
            }

            return BadRequest(new
            {
                Message = "Some properties are not valid",
                IsSuccess = false
            });
        }
        #endregion

        #region Delete
        // DELETE: api/debtpayment/4534
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var debtPayment = await _db.DebtPayment.FindAsync(id);
            if (debtPayment == null)
                return NotFound();

            _db.DebtPayment.Remove(debtPayment);
            await _db.SaveChangesAsync();

            return Ok(new HttpSingleResponse<DebtPayment>
            {
                IsSuccess = true,
                Message = "Debt payment deleted",
                Value = debtPayment
            });
        }
        #endregion 

    }
}