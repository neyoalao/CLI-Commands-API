using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CommandAPI.Models;
using CommandAPI.Token;
using Microsoft.AspNetCore.Authorization;
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
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IJWTTokenGenerator jWTTokenGenerator, RoleManager<IdentityRole> roleManager)
        {
            _jWTTokenGenerator = jWTTokenGenerator;
            _roleManager = roleManager;
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
            var roles = await _userManager.GetRolesAsync(userInDb);
            var claims = await _userManager.GetClaimsAsync(userInDb);


            return Ok(new
            {
                result = result,
                username = userInDb.UserName,
                email = userInDb.Email,
                token = _jWTTokenGenerator.GenerateToken(userInDb, roles, claims)
            });
        }


        //Register user
        // [Authorize(Policy = "OwnerDeveloper")]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            // creates the user role context in case it doesn't exists already
            if (!(await _roleManager.RoleExistsAsync(model.Role)))
            {
                await _roleManager.CreateAsync(new IdentityRole(model.Role));
            }

            var userToCreate = new IdentityUser
            {
                Email = model.Email,
                UserName = model.Username
            };

            //Create User
            var result = await _userManager.CreateAsync(userToCreate, model.Password);

            if (result.Succeeded)
            {
                var userInDb = await _userManager.FindByNameAsync(userToCreate.UserName);

                // add the created role to the user
                await _userManager.AddToRoleAsync(userInDb, model.Role);

                // Create claims and assign it to user
                var claim = new Claim("JobTitle", model.JobTitle);

                await _userManager.AddClaimAsync(userInDb, claim);

                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}