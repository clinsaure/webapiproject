using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WebApiProject.Entities.DbSet;

public class RefreshToken : BaseEntity
{
    public string UserId { get; set; } // User Id when logged in 
    public string Token { get; set; }
    public string JwtId { get; set; } // the id generated when a jwt id has been requested
    public bool IsUsed { get; set; } // To make sure that the token is only used once
    public bool IsRevoked { get; set; } // make sure they are valid
    public DateTime ExpiryDate { get; set; }

    [ForeignKey(nameof(UserId))]
    public IdentityUser User { get; set; }
}

