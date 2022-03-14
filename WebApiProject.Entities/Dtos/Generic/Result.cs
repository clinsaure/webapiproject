using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiProject.Entities.Dtos.Errors;

namespace WebApiProject.Entities.Dtos.Generic
{
    public class Result<T>
    {
        public T Content { get; set; }
        public Error Error { get; set; }
        public bool IsSuccess => Error == null;
        public DateTime ResponseTime { get; set; } = DateTime.UtcNow;

    }
}
