using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AkExpenses.Models.Shared
{

    public class RegisterViewModel
    {
        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        [Required]
        [StringLength(50)]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }
    }

    public class UserManageResponse
    {
        public string Message { get; set; }

        public string AccountId { get; set; }

        public DateTime ExpireDate { get; set; }

        public bool IsSuccess { get; set; }

        public IEnumerable<string> Errors { get; set; }
    }
}
