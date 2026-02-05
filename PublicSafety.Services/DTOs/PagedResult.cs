using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicSafety.Services.DTOs
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int Total { get; set; }
    }
}
