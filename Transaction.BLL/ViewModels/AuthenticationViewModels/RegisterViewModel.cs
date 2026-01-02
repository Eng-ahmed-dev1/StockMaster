using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Transaction.DAL;

namespace Transaction.BLL
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(200, ErrorMessage = "Full Name cannot exceed 200 characters")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(256)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(256, MinimumLength = 5, ErrorMessage = "Username must be between 5 and 256 characters")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match")]
        public string ConfirmPassword { get; set; }
        [StringLength(11)]
        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string City { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public CountryType Country { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }
    }
}