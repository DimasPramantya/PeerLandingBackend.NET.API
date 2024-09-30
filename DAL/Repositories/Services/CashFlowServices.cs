using DAL.DTO.Req.Cash_Flow;
using DAL.DTO.Req.Pagination;
using DAL.DTO.Res;
using DAL.DTO.Res.CashFlow;
using DAL.DTO.Res.LoadDto;
using DAL.DTO.Res.UserDto;
using DAL.Models;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services
{
    public class CashFlowServices : ICashFlowServices
    {
        private readonly P2plandingContext _context;
        private readonly IConfiguration _configuration;
        public CashFlowServices(P2plandingContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<int> CountUserCashFlows(string userId, ReqPaginationQuery paginationQuery)
        {
            IQueryable<CashFlow> query = _context.CashFlows
               .Where(l => l.UserId == userId)
               .OrderByDescending(l => l.CreatedAt);

            var results = await query.CountAsync();

            return results;
        }

        public async Task<ResCashFlowDto> CreateCashFlow(string userID, ReqCashFlowDto cashFlowDto)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var user = await _context.MstUsers.FirstOrDefaultAsync(u => u.Id == userID);
                    if (user == null)
                    {
                        throw new ResErrorDto
                        {
                            Data = null,
                            Message = "User with id-" + userID + " not found",
                            StatusCode = StatusCodes.Status404NotFound
                        };
                    }
                    var type = cashFlowDto.Amount > 0 ? "IN" : "OUT";
                    if (type == "OUT" && user.Balance < cashFlowDto.Amount)
                    {
                        throw new ResErrorDto
                        {
                            Data = null,
                            Message = "Insufficient balance",
                            StatusCode = StatusCodes.Status400BadRequest
                        };
                    }
                    var cashFlow = new CashFlow
                    {
                        UserId = userID,
                        Amount = cashFlowDto.Amount,
                        Description = cashFlowDto.Description,
                        Type = cashFlowDto.Amount > 0 ? "IN" : "OUT"
                    };

                    user.Balance += cashFlowDto.Amount;

                    _context.MstUsers.Update(user);

                    await _context.CashFlows.AddAsync(cashFlow);

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return new ResCashFlowDto
                    {
                        UserId = cashFlow.UserId,
                        Amount = cashFlow.Amount,
                        Description = cashFlow.Description,
                        CreatedAt = cashFlow.CreatedAt,
                        UpdatedAt = cashFlow.UpdatedAt,
                        Type = cashFlow.Type
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    Console.WriteLine($"An error occurred: {ex.Message}");
                    throw;
                }
            }
        }

        public async Task<List<ResCashFlowDto>> GetUserCashFlows(string userId, ReqPaginationQuery paginationQuery)
        {
            IQueryable<CashFlow> query = _context.CashFlows
               .Where(l => l.UserId == userId)
               .OrderByDescending(l => l.CreatedAt);


            query = query.Skip((paginationQuery.PageNumber - 1) * paginationQuery.PageSize)
                 .Take(paginationQuery.PageSize);

            var results = await query.Select(loan => new ResCashFlowDto
            {
                Id = loan.Id,
                UserId = loan.UserId,
                Amount = loan.Amount,
                Description = loan.Description,
                Type = loan.Type,
                CreatedAt = loan.CreatedAt,
                UpdatedAt = loan.UpdatedAt
            }).ToListAsync();

            return results;
        }
    }
}
