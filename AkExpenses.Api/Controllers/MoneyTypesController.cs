using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AkExpenses.Api.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AkExpenses.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoneyTypesController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public MoneyTypesController(ApplicationDbContext context)
        {
            this.context = context;
        }

        #region Get

        

        #endregion

        #region Post



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
            var type = await context.MoneyTypes.FindAsync(id);

            if (type == null)
            {
                return NotFound();
            }

            context.MoneyTypes.Remove(type);
            await context.SaveChangesAsync();

            return Ok(new
            {

            });
        }
        
        #endregion
    }
}