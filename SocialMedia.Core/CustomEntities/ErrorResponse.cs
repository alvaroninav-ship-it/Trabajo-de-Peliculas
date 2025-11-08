using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.CustomEntities
{
    public class ErrorResponse
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public DateTime Timestamp { get; set; }
        public string Path { get; set; }

        public ErrorResponse()
        {
            Timestamp = DateTime.UtcNow;
        }
    }

}
