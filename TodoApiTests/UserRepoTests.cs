using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using TodoAPI.Dtos;
using TodoAPI.Models;
using TodoAPI.Repositories;
using Xunit;

namespace TodoApiTests
{
    public class UserRepoTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<UserRepo>> _loggerMock;
        private readonly UserRepo _userRepo;

        public UserRepoTests() {
            _userManagerMock = MockUserManager();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<UserRepo>>();
            _userRepo = new UserRepo(_userManagerMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        private  Mock<UserManager<User>> MockUserManager() {
            var store = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
        }

        [Fact]
        public async Task CreateUserAsync_ReturnsError_WhenUserCreationFails() {
            var email = "test@test.com";
            var pass = "pass1234@";
            var errorMsg = "User creation failed";

            var dto = new CreateUserDto{Email=email, Password= pass};
            var user = new User();

            _mapperMock.Setup(m => m.Map<User>(dto)).Returns(user);
            _userManagerMock.Setup(um => um.CreateAsync(user, dto.Password))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = errorMsg }));

            var errors = await _userRepo.CreateUserAsync(dto);

            Assert.True(errors.Any());
            Assert.Single(errors);
            Assert.Equal(errorMsg, errors.First().Description);
        }
    
        [Fact]
        public async Task CreateUserAsync_ReturnsNoError_WhenUserCreationSucceeds() {
            var email = "test@test.com";
            var pass = "pass1234@";

            var dto = new CreateUserDto{Email=email, Password= pass};
            var user = new User();

            _mapperMock.Setup(m => m.Map<User>(dto)).Returns(user);
            _userManagerMock.Setup(um => um.CreateAsync(user, dto.Password))
                .ReturnsAsync(IdentityResult.Success);

            var errors = await _userRepo.CreateUserAsync(dto);
            Assert.False(errors.Any());
        }

        [Fact]
        public async Task GetUserAsync_ReturnsUser_WhenExist() {
            var email = "test@test.com";
            var returnObject = new User{Email=email};

            _userManagerMock.Setup(um => um.FindByEmailAsync(email))
                .ReturnsAsync(returnObject);

            var user = await _userRepo.GetUserAsync(email);
            Assert.NotNull(user);
            Assert.Equal(email, user.Email);
        }

        [Fact]
        public async Task GetUserAsync_ReturnsNull_WhenUserDoesNotExist() {
            var email = "test@test.com";
            User? returnObject = null;

            _userManagerMock.Setup(um => um.FindByEmailAsync(email))
                .ReturnsAsync(returnObject);

            var user = await _userRepo.GetUserAsync(email);
            Assert.Null(user);
        }

        [Fact]
        public async Task UsernameOrEmailUsedAsync_ReturnsFalls_WhenAlreadyTaken() {
            var email = "test@test.com";
            var username = "taken";

            // Mock user list with a matching user
            var userList = new List<User>
            {
                new User { UserName = "taken", Email = "test@test.com" }
            };

            var mockUsers = userList.AsQueryable().BuildMock();

            _userManagerMock.Setup(um => um.Users).Returns(mockUsers);

            var taken = await _userRepo.UsernameOrEmailUsedAsync(username, email);
            Assert.True(taken);
        }

        [Fact]
        public async Task UsernameOrEmailUsedAsync_ReturnsSucceeds_WhenAlreadyTaken() {
            var email = "test@test.com";
            var username = "taken";

            // Mock user list with a matching user
            var userList = new List<User>
            {
                new User { UserName = "not_taken", Email = "test2@test.com" }
            };

            var mockUsers = userList.AsQueryable().BuildMock();

            _userManagerMock.Setup(um => um.Users).Returns(mockUsers);

            var taken = await _userRepo.UsernameOrEmailUsedAsync(username, email);
            Assert.False(taken);
        }
    }
}