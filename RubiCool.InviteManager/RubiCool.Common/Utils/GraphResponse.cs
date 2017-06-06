using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RubiCool.Common.Utils
{
    public class GraphResponse<T>
    {
        public T Data { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccessfull { get; set; }
        public string Message { get; set; }

        public GraphResponse()
        {
            IsSuccessfull = true;
        }
    }
}
