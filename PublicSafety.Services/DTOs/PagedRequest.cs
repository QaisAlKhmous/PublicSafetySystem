using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services.DTOs
{
    public class PagedRequest
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SortField { get; set; }
        public string SortDir { get; set; }
        public Dictionary<string, string> Filter { get; set; }
    }
}
