using DAL.DTO.Req;
using DAL.DTO.Req.Pagination;
using DAL.DTO.Req.UserDto;
using DAL.DTO.Res;
using DAL.DTO.Res.UserDto;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services.Interfaces
{
    public interface IUserServices
    {
        Task<string> Register(ReqRegisterUserDto register);
        Task<ResLoginDto> Login(ReqLoginDto login);
        Task<List<ResUserDto>> GetAllUsers(ReqPaginationQuery paginationQuery);
        Task<int> GetCount(ReqPaginationQuery paginationQuery);
        Task<ResSingleUserDto> GetUserById(string id);
        Task DeleteUserById(string id);
        Task<ResUserDto> UpdateUserById(string id, ReqEditUserDto user);
    }
}
