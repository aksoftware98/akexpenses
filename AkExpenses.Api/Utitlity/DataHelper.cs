using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AkExpenses.Api.Utitlity
{
    public class DataHelper
    {
        /// <summary>
        /// Calcualte the size of the page 
        /// </summary>
        /// <param name="totalItems">Number of all items in the list</param>
        /// <param name="pageSize">Size of each page</param>
        /// <returns></returns>
        public static int GetTotalPages(int totalItems, int pageSize)
        {
            var remaining = totalItems % pageSize;
            if (remaining == 0)
                return totalItems / pageSize;

            return (totalItems / (pageSize - remaining)) + 1;
        }

        /// <summary>
        /// Returns list of all the allowed extensions to upload a picture 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> AllowedPictureExtensions()
        {
            return new List<string>
            {
                ".jpg",
                ".png",
                ".bmp"
            };
        }

        /// <summary>
        /// Return list of all the allowed extension to upload a specific fiel 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> AllowedDocumentsExtensions()
        {
            return new List<string>
            {
                ".jpg",
                ".png",
                ".bmp",
                ".xlsx",
                ".xls",
                ".doc",
                ".docx",
                ".txt",
                ".pdf"
            };
        }


    }
}
