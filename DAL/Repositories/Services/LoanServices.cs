using DAL.DTO.Req;
using DAL.DTO.Res;
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
    public class LoanServices : ILoanServices
    {
        private readonly P2plandingContext _context;
        private readonly IConfiguration _configuration;
        public LoanServices(P2plandingContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<string> CreateLoan(ReqLoanDto loanDto)
        {
            var newLoan = new MstLoans
            {
                BorrowerId = loanDto.BorrowerId,
                Amount = loanDto.Amount,
                InterestRate = loanDto.InterestRate,
                DurationMonth = loanDto.Duration,
            };

            await _context.MstLoans.AddAsync(newLoan);
            await _context.SaveChangesAsync();

            return newLoan.BorrowerId;
        }

        public async Task<List<ResLoanDto>> GetAllLoans(string? status)
        {
            if(status != null)
            {
                var loans = await _context.MstLoans.Include(l => l.User).Where(l => l.Status == status || l.Status == null).OrderByDescending(l => l.CreatedAt).Select(
                loan => new ResLoanDto
                {
                    BorrowerId = loan.BorrowerId,
                    Amount = loan.Amount,
                    InterestRate = loan.InterestRate,
                    Duration = loan.DurationMonth,
                    CreatedAt = loan.CreatedAt,
                    UpdatedAt = loan.UpdatedAt,
                    Status = loan.Status,
                    Borrower = new ResUserDto
                    {
                        Id = loan.User.Id,
                        Name = loan.User.Name,
                        Email = loan.User.Email,
                        Role = loan.User.Role,
                        Balance = loan.User.Balance
                    }

                }
                ).ToListAsync();
                return loans;
            }else
            {
                var loans = await _context.MstLoans.Include(l => l.User).OrderByDescending(l => l.CreatedAt).Select(
                loan => new ResLoanDto
                {
                    BorrowerId = loan.BorrowerId,
                    Amount = loan.Amount,
                    InterestRate = loan.InterestRate,
                    Duration = loan.DurationMonth,
                    CreatedAt = loan.CreatedAt,
                    UpdatedAt = loan.UpdatedAt,
                    Status = loan.Status,
                    Borrower = new ResUserDto
                    {
                        Id = loan.User.Id,
                        Name = loan.User.Name,
                        Email = loan.User.Email,
                        Role = loan.User.Role,
                        Balance = loan.User.Balance
                    }

                }
                ).ToListAsync();
                return loans;

            }
            
            
        }

        public async Task<ResStatusLoanDto> UpdateStatusLoan(string loanId, ReqLoanStatusDto dto)
        {
            var loan = await _context.MstLoans.FindAsync(loanId);
            if(loan == null)
            {
                throw new ResErrorDto
                {
                    Message = "Loan not found",
                    StatusCode = StatusCodes.Status404NotFound
                };
            }
            loan.Status = dto.Status;
            loan.UpdatedAt = DateTime.UtcNow;
            _context.MstLoans.Update(loan);
            await _context.SaveChangesAsync();
            return new ResStatusLoanDto
            {
                Status = loan.Status
            };
        }
    }
}
