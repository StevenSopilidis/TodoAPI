using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TodoAPI.Dtos;
using TodoAPI.Models;

namespace TodoAPI.Repositories
{
    public interface IUserRepo
    {
        Task<bool> UsernameOrEmailUsedAsync(string username, string email);
        Task<IEnumerable<IdentityError>?> CreateUserAsync(CreateUserDto dto);
        Task<User?> GetUserAsync(string email);
    }
}