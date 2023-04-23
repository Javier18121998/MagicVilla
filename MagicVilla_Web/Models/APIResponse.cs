using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace MagicVilla_Web.Models
{
    public class APIResponse
    {
        public HttpStatusCode statusCode { get; set; }
        public bool IsExitoso { get; set; } = true;
        public List<string> ErrorMessages { get; set; }
        public object Resultado { get; set; }
    }
}