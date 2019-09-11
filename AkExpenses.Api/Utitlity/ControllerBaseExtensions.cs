using AkExpenses.Models.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AkExpenses.Api.Utitlity
{
    public static class ControllerBaseExtensions
    {
        /// <summary>
        /// Returns a Bad request with data specific for this current system
        /// </summary>
        /// <param name="message">Message to send with the request</param>
        /// <returns></returns>
        public static IActionResult FixedBadRequest(this ControllerBase _base,string message = null)
        {
            return _base.BadRequest(new HttpSingleResponse<object>
            {
                IsSuccess = false,
                Message = message
            });
        }
    }
}
