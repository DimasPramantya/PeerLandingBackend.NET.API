using DAL.DTO.Req.Pagination;
using DAL.DTO.Res.CashFlow;
using DAL.DTO.Res;
using DAL.DTO.Res.LoadDto;
using DAL.DTO.Res.RepaymentDto;
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
using System.Reflection;

namespace DAL.Repositories.Services
{
    public class RepaymentServices : IRepaymentServices
    {
        private readonly P2plandingContext _context;
        private readonly IConfiguration _configuration;
        public RepaymentServices(P2plandingContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<int> CountLoanRepayments(string loanId, string status, ReqPaginationQuery paginationQuery)
        {
            IQueryable<TrnRepayment> query = _context.TrnRepayments
               .Where(l => l.LoanId == loanId)
               .OrderByDescending(l => l.CreatedAt);

            if (status != "all")
            {
                query = query.Where(l => l.Status == status);
            }

            return await query.CountAsync();
        }

        public async Task<List<ResPaymentLoanDto>> GetLoanRepayments(string loanId, string status, ReqPaginationQuery paginationQuery)
        {
            IQueryable<TrnRepayment> query = _context.TrnRepayments
               .Where(l => l.LoanId == loanId)
               .OrderByDescending(l => l.CreatedAt);

            if (status != "all")
            {
                query = query.Where(l => l.Status == status);
            }

            query = query.Skip((paginationQuery.PageNumber - 1) * paginationQuery.PageSize)
                 .Take(paginationQuery.PageSize);

            var repayments = await query.Select(repayment => new ResPaymentLoanDto
            {
                Id = repayment.Id,
                LoanId = repayment.LoanId,
                Amount = repayment.Amount,
                Status = repayment.Status,
                CreatedAt = repayment.CreatedAt,
                UpdatedAt = repayment.UpdatedAt,
                DeadlineDate = repayment.DeadlineDate,
                RepaidAt = repayment.RepaidAt

            }).ToListAsync();

            return repayments;
        }

        public async Task<string> PaysTheLoan(string userId, ReqPaymentLoanDto reqPaymentLoanDto)
        {
            decimal paymentAmount = 0;

            string loanId = "";

            var borrower = await _context.MstUsers.FirstOrDefaultAsync(u => u.Id == userId);
            var admin = await _context.MstUsers.FirstOrDefaultAsync(u => u.Role == "admin");
            if (borrower == null)
            {
                throw new ResErrorDto
                {
                    Data = null,
                    Message = $"User with id-{userId} not found",
                    StatusCode = StatusCodes.Status404NotFound
                };
            }
            if (admin == null)
            {
                throw new ResErrorDto
                {
                    Data = null,
                    Message = $"Admin not found",
                    StatusCode = StatusCodes.Status404NotFound
                };
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach (var repaymentId in reqPaymentLoanDto.RepaymentIds)
                    {
                        var repayment = await _context.TrnRepayments.FirstOrDefaultAsync(r => r.Id == repaymentId);
                        if (repayment == null)
                        {
                            throw new ResErrorDto
                            {
                                Data = null,
                                Message = $"Repayment with id-{repaymentId} not found",
                                StatusCode = StatusCodes.Status404NotFound
                            };
                        }
                        repayment.Status = "paid";
                        repayment.RepaidAt = DateTime.UtcNow;
                        _context.Update(repayment);
                        paymentAmount += repayment.Amount;
                        loanId = repayment.LoanId;
                    }

                    if (borrower.Balance >= paymentAmount)
                    {
                        borrower.Balance -= paymentAmount;
                    }
                    else
                    {
                        throw new ResErrorDto
                        {
                            Data = null,
                            Message = "Insufficient balance",
                            StatusCode = StatusCodes.Status400BadRequest
                        };
                    }

                    await _context.CashFlows.AddAsync(new CashFlow
                    {
                        UserId = admin.Id,
                        Amount = paymentAmount,
                        Type = "IN",
                        Description = $"Payment from loan repayments with id {loanId}"
                    });

                    admin.Balance += paymentAmount;
                    _context.MstUsers.Update(admin);

                    _context.MstUsers.Update(borrower);

                    var loan = await _context.MstLoans.FirstOrDefaultAsync(l => l.Id == loanId);
                    if (loan == null)
                    {
                        throw new ResErrorDto
                        {
                            Data = null,
                            Message = $"Loan with id-{loanId} not found",
                            StatusCode = StatusCodes.Status404NotFound
                        };
                    }

                    loan.TotalRepaid += paymentAmount;
                    Console.WriteLine("TOTAL REPAIDJASDHJKAHGSDJKASHDJKASHDJK AHK " + loan.TotalRepaid);

                    _context.MstLoans.Update(loan);

                    var lender = await _context.MstUsers.FirstOrDefaultAsync(u => u.Id == loan.LenderId);
                    if (lender == null)
                    {
                        throw new ResErrorDto
                        {
                            Data = null,
                            Message = $"Lender with id-{loan.LenderId} not found",
                            StatusCode = StatusCodes.Status404NotFound
                        };
                    }

                    if (loan.TotalRepaid >= loan.TotalAmount)
                    {
                        loan.Status = "repaid";
                        _context.MstLoans.Update(loan);

                        lender.Balance += loan.Amount + ((loan.TotalAmount - loan.Amount) / 2);
                        admin.Balance -= loan.Amount + ((loan.TotalAmount - loan.Amount) / 2);
                        _context.MstUsers.Update(lender);
                        _context.MstUsers.Update(admin);

                        await _context.CashFlows.AddAsync(new CashFlow
                        {
                            UserId = userId,
                            Amount = paymentAmount * -1,
                            Type = "OUT",
                            Description = $"Payment for loan repayments to {lender.Name}"
                        });

                        await _context.CashFlows.AddAsync(new CashFlow
                        {
                            UserId = lender.Id,
                            Amount = loan.Amount + ((loan.TotalAmount - loan.Amount) / 2),
                            Type = "IN",
                            Description = $"Payment for loan repayments from {borrower.Name}"
                        });

                        await _context.CashFlows.AddAsync(new CashFlow
                        {
                            UserId = admin.Id,
                            Amount = (loan.Amount + ((loan.TotalAmount - loan.Amount) / 2)) * -1,
                            Type = "OUT",
                            Description = $"Payment for loan repayments to {lender.Name} with id {loan.Id}"
                        });
                    }
                    else
                    {
                        await _context.CashFlows.AddAsync(new CashFlow
                        {
                            UserId = userId,
                            Amount = paymentAmount * -1,
                            Type = "OUT",
                            Description = $"Payment for loan repayments to {lender.Name}"
                        });
                    }

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return $"Payment for loan repayments with id {loanId} is successful";
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
