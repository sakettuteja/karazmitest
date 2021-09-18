using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using TestProject.WebAPI.Data;
using TestProject.WebAPI.Models;
using TestProject.WebAPI.SeedData;

namespace TestProject.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        //private readonly RoleManager<User> _roleManager;

        public AuthController(UserManager<User> userManager, IMapper mapper)
        {
            this._userManager = userManager;
            //this._roleManager = roleManager;
            this._mapper = mapper;
        }

        [HttpPost]
        [Route("/Register")]
        public async Task<IActionResult> Register(Register register)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(register);
                }
                var user = _mapper.Map<User>(register);

                var result = await _userManager.CreateAsync(user, user.Password);

                if (!result.Succeeded)
                {
                    List<ErrorResponse> errorResponseList = new List<ErrorResponse>();

                    foreach (var error in result.Errors)
                    {
                        ErrorResponse errorResponse = new ErrorResponse() { ErrorCode = error.Code, ErrorDescription = error.Description };
                        errorResponseList.Add(errorResponse);
                    }
                    return BadRequest(errorResponseList);
                }
                //await _userManager.AddToRoleAsync(user, "Administrator");
                await _userManager.AddToRoleAsync(user, "Visitor");
                return Ok(register);
            }
            catch (Exception ex)
            {
                ErrorResponse errorResponse = new ErrorResponse()
                {
                    ErrorCode = "400",
                    ErrorDescription = ex.Message
                };

                return BadRequest(errorResponse);
                throw;
            }
        }

        [HttpPost, Route("/token")]
        public async Task<IActionResult> Login([FromBody]LoginUserForm user)
        {
            if (user == null)
            {
                return BadRequest("Invalid client request");
            }

            var user1 = await _userManager.FindByEmailAsync(user.Email);
            if (user1 != null)
            {
                if (await _userManager.CheckPasswordAsync(user1, user.Password))
                {
                    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
                    var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                    var roles = await _userManager.GetRolesAsync(user1);

                    List<Claim> claims = new List<Claim>();

                    claims.Add(new Claim(ClaimTypes.Name, user.Email));
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }
                    var tokeOptions = new JwtSecurityToken(
                        issuer: "http://localhost:44350",
                        audience: "http://localhost:4200",
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(5),
                        signingCredentials: signinCredentials
                    );
                    var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                    LoginResponseModel loginResponseModel = new LoginResponseModel()
                    {
                        AccessToken = tokenString,
                        Username = user.Email
                    };
                    return Ok(loginResponseModel);
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
