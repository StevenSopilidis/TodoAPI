using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoAPI.Dtos;
using TodoAPI.Models;

namespace TodoAPI.Repositories
{
    public class UserRepo : IUserRepo
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public UserRepo(UserManager<User> userManager, IMapper mapper) {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IEnumerable<IdentityError>> CreateUserAsync(CreateUserDto dto)
        {
            var newUser = _mapper.Map<User>(dto);
        
            var result = await _userManager.CreateAsync(newUser, dto.Password);
            
            return result.Errors; 
        }

        public async Task<bool> UsernameOrEmailUsedAsync(string username, string email)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.UserName == username || u.Email == email);
            
            if (user is not null)
                return true;
            
            return false;
        }
    }
}