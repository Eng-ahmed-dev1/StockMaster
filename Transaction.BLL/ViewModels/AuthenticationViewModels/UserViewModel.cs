using System;
using System.Collections.Generic;
using System.Text;
using Transaction.DAL;
namespace Transaction.BLL
{
    // This ViewModle to show the user data after the login in profile or any someting like that 
    public class UserViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool IsActive { get; set; }
        public Gender? Gender { get; set; }
        public ICollection<string> Roles { get; set; } = new HashSet<string>();
    }
}
