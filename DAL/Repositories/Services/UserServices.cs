using DAL.DTO.Req;
using DAL.DTO.Req.Pagination;
using DAL.DTO.Req.UserDto;
using DAL.DTO.Res;
using DAL.DTO.Res.UserDto;
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

        public async Task<List<ResUserDto>> GetAllUsers(ReqPaginationQuery paginationQuery)
        {
            int skip = (paginationQuery.PageNumber - 1) * paginationQuery.PageSize;

            var query = _context.MstUsers.AsQueryable();

            if (string.Equals(paginationQuery.OrderDirection, "asc", StringComparison.OrdinalIgnoreCase))
            {
                query = paginationQuery.OrderBy switch
                {
                    "Email" => query.OrderBy(user => user.Email),
                    "Role" => query.OrderBy(user => user.Role),
                    "Balance" => query.OrderBy(user => user.Balance),
                    "CreatedAt" => query.OrderBy(user => user.CreatedAt),
                    "UpdatedAt" => query.OrderBy(user => user.UpdatedAt),
                    _ => query.OrderBy(user => user.Name),
                };
            }
            else
            {
                query = paginationQuery.OrderBy switch
                {
                    "Email" => query.OrderByDescending(user => user.Email),
                    "Role" => query.OrderByDescending(user => user.Role),
                    "Balance" => query.OrderByDescending(user => user.Balance),
                    "CreatedAt" => query.OrderBy(user => user.CreatedAt),
                    "UpdatedAt" => query.OrderBy(user => user.UpdatedAt),
                    _ => query.OrderByDescending(user => user.Name),
                };
            }


            var result = await query
                .Where(user => !user.IsDeleted)
                .Select(user => new ResUserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    Balance = user.Balance,
                })
                .Skip(skip)
                .Take(paginationQuery.PageSize)
                .ToListAsync();
            return result;
        }

        public async Task<ResLoginDto> Login(ReqLoginDto login)
        {
            var user = await _context.MstUsers.SingleOrDefaultAsync(x => x.Email == login.Email);
            if (user == null || user.IsDeleted)
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
            if (isAnyEmail != null)
            {
                throw new ResErrorDto
                {
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
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };
            var token = new JwtSecurityToken(
                issuer: jwtSettings["ValidIssuer"],
                audience: jwtSettings["ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddHours(50),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<ResSingleUserDto> GetUserById(string id)
        {
            var user = await _context.MstUsers.SingleOrDefaultAsync(x => x.Id == id);
            if (user == null)
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
            var user = await _context.MstUsers.SingleOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                throw new ResErrorDto
                {
                    Message = "User Not Found!!!",
                    StatusCode = StatusCodes.Status404NotFound
                };
            }
            user.IsDeleted = true;
            _context.MstUsers.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<ResUserDto> UpdateUserById(string id, ReqEditUserDto dto)
        {
            var user = await _context.MstUsers.SingleOrDefaultAsync(x => x.Id == id);
            if (user == null)
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

        public Task<int> GetCount(ReqPaginationQuery paginationQuery)
        {
            int skip = (paginationQuery.PageNumber - 1) * paginationQuery.PageSize;

            var query = _context.MstUsers.AsQueryable();

            if (string.Equals(paginationQuery.OrderDirection, "asc", StringComparison.OrdinalIgnoreCase))
            {
                query = paginationQuery.OrderBy switch
                {
                    "Email" => query.OrderBy(user => user.Email),
                    "Role" => query.OrderBy(user => user.Role),
                    "Balance" => query.OrderBy(user => user.Balance),
                    "CreatedAt" => query.OrderBy(user => user.CreatedAt),
                    "UpdatedAt" => query.OrderBy(user => user.UpdatedAt),
                    _ => query.OrderBy(user => user.Name),
                };
            }
            else
            {
                query = paginationQuery.OrderBy switch
                {
                    "Email" => query.OrderByDescending(user => user.Email),
                    "Role" => query.OrderByDescending(user => user.Role),
                    "Balance" => query.OrderByDescending(user => user.Balance),
                    "CreatedAt" => query.OrderBy(user => user.CreatedAt),
                    "UpdatedAt" => query.OrderBy(user => user.UpdatedAt),
                    _ => query.OrderByDescending(user => user.Name),
                };
            }


            return query
                .Select(user => new ResUserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    Balance = user.Balance
                })
                .CountAsync();
        }
    }
}
