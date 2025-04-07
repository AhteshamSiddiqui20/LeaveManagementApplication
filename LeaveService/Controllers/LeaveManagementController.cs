using LeaveService.Entities;
using LeaveService.Model;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;


namespace LeaveService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveManagementController : ControllerBase
    {
        private readonly LeaveApplicationDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IHttpClientFactory _httpClientFactory;
        public LeaveManagementController(LeaveApplicationDbContext context, IPublishEndpoint publishEndpoint, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("LeaveCount")]
        public IActionResult LeaveCount([FromBody] LeaveDTO leave)
        {
            try
            {
                var leaveObj = new LeaveBalance()
                {
                    EmployeeId = leave.EmployeeId,
                    TotalLeaves = leave.TotalLeaves,
                    UsedLeaves = leave.UsedLeaves,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.LeaveBalance.Add(leaveObj);
                _context.SaveChanges();

                return Ok(new { message = "Leave Applied Successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while applying leave.", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("LeaveApply")]
        public async Task<IActionResult> LeaveApply([FromBody] LeaveApplyDTO leave)
        {
            try
            {
              
                TokenModel token = GetTokenDataModel(HttpContext);
                
                
                if (token != null)
                {
                var leaveObj = new LeaveRequest()
                {
                    EmployeeId = token.UserID,
                    Status = leave.Status,
                    StartDate = leave.StartDate,
                    EndDate = leave.EndDate,
                    Comments = leave.Comments,
                    CreatedAt = DateTime.UtcNow,

                };
                    _context.LeaveRequest.Add(leaveObj);
                    _context.SaveChanges();
                     await _publishEndpoint.Publish(new LeaveEvent
                     {
                        FromUser = token.Email,
                        ToUser = "manager@yopmail.com",
                        Status = "Pending"
                    });

                    var LeaveBalance = _context.LeaveBalance.FirstOrDefault(x => x.EmployeeId == leave.EmployeeId); 
                    if (LeaveBalance != null)
                    {
                        LeaveBalance.UsedLeaves += 1;
                        LeaveBalance.TotalLeaves -= 1;
                        LeaveBalance.UpdatedAt = DateTime.UtcNow;
                        _context.LeaveBalance.Update(LeaveBalance);
                        _context.SaveChanges();
                    }
                    else
                    {
                        return NotFound("Leave balance not found.");
                    }
                    return Ok(new { message = "Leave Applied Successfully!" });
                }
                else
                {
                    return BadRequest(new { message = "Something went wrong" });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while applying leave.", error = ex.Message });
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApproveLeave(int id)
        {
            TokenModel token = GetTokenDataModel(HttpContext);
            var leave = _context.LeaveRequest.FirstOrDefault(x => x.Id == id);
            if (leave == null) return NotFound("Leave request not found.");

            leave.Status = "Approved";

            // Publish LeaveEvent to RabbitMQ
            await _publishEndpoint.Publish(new LeaveEvent
            {
                FromUser = token.Email,
                ToUser = leave.EmployeeEmail,
                Status = "Approved"
            });

            return Ok(new { Message = "Leave Approved" });
        }

        [Authorize(Roles = "Manager")]
        [HttpPut("reject/{id}")]
        public async Task<IActionResult> RejectLeave(int id)
        {
            TokenModel token = GetTokenDataModel(HttpContext);
            var leave = _context.LeaveRequest.FirstOrDefault(x => x.Id == id);
            if (leave == null) return NotFound("Leave request not found.");

            leave.Status = "Rejected";

            // Publish LeaveEvent to RabbitMQ
            await _publishEndpoint.Publish(new LeaveEvent
            {
                FromUser = token.Email,
                ToUser = leave.EmployeeEmail,
                Status = "Rejected"
            });

            return Ok(new { Message = "Leave Rejected" });
        }



        private TokenModel GetTokenDataModel(HttpContext request)
        {
            //InternalLogger.Error("GetTokenDataModel  Begin ");           

            TokenModel token = null;
            //#if !DEBUG
            StringValues authorizationToken;
            StringValues timezone;
            StringValues ipAddress;
            JsonModel response = new JsonModel();
            var authHeader = request.Request.Headers.TryGetValue("Authorization", out authorizationToken);
            var authToken = authorizationToken.ToString().Replace("Bearer", "").Trim();
           

            try
            {
         
                if (authToken != null)
                {
                    var encryptData = GetDataFromToken(authToken, request);
                   

                    if (encryptData != null && encryptData.Claims != null)
                    {
                        var RoleID = 0;

                        if (!string.IsNullOrEmpty(encryptData.Claims[2].Value))
                        {
                            RoleID = Convert.ToInt32(encryptData.Claims[2].Value);
                        }



                        token = new TokenModel()
                        {
                            UserID = Convert.ToInt32(encryptData.Claims[0].Value),
                            RoleID = Convert.ToInt32(encryptData.Claims[2].Value),
                            Email = Convert.ToString(encryptData.Claims[4].Value),
                            RoleName = Convert.ToString(encryptData.Claims[3].Value)

                        };
                        
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }


            return token;
        }
        private dynamic GetDataFromToken(string token, HttpContext request)
        {
            var url = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(request.Request);
            try
            {
              

                var handler = new JwtSecurityTokenHandler();
                return handler.ReadToken(token);

            }
            catch (Exception ex)
            {
              
                return ex;
            }

        }

    }
}

