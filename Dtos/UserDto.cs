using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Username { get; set; }        
        public string Email { get; set; }
        public string Token { get; set; }
    }
}