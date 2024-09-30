using DAL.DTO.Req.LoanDto;
using DAL.DTO.Req.Pagination;
using DAL.DTO.Res;
using DAL.DTO.Res.CashFlow;
using DAL.DTO.Res.LoadDto;
using DAL.DTO.Res.LoanDto;
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
    public class LoanServices : ILoanServices
    {
        private readonly P2plandingContext _context;
        private readonly IConfiguration _configuration;
        public LoanServices(P2plandingContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<int> CountAllLoans(ReqPaginationQuery paginationQuery, string status)
        {
            IQueryable<MstLoans> query = _context.MstLoans
                .OrderByDescending(l => l.CreatedAt);

            if (status != "all")
            {
                query = query.Where(l => l.Status == status || l.Status == null);
            }

            return await query.CountAsync();
        }

        public async Task<int> CountUserFunds(string lenderId, string status, ReqPaginationQuery paginationQuery)
        {
            IQueryable<MstLoans> query = _context.MstLoans
              .Include(l => l.Borrower)
              .Include(l => l.Lender)
              .Where(l => l.LenderId == lenderId)
              .OrderByDescending(l => l.CreatedAt);

            if (status != "all")
            {
                query = query.Where(l => l.Status == status);
            }

            return await query.CountAsync();
        }

        public async Task<int> CountUserLoans(string borrowerId, ReqPaginationQuery paginationQuery, string status)
        {
            IQueryable<MstLoans> query = _context.MstLoans
                .Where(l => l.BorrowerId == borrowerId)
                .OrderByDescending(l => l.CreatedAt);

            if (status != "all")
            {
                query = query.Where(l => l.Status == status || l.Status == null);
            }

            return await query.CountAsync();
        }

        public async Task<string> CreateLoan(string borrowerId, ReqLoanDto loanDto)
        {
            var newLoan = new MstLoans
            {
                BorrowerId = borrowerId,
                Amount = loanDto.Amount,
                InterestRate = loanDto.InterestRate,
                DurationMonth = loanDto.Duration,
                TotalAmount = loanDto.Amount + (loanDto.Amount * loanDto.InterestRate / 100),
            };

            await _context.MstLoans.AddAsync(newLoan);
            await _context.SaveChangesAsync();

            return newLoan.BorrowerId;
        }

        public async Task<List<ResLoanDto>> GetAllLoans(ReqPaginationQuery paginationQuery, string status)
        {
            IQueryable<MstLoans> query = _context.MstLoans
                .Include(l => l.Borrower)
                .Include(l => l.Lender)
                .OrderByDescending(l => l.CreatedAt);

            if (status != "all")
            {
                query = query.Where(l => l.Status == status || l.Status == null);
            }

            query = query.Skip((paginationQuery.PageNumber - 1) * paginationQuery.PageSize)
                 .Take(paginationQuery.PageSize);

            var loans = await query.Select(loan => new ResLoanDto
            {
                Id = loan.Id,
                BorrowerId = loan.BorrowerId,
                LenderId = loan.LenderId,
                Amount = loan.Amount,
                InterestRate = loan.InterestRate,
                Duration = loan.DurationMonth,
                CreatedAt = loan.CreatedAt,
                UpdatedAt = loan.UpdatedAt,
                Status = loan.Status,
                TotalRepaid = loan.TotalRepaid,
                Borrower = new ResSingleUserDto
                {
                    Id = loan.Borrower.Id,
                    Name = loan.Borrower.Name,
                    Role = loan.Borrower.Role
                },
                Lender = loan.Lender != null ? new ResSingleUserDto
                {
                    Id = loan.Lender.Id,
                    Name = loan.Lender.Name,
                    Role = loan.Lender.Role
                } : null
            }).ToListAsync();

            return loans;
        }

        public async Task<ResLoanDto> GetLoanById(string loanId)
        {
            var result = await _context.MstLoans
                .Include(l => l.Borrower)
                .Include(l => l.Lender)
                .FirstOrDefaultAsync(l => l.Id == loanId);
            if(result == null)
            {
                throw new ResErrorDto
                {
                    Message = "Loan not found",
                    StatusCode = StatusCodes.Status404NotFound
                };
            }
            return new ResLoanDto
            {
                Id = result.Id,
                BorrowerId = result.BorrowerId,
                LenderId = result.LenderId,
                Amount = result.Amount,
                InterestRate = result.InterestRate,
                Duration = result.DurationMonth,
                CreatedAt = result.CreatedAt,
                UpdatedAt = result.UpdatedAt,
                Status = result.Status,
                TotalRepaid = result.TotalRepaid,
                Borrower = new ResSingleUserDto
                {
                    Id = result.Borrower.Id,
                    Name = result.Borrower.Name,
                    Role = result.Borrower.Role
                },
                Lender = result.Lender != null ? new ResSingleUserDto
                {
                    Id = result.Lender.Id,
                    Name = result.Lender.Name,
                    Role = result.Lender.Role
                } : null
            };
        }

        public async Task<List<ResLoanDto>> GetUserFunds(string lenderId, string status, ReqPaginationQuery paginationQuery)
        {
            IQueryable<MstLoans> query = _context.MstLoans
               .Include(l => l.Borrower)
               .Include(l => l.Lender)
               .Where(l => l.LenderId == lenderId)
               .OrderByDescending(l => l.CreatedAt);

            if (status != "all")
            {
                query = query.Where(l => l.Status == status);
            }

            query = query.Skip((paginationQuery.PageNumber - 1) * paginationQuery.PageSize)
                 .Take(paginationQuery.PageSize);

            var loans = await query.Select(loan => new ResLoanDto
            {
                Id = loan.Id,
                BorrowerId = loan.BorrowerId,
                LenderId = loan.LenderId,
                Amount = loan.Amount,
                InterestRate = loan.InterestRate,
                Duration = loan.DurationMonth,
                CreatedAt = loan.CreatedAt,
                UpdatedAt = loan.UpdatedAt,
                Status = loan.Status,
                TotalRepaid = loan.TotalRepaid,
                Borrower = new ResSingleUserDto
                {
                    Id = loan.Borrower.Id,
                    Name = loan.Borrower.Name,
                    Role = loan.Borrower.Role
                },
                Lender = loan.Lender != null ? new ResSingleUserDto
                {
                    Id = loan.Lender.Id,
                    Name = loan.Lender.Name,
                    Role = loan.Lender.Role
                } : null
            }).ToListAsync();

            return loans;
        }

        public async Task<List<ResLoanDto>> GetUserLoans(string borrowerId, ReqPaginationQuery paginationQuery, string status)
        {
            IQueryable<MstLoans> query = _context.MstLoans
                .Include(l => l.Borrower)
                .Include(l => l.Lender) 
                .Where(l => l.BorrowerId == borrowerId)
                .OrderByDescending(l => l.CreatedAt);

            if (status != "all")
            {
                query = query.Where(l => l.Status == status || l.Status == null);
            }

            query = query.Skip((paginationQuery.PageNumber - 1) * paginationQuery.PageSize)
                 .Take(paginationQuery.PageSize);

            var loans = await query.Select(loan => new ResLoanDto
            {
                Id = loan.Id,
                BorrowerId = loan.BorrowerId,
                LenderId = loan.LenderId,
                Amount = loan.Amount,
                InterestRate = loan.InterestRate,
                Duration = loan.DurationMonth,
                CreatedAt = loan.CreatedAt,
                UpdatedAt = loan.UpdatedAt,
                Status = loan.Status,
                TotalRepaid = loan.TotalRepaid,
                Borrower = new ResSingleUserDto
                {
                    Id = loan.Borrower.Id,
                    Name = loan.Borrower.Name,
                    Role = loan.Borrower.Role
                },
                Lender = loan.Lender != null ? new ResSingleUserDto
                {
                    Id = loan.Lender.Id,
                    Name = loan.Lender.Name,
                    Role = loan.Lender.Role
                } : null
            }).ToListAsync();

            return loans;
        }

        public async Task<ResStatusLoanDto> UpdateStatusLoan(string loanId, string userId, string status)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var loan = await _context.MstLoans.FindAsync(loanId);
                    var lender = await _context.MstUsers.FindAsync(userId);
                    if(lender == null)
                    {
                        throw new ResErrorDto
                        {
                            Message = "Lender not found",
                            StatusCode = StatusCodes.Status404NotFound
                        };
                    }
                    if (loan == null)
                    {
                        throw new ResErrorDto
                        {
                            Message = "Loan not found",
                            StatusCode = StatusCodes.Status404NotFound
                        };
                    }
                    var borrower = await _context.MstUsers.FindAsync(loan.BorrowerId);
                    if (borrower == null)
                    {
                        throw new ResErrorDto
                        {
                            Message = "Borrower not found",
                            StatusCode = StatusCodes.Status404NotFound
                        };
                    }
                    if (loan.Status != "requested")
                    {
                        throw new ResErrorDto
                        {
                            Message = "Loan status is not requested",
                            StatusCode = StatusCodes.Status400BadRequest
                        };
                    }
                    if (status == "funded" && lender.Balance < loan.Amount)
                    {
                        throw new ResErrorDto
                        {
                            Message = "Insufficient balance",
                            StatusCode = StatusCodes.Status400BadRequest
                        };
                    }
                    borrower.Balance += loan.Amount;
                    lender.Balance -= loan.Amount;

                    loan.Status = status;
                    loan.UpdatedAt = DateTime.UtcNow;
                    loan.FundedAt = DateTime.UtcNow;
                    loan.LenderId = userId;

                    var borrowerCashFlow = new CashFlow
                    {
                        Amount = loan.Amount,
                        Description = $"Loan Funded From {lender.Name}",
                        Type = "IN",
                        UserId = borrower.Id,
                    };

                    var lenderCashFlow = new CashFlow
                    {
                        Amount = loan.Amount * -1,
                        Description = $"Loan Funded To {borrower.Name}",
                        Type = "OUT",
                        UserId = lender.Id,
                    };

                    var repaymentAmount = Math.Ceiling(loan.TotalAmount / 12);

                    for (int i = 1; i <= 12; i++)
                    {
                        var repayment = new TrnRepayment
                        {
                            LoanId = loan.Id,
                            Amount = repaymentAmount,
                            DeadlineDate = DateTime.UtcNow.AddMonths(i),
                            Status = "unpaid",  
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        await _context.TrnRepayments.AddAsync(repayment);
                    }

                    await _context.CashFlows.AddAsync(borrowerCashFlow);
                    await _context.CashFlows.AddAsync(lenderCashFlow);
                    _context.MstLoans.Update(loan);
                    _context.MstUsers.Update(borrower);
                    _context.MstUsers.Update(lender);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return new ResStatusLoanDto
                    {
                        Status = loan.Status
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
    }
}
