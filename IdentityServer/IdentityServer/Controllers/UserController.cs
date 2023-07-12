using IdentityModel;
using IdentityServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace IdentityServer.Controllers
{
    [ApiController]
    //[Authorize]
    //  [Route("[User]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<IdentityUser<long>> _userManager;
        private readonly SignInManager<IdentityUser<long>> _signInManager;
        private readonly RoleManager<IdentityRole<long>> _roleManager;
        public UserController(
                UserManager<IdentityUser<long>> userManager
               , SignInManager<IdentityUser<long>> signInManager
                , RoleManager<IdentityRole<long>> roleManager
            //   , RoleManager<IdentityUserRole<long>> roleManager
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        /*    [HttpGet("role", Name ="GetRole")]
          public async Task<IActionResult> GetRole()
           {
               var roles = await _roleManager.Roles.ToListAsync();

               return Ok(roles);
           }*/
        [HttpGet("{roleName}/permissions")]
        public async Task<IActionResult> GetRolePermissions(string roleName)
        {
            // Tìm vai trò theo tên
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                return NotFound();
            }

            // Lấy danh sách quyền của vai trò
            var permissions = await _roleManager.GetRoleNameAsync(role);// GetClaimsAsync(role);

            // Trả về danh sách quyền
            return Ok(permissions);//.Select(c => new { Type = c.Type, Value = c.Value }));
        }
        [HttpGet("user",Name = "GetUser")]
        public async Task<IActionResult> GetUser()
        {
            var users = await _userManager.Users.ToListAsync();

            List<user> userList = new List<user>();

           users.ForEach(async e =>
            {
                var user = await  _userManager.FindByIdAsync(e.Id.ToString() );
                if (user != null)
                {
                    var roles =  _userManager.GetRolesAsync(user).Result;
                    userList.Add(new user
                    {
                        Name = e.UserName,
                        Email = e.Email,
                        Role = roles
                    });
                }
            });
            
            return Ok(userList);
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
            
            if (result.Succeeded)
            {   var info =  _userManager.Users
                    .FirstOrDefault(c => c.UserName == model.UserName);
                var user = await _userManager.FindByIdAsync(info.Id.ToString());
                var roles = _userManager.GetRolesAsync(user).Result.ToList();
                // setup Claim
                var claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, info.Id.ToString()));
                claims.Add(new Claim(ClaimTypes.Name, info.UserName));
                claims.Add(new Claim(ClaimTypes.Email, info.Email));
                claims.Add(new Claim("AspNet.Identity.SecurityStamp", info.SecurityStamp));
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
                   
                var secretKey = "SWHRMS_C421AAEE0D114E9C";//_configuration["Jwt:SecretKey"];
                var keyBytes = Encoding.UTF8.GetBytes(secretKey);
                var signingKey = new SymmetricSecurityKey(keyBytes);

                var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "SWHRMS",
                    audience: "SWHRMS",
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1), 
                    signingCredentials: credentials
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            else
            {
                return Unauthorized();
            }
        }


    }
}
