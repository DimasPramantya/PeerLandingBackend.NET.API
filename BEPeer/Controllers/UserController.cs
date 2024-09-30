using DAL.DTO.Req;
using DAL.DTO.Req.Cash_Flow;
using DAL.DTO.Req.Pagination;
using DAL.DTO.Req.UserDto;
using DAL.DTO.Res;
using DAL.DTO.Res.CashFlow;
using DAL.DTO.Res.UserDto;
using DAL.Enum;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace BEPeer.Controllers
{
    [Route("rest/v1/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly IConfiguration _configuration;
        private readonly ICashFlowServices _cashFlowServices;
        public UserController(IUserServices userServices, IConfiguration configuration, ICashFlowServices cashFlowServices)
        {
            _userServices = userServices;
            _configuration = configuration;
            _cashFlowServices = cashFlowServices;
        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register(ReqRegisterUserDto register)
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
                if(register.Role == "admin")
                {
                    throw new ResErrorDto
                    {
                        Message = "You can't register as admin",
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                }
                var user = await _userServices.Register(register);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "User registered successfully",
                    Data = user
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

        [Authorize(Roles = "admin")]
        [Route("add-user")]
        [HttpPost]
        public async Task<IActionResult> AddUser(ReqRegisterUserDto register)
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
                var user = await _userServices.Register(register);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "User registered successfully",
                    Data = user
                });
            }
            catch (ResErrorDto ex)
            {
                Console.WriteLine("Error add user: " + ex.Message);
                return StatusCode(ex.StatusCode, new ResBaseDto<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null,
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error add user: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [Authorize(Roles = "admin")]
        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] ReqPaginationQuery paginationQuery
        ){
            try
            {
                var baseUrl = _configuration.GetSection("BaseURL").Value;
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

                var pagination = new ResPagination<List<ResUserDto>>();
                pagination.OrderBy = paginationQuery.OrderBy ?? "CreatedAt";
                pagination.OrderDirection = paginationQuery.OrderDirection ?? "asc";
                pagination.PageNumber = paginationQuery.PageNumber;
                pagination.PageSize = paginationQuery.PageSize;
                pagination.Records = await _userServices.GetAllUsers(paginationQuery);
                pagination.TotalRecords = await _userServices.GetCount(paginationQuery);
                pagination.SetUrls($"{baseUrl}{Request.Path.Value}");

                return Ok(new ResBaseDto<ResPagination<List<ResUserDto>>>
                {
                    Success = true,
                    Message = "User fetched succesfully",
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

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login(ReqLoginDto login)
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
                var user = await _userServices.Login(login);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "User logged in successfully",
                    Data = user
                });
            }
            catch (ResErrorDto ex)
            {
                Console.WriteLine("Error login: " + ex.Message);
                return StatusCode(ex.StatusCode, new ResBaseDto<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null,
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error login: " + ex.Message);
                if (ex.Message == "Invalid Email or Password")
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new ResBaseDto<object>
                    {
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new ResBaseDto<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _userServices.GetUserById(id);
                await _userServices.DeleteUserById(id);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "User deleted successfully",
                    Data = null
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
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update(string id, ReqEditUserDto dto)
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
                await _userServices.GetUserById(id);
                var user = await _userServices.UpdateUserById(id, dto);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "User updated successfully",
                    Data = user
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
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            Console.WriteLine(id);
            try
            {
                var user = await _userServices.GetUserById(id);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "User fetched successfully",
                    Data = user
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
        [HttpGet]
        [Route("me")]
        public async Task<IActionResult> GetMe()
        {
            try
            {
                var id = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine(id);
                Console.WriteLine("Test");
                if (string.IsNullOrEmpty(id))
                {
                    return Unauthorized(new ResBaseDto<string>
                    {
                        Success = false,
                        Message = "User ID not found in token",
                        Data = null
                    });
                }

                var user = await _userServices.GetUserById(id);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "User fetched successfully",
                    Data = user
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
        [HttpPatch]
        [Route("balances")]
        public async Task<IActionResult> UpdateBalance([FromBody] ReqCashFlowDto reqCashFlowDto)
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
                var cashFlow = await _cashFlowServices.CreateCashFlow(id, reqCashFlowDto);
                return Ok(new ResBaseDto<object>
                {
                    Success = true,
                    Message = "Balance updated successfully",
                    Data = cashFlow
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
        [HttpGet]
        [Route("me/cash-flow")]
        public async Task<IActionResult> GetUserCashFlows([FromQuery] ReqPaginationQuery paginationQuery)
        {
            try
            {
                var baseUrl = _configuration.GetSection("BaseURL").Value;
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
                var pagination = new ResPagination<List<ResCashFlowDto>>();
                pagination.OrderBy = paginationQuery.OrderBy ?? "CreatedAt";
                pagination.OrderDirection = paginationQuery.OrderDirection ?? "asc";
                pagination.PageNumber = paginationQuery.PageNumber;
                pagination.PageSize = paginationQuery.PageSize;
                pagination.Records = await _cashFlowServices.GetUserCashFlows(id, paginationQuery);
                pagination.TotalRecords = await _cashFlowServices.CountUserCashFlows(id, paginationQuery);
                pagination.SetUrls($"{baseUrl}{Request.Path.Value}");

                return Ok(new ResBaseDto<ResPagination<List<ResCashFlowDto>>>
                {
                    Success = true,
                    Message = "User fetched succesfully",
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
