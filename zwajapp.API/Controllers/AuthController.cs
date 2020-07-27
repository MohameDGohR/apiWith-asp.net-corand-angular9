using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using zwajapp.API.Data;
using zwajapp.API.DTOS;
using zwajapp.API.Models;

namespace zwajapp.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController :ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo ,IConfiguration config)
        {
            _repo =repo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserTransferRegisterDto userTransferRegisterDto)
        {

            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);
            }
            userTransferRegisterDto.username = userTransferRegisterDto.username.ToLower();

            if (await _repo.UserExists(userTransferRegisterDto.username))
                return BadRequest("هذا المستخدم مسجل من قبل ");
            var usertocreate = new User {
            Username = userTransferRegisterDto.username
            };

            var CreatedUser =  await _repo.Register(usertocreate, userTransferRegisterDto.password);
            // createdatroute()
            return StatusCode(201);
        }


        [HttpPost("login")]

        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {

            var userfromrepo = await _repo.Login(userForLoginDto.username.ToLower() , userForLoginDto.password);
            if (userfromrepo == null)
                return Unauthorized();
            var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier,userfromrepo.Id.ToString()),
            new Claim(ClaimTypes.Name , userfromrepo.Username)};

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key , SecurityAlgorithms.HmacSha512);
            var tokendescriptor = new SecurityTokenDescriptor { 
            Subject= new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(1),
            SigningCredentials = creds
            };
            var tokenhandler = new JwtSecurityTokenHandler();
            var token = tokenhandler.CreateToken(tokendescriptor);
            return Ok(new { token = tokenhandler.WriteToken(token)});
        
        }
        
    }
}
