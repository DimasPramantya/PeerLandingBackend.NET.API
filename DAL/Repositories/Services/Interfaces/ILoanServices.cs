using DAL.DTO.Req.LoanDto;
using DAL.DTO.Req.Pagination;
using DAL.DTO.Res.LoadDto;
using DAL.DTO.Res.LoanDto;
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
        Task<string> CreateLoan(string borrowerId, ReqLoanDto loanDto);
        Task<ResStatusLoanDto> UpdateStatusLoan(string loanId, string userId, string status);
        Task<List<ResLoanDto>> GetUserLoans(string borrowerId, ReqPaginationQuery paginationQuery, string status);
        Task<List<ResLoanDto>> GetAllLoans(ReqPaginationQuery paginationQuery, string status);
        Task<int> CountAllLoans(ReqPaginationQuery paginationQuery,  string status);
        Task<int> CountUserLoans(string borrowerId, ReqPaginationQuery paginationQuery, string status);
        Task<List<ResLoanDto>> GetUserFunds(string lenderId, string status, ReqPaginationQuery paginationQuery);
        Task<int> CountUserFunds(string lenderId, string status, ReqPaginationQuery paginationQuery);
        Task<ResLoanDto> GetLoanById(string loanId);
    }
}
