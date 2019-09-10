using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AkExpenses.Api.Data;
using AkExpenses.Api.Utitlity;
using AkExpenses.Models;
using AkExpenses.Models.Shared;
using AkExpenses.Models.Shared.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO; 

namespace AkExpenses.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillsController : ControllerBase
    {
        const int PAGE_SIZE = 10;

        private readonly ApplicationDbContext _db;

        public BillsController(ApplicationDbContext db)
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
            int totalCount = _db.Bills.Where(b => b.AccountId == account.Id).Count();

            var bills = _db.Bills.Where(b => b.Number.Contains(query) && b.AccountId == account.Id)
                            .OrderByDescending(b => b.BillDate)
                            .ThenByDescending(b => b.CreatedDate)
                            .Skip(PAGE_SIZE * (page.Value - 1))
                            .Take(PAGE_SIZE);

            return Ok(new HttpCollectionResponse<object>
            {
                Count = bills.Count(),
                IsSuccess = true,
                Message = "Bills retrieved",
                Page = page.Value,
                PageSize = PAGE_SIZE,
                SearchQuery = query,
                TotalPages = DataHelper.GetTotalPages(totalCount, PAGE_SIZE),
                Values = bills.Select(b => new
                {
                    b.Id,
                    b.Number,
                    b.CreatedDate,
                    b.Description,
                    b.Attachment,
                    b.BillDate,
                    b.AccountId,
                })
            });
        }

        // Get specific bill 
        //  GET: api/bills/435g
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var bill = await _db.Bills.FindAsync(id);
            if (bill == null)
                return NotFound();

            return Ok(new HttpSingleResponse<Bill>
            {
                IsSuccess = true,
                Message = "Bill retrieved",
                Value = bill,
            });
        }
        #endregion

        #region Create

        // POST: api/bills
        [HttpPost]
        public async Task<IActionResult> Post([FromForm]BillViewModel model)
        {
            if (ModelState.IsValid)
            {
                var account = await getAccount();

                // Check if there is a file 


                var bill = await _db.AddAsync(new Bill
                {
                    Id = Guid.NewGuid().ToString(),
                    Number = model.Number,
                    AccountId = account.Id,
                    BillDate = model.BillDate.ToUniversalTime(),
                    CreatedDate = DateTime.UtcNow,
                    Description = model.Description,
                    Provider = model.Provider
                });
                await _db.SaveChangesAsync();

                return Ok(new HttpSingleResponse<Bill>
                {
                    IsSuccess = true,
                    Message = "Bill added",
                    Value = bill.Entity
                });

            }

            return BadRequest(new HttpSingleResponse<object>
            {
                IsSuccess = false,
                Message = "Some fields are not valid",
            });
        }

        // POST: api/bills
        [HttpPost("{id}"), DisableRequestSizeLimitAttribute]
        public async Task<IActionResult> Post(string id, IFormFile file)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();
            
            // get the bill 
            var bill = await _db.Bills.FindAsync(id);
            if (bill == null)
                return NotFound();

            if (file == null || file.FileName == "")
            {
                return BadRequest();
            }


            // Upload the file 
            string extension = Path.GetExtension(file.FileName);
            // check the extension 
            var allowedExtensions = DataHelper.AllowedDocumentsExtensions();
            if (!allowedExtensions.Contains(extension))
                return BadRequest(new
                {
                    Message = "The uploaded file is not supported",
                    IsSuccess = false,
                });

            string directoryPath = Directory.GetCurrentDirectory() + "/wwwroot/";

            string newFileName = $"Uploads/{Guid.NewGuid()}{extension}";

            using (FileStream fs = new FileStream($"{directoryPath}{newFileName}", FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(fs);
            }

            // Remove the old file if existing 
            if (bill.Attachment != null)
            {
                System.IO.File.Delete($"{directoryPath}{bill.Attachment}");
            }

            bill.Attachment = newFileName;
            await _db.SaveChangesAsync();

            return Ok(new HttpSingleResponse<Bill>
            {
                IsSuccess = true,
                Value = bill,
                Message = "File uploaded"
            });

        }
        #endregion

        #region Edit
        // PUT api/bills
        [HttpPut]
        public async Task<IActionResult> Put(BillViewModel model)
        {
            if(ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(model.Id))
                    return NotFound();

                var bill = await _db.Bills.FindAsync(model.Id);
                if (bill == null)
                    return NotFound();

                bill.Number = model.Number;
                bill.Description = model.Description;
                bill.BillDate = model.BillDate.ToUniversalTime();
                bill.Provider = model.Provider;

                await _db.SaveChangesAsync();

                return Ok(new HttpSingleResponse<Bill>
                {
                    IsSuccess = true,
                    Message = "Bill edited",
                    Value = bill
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

            var bill = await _db.Bills.FindAsync(id);
            if (bill == null)
                return NotFound();

            _db.Bills.Remove(bill);
            await _db.SaveChangesAsync(); 

            // Check if there is a file 
            if(bill.Attachment != null)
            {
                // Remove the fiel 
                string directory = $"{Directory.GetCurrentDirectory()}/wwwroot/";
                System.IO.File.Delete($"{directory}{bill.Attachment}"); 
            }

            return Ok(new HttpSingleResponse<Bill>
            {
                IsSuccess = true,
                Value = bill,
                Message = "Bill deleted"
            });
        }
        #endregion 
    }
}