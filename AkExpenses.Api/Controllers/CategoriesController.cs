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
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        const string iconPath = "Uploads/category.png";

        private readonly ApplicationDbContext _db;

        public CategoriesController(ApplicationDbContext db)
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

        // Get all the categories of a specific user 
        // api/categories?query=&page=1
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            //Get logged in account
            var account = await getAccount();

            //Get categories
            var categories = _db.Categories.Where(t => t.AccountId == account.Id).OrderByDescending(t => t.Name);

            //Create response
            var response = new HttpCollectionResponse<object>
            {
                Count = categories.Count(),
                IsSuccess = true,
                Message = "Categories have been retrieved successfully.",
                Values = categories.Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Description
                })
            };

            //Return result
            return Ok(response);
        }

        // Get specific category 
        //  GET: api/categories/435g
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var category = await _db.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            return Ok(new HttpSingleResponse<Category>
            {
                IsSuccess = true,
                Message = "Category retrieved",
                Value = category,
            });
        }

        #endregion

        #region Create

        // POST: api/categories
        [HttpPost]
        public async Task<IActionResult> Post(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var account = await getAccount();

                // Check if there is a file 
                var oldCategory = _db.Categories.SingleOrDefault(c => c.Name == model.Name.Trim() && c.AccountId == account.Id);

                if (oldCategory != null)
                {
                    return this.FixedBadRequest($"Category with name: {oldCategory.Name} already exists.");
                }

                var category = await _db.AddAsync(new Category
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = model.Name,
                    AccountId = account.Id,
                    Description = model.Description,
                    IconPath = iconPath
                });
                await _db.SaveChangesAsync();

                return Ok(new HttpSingleResponse<Category>
                {
                    IsSuccess = true,
                    Message = "Bill added",
                    Value = category.Entity
                });

            }
            return this.FixedBadRequest("Some fields are not valid.");
        }

        // POST: api/categories
        [HttpPost("{id}"), DisableRequestSizeLimitAttribute]
        public async Task<IActionResult> Post(string id, IFormFile file)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            // get the category 
            var category = await _db.Categories.FindAsync(id);
            if (category == null)
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
                return this.FixedBadRequest("The uploaded file is not supported.");

            string directoryPath = Directory.GetCurrentDirectory() + "/wwwroot/";

            string newFileName = $"Uploads/{Guid.NewGuid()}{extension}";

            using (FileStream fs = new FileStream($"{directoryPath}{newFileName}", FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(fs);
            }

            // Remove the old file if existing 
            if (category.IconPath != iconPath)
            {
                System.IO.File.Delete($"{directoryPath}{category.IconPath}");
            }

            category.IconPath = newFileName;
            await _db.SaveChangesAsync();

            return Ok(new HttpSingleResponse<Category>
            {
                IsSuccess = true,
                Value = category,
                Message = "File uploaded"
            });

        }

        #endregion

        #region Edit

        // PUT api/categories
        [HttpPut]
        public async Task<IActionResult> Put(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(model.CategoryId))
                    return NotFound();

                var account = await getAccount();

                var category = await _db.Categories.FindAsync(model.CategoryId);
                if (category == null)
                    return NotFound();

                var oldCategory = _db.Categories
                    .SingleOrDefault(c => c.Name == model.Name.Trim() && c.Id != model.CategoryId && c.AccountId == account.Id);

                if (oldCategory != null)
                {
                    return this.FixedBadRequest($"There is already a category with the name: {oldCategory.Name}.");
                }

                category.Name = model.Name;
                category.Description = model.Description;

                await _db.SaveChangesAsync();

                return Ok(new HttpSingleResponse<Category>
                {
                    IsSuccess = true,
                    Message = "Category edited",
                    Value = category
                });
            }

            return this.FixedBadRequest("Some propreties are not valid");
        }

        #endregion

        #region Delete

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var category = await _db.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();

            // Check if there is a file 
            if (category.IconPath != iconPath)
            {
                // Remove the fiel 
                string directory = $"{Directory.GetCurrentDirectory()}/wwwroot/";
                System.IO.File.Delete($"{directory}{category.IconPath}");
            }

            return Ok(new HttpSingleResponse<Category>
            {
                IsSuccess = true,
                Value = category,
                Message = "Category deleted"
            });
        }

        #endregion 
    }
}