using MangoFusion_API.Models;
using MangoFusion_API.Models.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
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

            secretKey = configuration.GetValue<string>("ApiSetting:ScretKey") ?? "eeee";  // get secret key from appsettings.json
            _response = new ApiResponse();
            _userManager = userManager;
            _roleManager = roleManager;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser userUser = new ()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    NormalizedEmail = model.Email.ToUpper(),
                
                };
                var result = await _userManager.CreateAsync(userUser, model.Password); 
                if (result.Succeeded)
                {
                    //// check if role exists
                    //if (!await _roleManager.RoleExistsAsync(model.Role))
                    //{
                    //    await _roleManager.CreateAsync(new IdentityRole(model.Role));
                    //}
                    //// assign role to user
                    //await _userManager.AddToRoleAsync(userUser, model.Role);
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

    }


}

