using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI.Dtos
{
    public class UserCreatedDto
    {
        public string Username { get; set; }        
        public string Email { get; set; }
    }
}