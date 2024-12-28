using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoAPI.Models;

namespace TodoAPI.Services
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}