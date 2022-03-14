using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiProject.Authentication;

public class JwtConfig
{
    public string Secret { get; set; }
    public TimeSpan ExpiryTimeFrame { get; set; }
}
