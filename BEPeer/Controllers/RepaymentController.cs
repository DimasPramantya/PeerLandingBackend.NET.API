using DAL.DTO.Req.LoanDto;
using DAL.DTO.Req.Pagination;
using DAL.DTO.Res;
using DAL.DTO.Res.LoadDto;
using DAL.DTO.Res.RepaymentDto;
using DAL.DTO.Res.UserDto;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace BEPeer.Controllers
{
    [Route("rest/v1/repayments")]
    [ApiController]
    public class RepaymentController : ControllerBase
    {
        private readonly ILoanServices _loanService;
        private readonly ICashFlowServices _cashFlowServices;
        private readonly IUserServices _userServices;
        private readonly IRepaymentServices _repaymentServices;
        private readonly IConfiguration _configuration;
        public RepaymentController(
            ILoanServices loanService, 
            ICashFlowServices cashFlowServices, 
            IUserServices userServices,
            IRepaymentServices repaymentServices,
            IConfiguration configuration
        )
        {
            _loanService = loanService;
            _cashFlowServices = cashFlowServices;
            _userServices = userServices;
            _repaymentServices = repaymentServices;
            _configuration = configuration;
        }

        [Authorize]
        [HttpGet("loans/{loanId}")]
        public async Task<IActionResult> GetLoanRepayments(string loanId, [FromQuery] ReqPaginationQuery paginationQuery, [FromQuery] string status)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ResBaseDto<string>
                    {
                        Success = false,
                        Message = "User ID not found in token",
                        Data = null
                    });
                }
                if (status != "all" && status != "paid" && status != "unpaid")
                {
                    return BadRequest(new ResBaseDto<string>
                    {
                        Success = false,
                        Message = "Invalid status",
                        Data = null
                    });
                }

                var baseUrl = _configuration.GetSection("BaseURL").Value;
                var pagination = new ResPagination<List<ResPaymentLoanDto>>();
                pagination.OrderBy = paginationQuery.OrderBy ?? "CreatedAt";
                pagination.OrderDirection = paginationQuery.OrderDirection ?? "asc";
                pagination.PageNumber = paginationQuery.PageNumber;
                pagination.PageSize = paginationQuery.PageSize;

                var additionalQuery = new { status = status };
                pagination.Records = await _repaymentServices.GetLoanRepayments(loanId, status, paginationQuery);
                pagination.TotalRecords = await _repaymentServices.CountLoanRepayments(loanId, status, paginationQuery);
                pagination.SetUrls($"{baseUrl}{Request.Path.Value}", additionalQuery);

                return Ok(new ResBaseDto<ResPagination<List<ResPaymentLoanDto>>>
                {
                    Success = true,
                    Message = "Get all funded loans successfully",
                    Data = pagination
                });
            }
            catch (ResErrorDto ex)
            {
                return StatusCode(ex.StatusCode, new ResBaseDto<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [Authorize(Roles = "admin,borrower")]
        [HttpPost("loans")]
        public async Task<IActionResult> PaysTheLoan([FromBody] ReqPaymentLoanDto reqPaymentLoanDto)
        {
            try
            {
                var userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ResBaseDto<string>
                    {
                        Success = false,
                        Message = "User ID not found in token",
                        Data = null
                    });
                }
                var result = await _repaymentServices.PaysTheLoan(userId, reqPaymentLoanDto);
                return Ok(new ResBaseDto<string>
                {
                    Success = true,
                    Message = "Loan paid successfully",
                    Data = result
                });

            }
            catch (ResErrorDto ex)
            {
                return StatusCode(ex.StatusCode, new ResBaseDto<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}
