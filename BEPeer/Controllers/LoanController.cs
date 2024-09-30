using DAL.DTO.Req.LoanDto;
using DAL.DTO.Req.Pagination;
using DAL.DTO.Res;
using DAL.DTO.Res.LoadDto;
using DAL.DTO.Res.UserDto;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace BEPeer.Controllers
{
    [Route("rest/v1/loans")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly ILoanServices _loanService;
        private readonly ICashFlowServices _cashFlowServices;
        private readonly IUserServices _userServices;
        private readonly IConfiguration _configuration;
        public LoanController(ILoanServices loanService, ICashFlowServices cashFlowServices, IUserServices userServices, IConfiguration configuration)
        {
            _loanService = loanService;
            _cashFlowServices = cashFlowServices;
            _userServices = userServices;
            _configuration = configuration;
        }

        [Authorize(Roles = "borrower")]
        [Route("")]
        [HttpPost]
        public async Task<IActionResult> CreateLoan(ReqLoanDto loanDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Any())
                        .Select(x => new
                        {
                            Field = x.Key,
                            Messages = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                        }).ToList();
                    var errorMessage = new StringBuilder("Validation error occured!");
                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = errorMessage.ToString(),
                        Data = errors
                    });
                };
                var id = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(id))
                {
                    return Unauthorized(new ResBaseDto<string>
                    {
                        Success = false,
                        Message = "User ID not found in token",
                        Data = null
                    });
                }
                var result = await _loanService.CreateLoan(id, loanDto);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Loan requested.",
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

        [Authorize]
        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var result = await _loanService.GetLoanById(id);
                return Ok(new ResBaseDto<ResLoanDto>
                {
                    Success = true,
                    Message = "Get loan by id successfully",
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

        [Authorize(Roles = "lender")]
        [Route("{loanId}/fund")]
        [HttpPost]
        public async Task<IActionResult> UpdateStatusLoan(string loanId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Any())
                        .Select(x => new
                        {
                            Field = x.Key,
                            Messages = x.Value.Errors.Select(e => e.ErrorMessage).ToList()
                        }).ToList();
                    var errorMessage = new StringBuilder("Validation error occured!");
                    return BadRequest(new ResBaseDto<object>
                    {
                        Success = false,
                        Message = errorMessage.ToString(),
                        Data = errors
                    });
                };
                var id = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(id))
                {
                    return Unauthorized(new ResBaseDto<string>
                    {
                        Success = false,
                        Message = "User ID not found in token",
                        Data = null
                    });
                }
                var result = await _loanService.UpdateStatusLoan(loanId, id, "funded");
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Loan status updated successfully",
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

        [Authorize(Roles = "lender,admin")]
        [Route("fund/users/me")]
        [HttpGet]
        public async Task<IActionResult> GetFundedLoansByUserId([FromQuery] ReqPaginationQuery paginationQuery, [FromQuery] string status)
        {
            try
            {
                var id = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(id))
                {
                    return Unauthorized(new ResBaseDto<string>
                    {
                        Success = false,
                        Message = "User ID not found in token",
                        Data = null
                    });
                }

                Console.WriteLine(status);

                if (status != "funded" && status != "repaid")
                {
                    throw new ResErrorDto
                    {
                        Message = "Invalid status",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }

                var baseUrl = _configuration.GetSection("BaseURL").Value;
                var pagination = new ResPagination<List<ResLoanDto>>();
                pagination.OrderBy = paginationQuery.OrderBy ?? "CreatedAt";
                pagination.OrderDirection = paginationQuery.OrderDirection ?? "asc";
                pagination.PageNumber = paginationQuery.PageNumber;
                pagination.PageSize = paginationQuery.PageSize;

                var additionalQuery = new { status = status };
                pagination.Records = await _loanService.GetUserFunds(id, status, paginationQuery);
                pagination.TotalRecords = await _loanService.CountUserFunds(id, status, paginationQuery);
                pagination.SetUrls($"{baseUrl}{Request.Path.Value}", additionalQuery);

                return Ok(new ResBaseDto<ResPagination<List<ResLoanDto>>>
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

        [Authorize]
        [Route("users/me")]
        [HttpGet]
        public async Task<IActionResult> GetAllLoansByUserId([FromQuery] ReqPaginationQuery paginationQuery, string status)
        {
            try
            {
                Console.WriteLine(status);
                var id = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(id))
                {
                    return Unauthorized(new ResBaseDto<string>
                    {
                        Success = false,
                        Message = "User ID not found in token",
                        Data = null
                    });
                }
                Console.WriteLine(id);
                var baseUrl = _configuration.GetSection("BaseURL").Value;
                
                var pagination = new ResPagination<List<ResLoanDto>>();
                pagination.OrderBy = paginationQuery.OrderBy ?? "CreatedAt";
                pagination.OrderDirection = paginationQuery.OrderDirection ?? "asc";
                pagination.PageNumber = paginationQuery.PageNumber;
                pagination.PageSize = paginationQuery.PageSize;
                pagination.Records = await _loanService.GetUserLoans(id, paginationQuery, status);
                Console.WriteLine(pagination.Records);
                pagination.TotalRecords = await _loanService.CountUserLoans(id, paginationQuery, status);
                var additionalQuery = new { status = status };
                Console.WriteLine($"{baseUrl}{Request.Path.Value}" + additionalQuery);
                pagination.SetUrls($"{baseUrl}{Request.Path.Value}", additionalQuery);
                
                return Ok(new ResBaseDto<ResPagination<List<ResLoanDto>>>
                {
                    Success = true,
                    Message = "Get user fundings successfully",
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

        [Authorize(Roles = "admin,lender")]
        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetAllLoans([FromQuery] ReqPaginationQuery paginationQuery, string status)
        {
            try
            {
                var role = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
                if (string.IsNullOrEmpty(role))
                {
                    return Unauthorized(new ResBaseDto<string>
                    {
                        Success = false,
                        Message = "Role not found in token",
                        Data = null
                    });
                }
                if(role == "lender" && status != "requested")
                {
                    throw new ResErrorDto
                    {
                        Message = "Lender can only view loans with status 'requested'",
                        StatusCode = StatusCodes.Status403Forbidden
                    };
                }
                var baseUrl = _configuration.GetSection("BaseURL").Value;
                var pagination = new ResPagination<List<ResLoanDto>>();
                pagination.OrderBy = paginationQuery.OrderBy ?? "CreatedAt";
                pagination.OrderDirection = paginationQuery.OrderDirection ?? "asc";
                pagination.PageNumber = paginationQuery.PageNumber;
                pagination.PageSize = paginationQuery.PageSize;

                pagination.Records = await _loanService.GetAllLoans(paginationQuery, status);
                pagination.TotalRecords = await _loanService.CountAllLoans(paginationQuery, status);
                var additionalQuery = new { status = status };
                pagination.SetUrls($"{baseUrl}{Request.Path.Value}", additionalQuery);

                return Ok(new ResBaseDto<ResPagination<List<ResLoanDto>>>
                {
                    Success = true,
                    Message = "Get all loans successfully",
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
    }
}
