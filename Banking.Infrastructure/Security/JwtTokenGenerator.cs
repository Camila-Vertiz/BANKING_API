using Banking.Application.Responses;
using Banking.Application.Security.Interfaces;
using Banking.Domain.Entities;
using Banking.Infrastructure.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Banking.Application.Security
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtSettings _settings;


        public JwtTokenGenerator(
            IOptions<JwtSettings> options)
        {
            _settings = options.Value;
        }

        public JwtTokenResult GenerateToken(User user)
        {
            var expiresAt = DateTime.UtcNow
                .AddMinutes(_settings.ExpiresMinutes);


            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()),

                new Claim(
                    JwtRegisteredClaimNames.Name,
                    user.UserName),

                new Claim(
                    ClaimTypes.Role,
                    user.Role.ToString())
            };


            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_settings.Key));


            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);


            return new JwtTokenResult
            {
                Token = new JwtSecurityTokenHandler()
                    .WriteToken(token),

                ExpiresAt = expiresAt
            };
        }
    }
}
