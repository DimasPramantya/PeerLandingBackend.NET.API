using DAL.DTO.Req.Cash_Flow;
using DAL.DTO.Req.Pagination;
using DAL.DTO.Res.CashFlow;
using DAL.DTO.Res.RepaymentDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services.Interfaces
{
    public interface IRepaymentServices
    {
        Task<string> PaysTheLoan(string userId, ReqPaymentLoanDto reqPaymentLoanDto);
        Task<List<ResPaymentLoanDto>> GetLoanRepayments(string loanId, string status, ReqPaginationQuery paginationQuery);
        Task<int> CountLoanRepayments(string loanId, string status, ReqPaginationQuery paginationQuery);
    }
}
