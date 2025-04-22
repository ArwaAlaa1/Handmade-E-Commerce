using ECommerce.Core.Models;
using ECommerce.Core.Services.Contract;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _usermanager;

        public AuthService(IConfiguration configuration, UserManager<AppUser> usermanager)
        {
            _configuration = configuration;
            _usermanager = usermanager;
        }

        public async Task<string> CreateTokenAsync(AppUser user)
        {
            var roles = await _usermanager.GetRolesAsync(user);
            
            // private claims
            var authClaims = new List<Claim>
            {
                //new Claim(ClaimTypes.GivenName, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                
            };

            //get role
            foreach (var itemRole in roles)
            {
            authClaims.Add(new Claim(ClaimTypes.Role, itemRole/*.ToString()*/));
            }

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddDays(double.Parse(_configuration["JWT:TokenExpirationInDay"])),
                claims: authClaims,
                signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
