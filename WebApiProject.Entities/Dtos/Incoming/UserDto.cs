using System;
using System.Collections.Generic;
using System.Text;

namespace WebApiProject.Entities.Dtos.Incoming;

public class UserDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string DateOfBirth { get; set; }
    public string Country { get; set; }
}

