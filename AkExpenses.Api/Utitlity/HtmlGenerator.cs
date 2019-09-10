using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkExpenses.Api.Utitlity
{
    internal static class HtmlGenerator
    {

        /// <summary>
        /// Gets a HTML message for a specific user to be sent by mail to him/her
        /// </summary>
        /// <param name="username">User name to be appeared within the mail header</param>
        /// <param name="accountId">Account Id associated to registered user</param>
        /// <returns></returns>
        public static string GetRegisterHTML(string username, string accountId)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("<HTML>" +
                                   "<HEAD></HEAD>" +
                                   "<BODY>");

            stringBuilder.Append($"<h1 style='text-algin:center'>Welcome {username} to AK Expenses</h1>");
            stringBuilder.Append($"<p>Thank you for your interest in AK Expenses, " +
                $"right now you can manage all your expenses very easily in an efficient way, make sure to organize yourself ;-)</p>" +
                $"<p>Your account ID is {accountId}</p>");
            stringBuilder.Append("</BODY></HTML>");

            return stringBuilder.ToString(); 
        }

    }
}
