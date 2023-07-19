using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SeaExpenBackend.DB;
using SeaExpenBackend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace SeaExpenBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private const string TokenSecret = "ThisIsASampleJsonWebTokenKey";
        
        [Route("{id}")]
        [HttpGet]
        public IActionResult Get(int id)
        {
            try
            {
                var context = new SeaExpenContext();
                
                    var data = from u in context.Users
                               join r in context.Roles on u.Role equals r.Id
                               where u.Id == id
                               select new UserModel
                               {
                                   Id = u.Id,
                                   Name = u.Name,
                                   Username = u.Username,
                                   Email = u.Email,
                                   Password = u.Password,
                                   Role = u.Role,
                                   RoleName = r.RoleName,
                                   IsActive = u.IsActive
                               };
                return Ok(data);
            }
            catch
            {
                return StatusCode(500, new { error = "Server Error" });
            }
        }

        [Route("login")]
        [HttpPost]
        public IActionResult Post([FromBody] UserLoginModel model)
        {
            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            using(var context = new SeaExpenContext())
            {
                var user = context.Users.FirstOrDefault(x => x.Username.Equals(model.Username));
                //var user = context.Users.FirstOrDefault(x => (x.Username).Equals(model.Username));

                if (user != null && user.Password == model.Password)
                {
                    try
                    {
                        var TokenHandler = new JwtSecurityTokenHandler();
                        var key = Encoding.UTF8.GetBytes(TokenSecret);

                        var claims = new List<Claim>
                        {
                            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new(JwtRegisteredClaimNames.Sub, user.Email),
                            new(JwtRegisteredClaimNames.Email, user.Email),
                            new(ClaimTypes.Role, user.Role == 1 ? "Admin" : "User"),
                            new("UserId", user.Id.ToString()),
                        };

                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(claims),
                            Expires = DateTime.UtcNow.AddDays(1),
                            Issuer = "http://seaexpen.somee.com",
                            Audience = "*",
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
                        };

                        var token = TokenHandler.CreateToken(tokenDescriptor);

                        var jwt = TokenHandler.WriteToken(token);

                        return Ok(jwt);
                    }
                    catch (Exception)
                    {
                        return StatusCode(500, new { error = "Server Error" });
                    }
                    
                }

                return Unauthorized(new { error = "Invalid Credentials" });
            }
        }

        [Route("register")]
        [HttpPost]
        public IActionResult Post([FromBody] UserModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
        
            try
            {
                using( var context = new SeaExpenContext())
                {
                    var user = context.Users.FirstOrDefault(x => x.Username.Equals(model.Username));

                    if (user != null)
                    {
                        return BadRequest(new {error="Already exists"});
                    }

                    user = null;

                    var i = context.Database.ExecuteSql($"exec usp_Manage_User @Action=1, @Name={model.Name}, @Username={model.Username}, @Password={model.Password}, @Email={model.Email} , @Role=2");

                    if(i != 0)
                    {
                        return Ok(new { message = "User created successful." });
                    }

                    return BadRequest(new {message = "User creation failed"});
                }
            } catch (Exception ex)
            {
                return StatusCode(500, new { error = "Server Error" });
            }
        }

        [Route("userid")]
        [HttpGet]
        [Authorize(Roles = "User,Admin")]
        public  IActionResult Get()
        {
            try
            {
                var val = HttpContext.User.Claims.First(i => i.Type == "UserId").Value.ToString();
                var role = HttpContext.User.Claims.First(i => i.Type == ClaimTypes.Role).Value.ToString();
                return Ok(new { message = $"User id: {val}, Role: {role} " });

            } catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
