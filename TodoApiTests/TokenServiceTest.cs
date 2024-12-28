using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Castle.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Moq;
using TodoAPI.Models;
using TodoAPI.Services;
using Xunit;

namespace TodoApiTests
{
    public class TokenServiceTests
    {
        [Fact]
        public void TestCreateValidToken()
        {
            var dict = new Dictionary<string, string?>{
               { "TokenKey", "yyCDzcZXLKeIi0GAzdZdjvFtzAug92gIjmqyQcCUUWgvb4lUGYuOPnRC9EDIRpOc"}
            };

            var config = new ConfigurationBuilder().AddInMemoryCollection(dict).Build();

            var tokenService = new TokenService(config);

            var userId = "userId";
            var user = new User{Id= userId};

            var token = tokenService.CreateToken(user);
            Assert.NotNull(token);

            var tokenHandler = new JwtSecurityTokenHandler();
            var canRead = tokenHandler.CanReadToken(token);
            Assert.True(canRead);

            var jwtToken = tokenHandler.ReadJwtToken(token);
            var nameIdentifierClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;

            Assert.NotNull(nameIdentifierClaim);
            Assert.Equal(userId, nameIdentifierClaim);
        }
    }
}