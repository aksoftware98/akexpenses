using System;
using System.Collections.Generic;
using System.Text;

namespace AkExpenses.Models.Shared
{
    public class HttpResponse<T> where T : class
    {
        public HttpResponse()
        {
            OperationDate = DateTime.UtcNow; 
        }
        public bool IsSuccess { get; set; }

        public int Count { get; set; }

        public string Message { get; set; }

        public T Value { get; set; }

        public DateTime OperationDate { get; set; }

        public IEnumerable<string> Errors { get; set; }

    }
}
