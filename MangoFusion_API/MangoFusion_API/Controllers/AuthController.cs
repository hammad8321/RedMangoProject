using MangoFusion_API.Models;
using MangoFusion_API.Models.Dto;
using MangoFusion_API.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MangoFusion_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {

        // default role is identity role
        private readonly ApiResponse _response;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;  // to manage roles
        //private readonly IConfiguration _configuration;  // to access appsettings.json for secret key
        //private readonly ApiDbContext _dbContext;  // to access database for user details
        private readonly string secretKey;  // to store secret key from appsettings.json


        public AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration) //, ApiDbContext dbContext
        {

            secretKey = configuration.GetValue<string>("ApiSetting:SecretKey") ?? "";  // get secret key from appsettings.json
            _response = new ApiResponse();
            _userManager = userManager;
            _roleManager = roleManager;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO model)
        {
            if (ModelState.IsValid)
            {
              
                ApplicationUser newUser = new()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    NormalizedEmail = model.Email.ToUpper(),

                };
                var result = await _userManager.CreateAsync(newUser, model.Password);
                if (result.Succeeded)
                {
                 
                    // check if role exists
                    if (!await _roleManager.RoleExistsAsync(SD.Role_Admin))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer));
                    }
                    if (model.Role.Equals(SD.Role_Admin, StringComparison.CurrentCultureIgnoreCase))
                    {
                        await _userManager.AddToRoleAsync(newUser, SD.Role_Admin);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(newUser, SD.Role_Customer);

                    }
                    // assign role to user
                    await _userManager.AddToRoleAsync(newUser, model.Role);
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    return Ok(_response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    foreach (var error in result.Errors)
                    {
                        _response.ErrorMessage.Add(error.Description);
                    }
                    return BadRequest(_response);
                }

            }
            else
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessage.Add("Model state is invalid");
                return BadRequest(_response);

            }

        }






        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            if (ModelState.IsValid)
            {

                var userFromDb = await _userManager.FindByEmailAsync(model.UserEmail);
                if (userFromDb != null)
                {
                    bool isValid = await _userManager.CheckPasswordAsync(userFromDb, model.Password);
                    if (!isValid)
                    {
                        // _userManager.AccessFailedAsync(userFromDb); // increment access failed count
                        _response.Result = new LoginResponseDTO();

                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.ErrorMessage.Add("Invalid email or password");

                        return BadRequest(_response);
                    }
                  

                    JwtSecurityTokenHandler tokenHandler = new();
                        byte[] key = Encoding.ASCII.GetBytes(secretKey);
                  //     byte[] key = Encoding.UTF32.GetBytes(secretKey);
                 //    key =  Encoding.UTF8.GetBytes(secretKey);

                    //var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

                    SecurityTokenDescriptor tokenDescriptor = new()
                    {
                        Subject = new ClaimsIdentity([
                            new ("fullname", userFromDb.Name),
                            new ("id", userFromDb.Id),
                       //     new ("roles", string.Join(";", await _userManager.GetRolesAsync(userFromDb))), // multiple roles separated by ;
                           // new Claim(ClaimTypes.Name, userFromDb.Id.ToString()),  // user id as name claim
                            new Claim(ClaimTypes.Email, userFromDb.Email),  // email claim
                           // new (ClaimTypes.Role, string.Join(";", await _userManager.GetRolesAsync(userFromDb))) // multiple roles separated by ;
                            new (ClaimTypes.Role,  _userManager.GetRolesAsync(userFromDb).Result.FirstOrDefault()!)


                            ]),
                        Expires = DateTime.UtcNow.AddDays(7), // token valid for 7 days
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    LoginResponseDTO loginResponse = new()
                    {
                        Email = userFromDb.Email,
                        Token = tokenHandler.WriteToken(token),
                        Role = _userManager.GetRolesAsync(userFromDb).Result.FirstOrDefault()!
                    };
                    _response.Result = loginResponse;
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;
                    return Ok(_response);







                }


                _response.Result = new LoginResponseDTO();

                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessage.Add("Invalid email or password");

                return BadRequest(_response);
            }




            else
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessage.Add("Model state is invalid");
                return BadRequest(_response);

            }

        }



    }
}

