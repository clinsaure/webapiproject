using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiProject.Entities.Dtos.Outgoing.Profile
{
    public class ProfileDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string DateOfBirth { get; set; }
        public string Country { get; set; }
    }
}
