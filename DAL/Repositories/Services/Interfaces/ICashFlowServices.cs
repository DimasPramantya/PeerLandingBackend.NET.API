using DAL.DTO.Req.Cash_Flow;
using DAL.DTO.Req.Pagination;
using DAL.DTO.Res.CashFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services.Interfaces
{
    public interface ICashFlowServices
    {
        Task<ResCashFlowDto> CreateCashFlow(string userId, ReqCashFlowDto cashFlowDto);
        Task<List<ResCashFlowDto>> GetUserCashFlows(string userId, ReqPaginationQuery paginationQuery);
        Task<int> CountUserCashFlows(string userId, ReqPaginationQuery paginationQuery);
    }
}
