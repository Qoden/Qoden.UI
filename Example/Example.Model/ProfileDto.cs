using System;
using SQLite;

namespace Example.Model
{
    public class ProfileDto
    {
        public Guid? ServerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
