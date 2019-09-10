using System;
using System.Collections.Generic;
using System.Text;

namespace AkExpenses.Models.Shared
{

    public abstract class HttpApiResponse<T> where T : class
    {
        public HttpApiResponse()
        {
            OperationDate = DateTime.UtcNow;
        }
        public bool IsSuccess { get; set; }
        public string SearchQuery { get; set; }
        public string Message { get; set; }
        public DateTime OperationDate { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }

    public class HttpSingleResponse<T> : HttpApiResponse<T> where T : class
    {
        public T Value { get; set; }

    }

    public class HttpCollectionResponse<T> : HttpApiResponse<T> where T : class
    {
        public IEnumerable<T> Values { get; set; }

        public int Count { get; set; }

        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }

    }
}
