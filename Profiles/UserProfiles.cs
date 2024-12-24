using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TodoAPI.Dtos;
using TodoAPI.Models;

namespace TodoAPI.Profiles
{
    // profiles for user entities to be used by mapper
    public class UserProfiles : Profile
    {
        public UserProfiles() {
            CreateMap<CreateUserDto, User>();
            CreateMap<User, UserCreatedDto>();
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
        }
    }
}