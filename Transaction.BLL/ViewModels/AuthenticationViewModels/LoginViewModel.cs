using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Transaction.BLL
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email or Username is required")]
        [StringLength(256)]
        public string EmailOrUserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; } = false;
    }
}
