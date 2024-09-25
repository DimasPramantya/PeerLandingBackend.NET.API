using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services.Interfaces
{
    public interface ILoanServices
    {
        Task<string> CreateLoan(ReqLoanDto loanDto);
        Task<ResStatusLoanDto> UpdateStatusLoan(string loanId, ReqLoanStatusDto dto);
        Task<List<ResLoanDto>> GetAllLoans(string? status);
    }
}
