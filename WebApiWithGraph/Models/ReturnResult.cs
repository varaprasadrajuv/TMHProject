using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiWithGraph.Models
{
    public class ReturnResult
    {
        public string Status { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}
