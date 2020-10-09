using Store_MVC.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Store_MVC.Models.ViewModels.Account
{
    public class UserProfileVM
    {
        public UserProfileVM()
        {

        }

        public UserProfileVM(UserDTO row)
        {
            Id = row.Id;
            FirstName = row.FirstName;
            LastName = row.LastName;
            EmailAdress = row.EmailAdress;
            Username = row.Username;
            Password = row.Password;
        }
        public int Id { get; set; }
        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string EmailAdress { get; set; }
        [Required]
        [Display(Name = "User name")]
        public string Username { get; set; }
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 6)]
        public string Password { get; set; }
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }
    }
}