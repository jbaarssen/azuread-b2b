using System;
using Newtonsoft.Json;

namespace RubiCool.Common.Model
{
    public class GraphError
    {
        public ResponseError Error { get; set; }
    }

    public class ResponseError
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public InnerError InnerError { get; set; }
    }

    public class InnerError
    {
        [JsonProperty("request-id")]
        public string RequestId { get; set; }
        public DateTime Date { get; set; }
    }
}
