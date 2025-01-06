using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoAPI.Dtos;

namespace TodoApiTests.Utils
{
    public static class UserGenerator
    {
        private static readonly Random _random = new Random();
        private const string _allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static CreateUserDto GenerateUser()
        {
            return new CreateUserDto
            {
                Username = GenerateUsername(),
                Email = GenerateEmail(),
                Password = GeneratePassword()
            };
        }

        private static string GenerateUsername(int minLength = 3, int maxLength = 20)
        {
            int length = _random.Next(minLength, maxLength + 1);
            StringBuilder username = new StringBuilder(length);

            username.Append(_allowedChars[_random.Next(0, 52)]);  // Start with a letter
            for (int i = 1; i < length; i++)
            {
                username.Append(_allowedChars[_random.Next(_allowedChars.Length)]);
            }
            return username.ToString();
        }

        private static string GenerateEmail()
        {
            string username = GenerateUsername(5, 10);
            string domain = GenerateUsername(3, 7);
            string tld = GenerateTld();
            return $"{username}@{domain}.{tld}";
        }

        private static string GeneratePassword()
        {
            // Simple password generator for demonstration purposes
            const string specialChars = "!@#$%^&*";
            const string numbers = "0123456789";

            var builder = new StringBuilder();
            builder.Append(_allowedChars[_random.Next(0, 52)]);  // One letter
            builder.Append(numbers[_random.Next(numbers.Length)]);  // One number
            builder.Append(specialChars[_random.Next(specialChars.Length)]);  // One special char

            for (int i = 3; i < _random.Next(8, 16); i++)
            {
                string allChars = _allowedChars + specialChars + numbers;
                builder.Append(allChars[_random.Next(allChars.Length)]);
            }
            return builder.ToString();
        }

        private static string GenerateTld()
        {
            string[] tlds = { "com", "net", "org", "edu", "gov" };
            return tlds[_random.Next(tlds.Length)];
        }

    }
}