﻿using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Models;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services
{
    public class UserServices : IUserServices
    {
        private readonly P2plandingContext _context;
        private readonly IConfiguration _configuration;
        public UserServices(P2plandingContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<List<ResUserDto>> GetAllUsers()
        {
            return await _context.MstUsers.Select(
                user => new ResUserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    Balance = user.Balance
                }
            ).Where(ResBaseDto => ResBaseDto.Role != "admin").ToListAsync();
        }

        public async Task<ResLoginDto> Login(ReqLoginDto login)
        {
            var user = await _context.MstUsers.SingleOrDefaultAsync(x => x.Email == login.Email);
            if (user == null)
            {
                throw new ResErrorDto
                {
                    Message = "Invalid Email or Password",
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
            var checkPassword = BCrypt.Net.BCrypt.Verify(login.Password, user.Password);
            if (!checkPassword)
            {
                throw new ResErrorDto
                {
                    Message = "Invalid Email or Password",
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
            return new ResLoginDto
            {
                Token = GenerateJwtToken(user)
            };
        }

        public async Task<string> Register(ReqRegisterUserDto register)
        {
            var isAnyEmail = await _context.MstUsers.SingleOrDefaultAsync(x => x.Email == register.Email);
            if (isAnyEmail !=null)
            {
               throw new ResErrorDto {
                    Message = "Email already used",
                    StatusCode = StatusCodes.Status400BadRequest
               };
            }
            var user = new MstUser
            {
                Name = register.Name,
                Email = register.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(register.Password),
                Role = register.Role,
                Balance = register.Balance ?? 0
            };

            await _context.MstUsers.AddAsync(user);
            await _context.SaveChangesAsync();

            return user.Name;
        }

        public string GenerateJwtToken(MstUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var token = new JwtSecurityToken(
                issuer: jwtSettings["ValidIssuer"],
                audience: jwtSettings["ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<ResSingleUserDto> GetUserById(string id)
        {
            var user = await _context.MstUsers.SingleOrDefaultAsync(x => x.Id == id);
            if(user == null)
            {
                throw new ResErrorDto
                {
                    Data = null,
                    Message = "User not found",
                    StatusCode = StatusCodes.Status404NotFound
                };
            }
            var result = new ResSingleUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Role = user.Role,
                Balance = user.Balance
            };
            return result;
        }

        public async Task DeleteUserById(string id)
        {
            await _context.MstUsers.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public async Task<ResUserDto> UpdateUserById(string id, ReqEditUserDto dto)
        {
            var user = await _context.MstUsers.SingleOrDefaultAsync(x => x.Id == id);
            if(user == null)
            {
                throw new ResErrorDto
                {
                    Message = "User Not Found!!!",
                    StatusCode = StatusCodes.Status404NotFound
                };
            }
            user.Name = dto.Name;
            user.Role = dto.Role;
            user.Balance = dto.Balance ?? user.Balance;
            _context.MstUsers.Update(user);
            await _context.SaveChangesAsync();
            return new ResUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Balance = user.Balance
            };
        }
    }
}
