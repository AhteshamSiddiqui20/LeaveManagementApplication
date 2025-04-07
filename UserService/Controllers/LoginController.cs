using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using UserService.Entities;
using UserService.Model;
using System.Threading.Tasks;


namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _iconfing;
        private readonly IHttpClientFactory _httpClientFactory;
        public LoginController(ApplicationDbContext dbContext, IConfiguration configuration, IHttpClientFactory httpClientFactory   )
        {
            _context = dbContext;
            _iconfing = configuration;
            _httpClientFactory = httpClientFactory;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public IActionResult Authenticate([FromBody] userDto login)
        {
            var data = _context.AhteshamSohaibusers.Where(x => x.Username == login.Username && x.PasswordHash == login.PasswordHash).FirstOrDefault();
            if (data != null)
            {
                var Token = GenerateJSONWebToken(data);
                return Ok(Token);
            }
            else
            {
                return NotFound();
            }
            
        }

        private  string GenerateJSONWebToken(AhteshamSohaibusers user)
        {
            var claims = new List<Claim>
            {
            new Claim(JwtRegisteredClaimNames.Sub, (user.UserId).ToString()),
             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var role =  _context.AhteshamSohaibUserRoles.Where(x => x.RoleId == user.RoleId).FirstOrDefault();
                /*laims.Add(new Claim(ClaimTypes.Role, role.RoleName));*/
                claims.Add(new Claim("roleId", user.RoleId.ToString()));
                claims.Add(new Claim("RoleName", role.RoleName));
            claims.Add(new Claim("Email", user.Email));
            
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_iconfing["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _iconfing["Jwt:Issuer"],
                audience: _iconfing["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO user)
        {
            try
            {
                var existingUser = _context.AhteshamSohaibusers.FirstOrDefault(x => x.Email == user.Email);
                if (existingUser == null)
                {
                    var userObj = new AhteshamSohaibusers()
                    {
                        Username = user.Username,
                        Email = user.Email,
                        PasswordHash = user.PasswordHash,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        RoleId = user.RoleId
                    };

                    _context.AhteshamSohaibusers.Add(userObj);
                    _context.SaveChanges();

                    var httpClient = _httpClientFactory.CreateClient("LeaveService");
                    var objLeave = new LeaveDTO()
                    {
                        EmployeeId = userObj.UserId,
                        TotalLeaves = 5,
                        UsedLeaves = 0
                    };
                    var content = new StringContent(JsonSerializer.Serialize(objLeave), Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync("/api/LeaveManagement/LeaveCount", content);
                    return Ok(new { message = "User registered successfully!" });
                }
                else
                {
                    return Conflict(new { message = "User already exists with this email." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while registering the user.", error = ex.Message });
            }

        }
    }
}
