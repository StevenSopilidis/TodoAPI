using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI.Dtos
{
    public class CreateUserDto
    {
        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 20 characters long")]
        public string Username { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email provided")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}