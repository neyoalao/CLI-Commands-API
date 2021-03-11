using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CommandAPI.Models;
using CommandAPI.Token;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CommandAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IJWTTokenGenerator _jWTTokenGenerator;

        public IdentityController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IJWTTokenGenerator jWTTokenGenerator)
        {
            _jWTTokenGenerator = jWTTokenGenerator;
            _userManager = userManager;
            _signInManager = signInManager;

        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var userInDb = await _userManager.FindByNameAsync(loginModel.Username);
            if (userInDb == null)
            {
                return BadRequest();
            }

            // the false in the argument is used to prevent the user from being locked out after several failed attempts
            var result = await _signInManager.CheckPasswordSignInAsync(userInDb, loginModel.Password, false);

            if (!result.Succeeded)
            {
                return BadRequest();
            }
            IList<string> roles = new List<string>() { "admin" };
            IList<Claim> claims = new List<Claim>() { };

            return Ok(new
            {
                result = result,
                username = userInDb.UserName,
                email = userInDb.Email,
                token = _jWTTokenGenerator.GenerateToken(userInDb, roles, claims)
            });
        }

        //Register API

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {

            var userToCreate = new IdentityUser
            {
                Email = model.Email,
                UserName = model.Username
            };

            //Create User
            var result = await _userManager.CreateAsync(userToCreate, model.Password);

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}