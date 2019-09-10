using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AkExpenses.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace AkExpenses.Api.Controllers
{
    public class BillsController : Controller
    {

        const int PAGE_SIZE = 50;

        private readonly ApplicationDbContext _db;

        public BillsController(ApplicationDbContext db)
        {
            _db = db;
        }


        public IActionResult Index(string search, int? page)
        {
            if (string.IsNullOrEmpty(search))
                search = "";
            if (!page.HasValue)
                page = 0; 

            var result = _db.Bills.Where(b => b.Number.Contains(search)).OrderByDescending(b => b.BillDate).ThenByDescending(b => b.CreatedDate).Skip()
            return View();
        }
    }
}