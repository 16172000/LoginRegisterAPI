using LoginRegisterAPI.Interface;
using LoginRegisterAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace LoginRegisterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ILoginRegister _LoginRegister;
        private readonly CoreProject5DbContext _context;
        private IConfiguration _config;


        public ProjectController(ILoginRegister LoginRegister, CoreProject5DbContext context, IConfiguration config)
        {
            _LoginRegister = LoginRegister;
            _context = context;
            _config = config;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            var obj = await _context.Registers.ToListAsync();
            return Ok(obj);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginTbl loginModel)
        {
            //var user = await _LoginRegister.LoginAsync(loginModel.Email, loginModel.Password);

            try
            {
                if (loginModel.Email != null || loginModel.Password != null)
                {
                    var result = await _LoginRegister.LoginAsync(loginModel);
                    if (result != null)

                    {
                        var tokenString = GenerateJSONWebToken(result);
                        return Ok(new { token = tokenString });
                        // return Ok(result);

                    }
                    return BadRequest(new { Error = "Invalid credentials" });

                }

                return BadRequest(new { Error = "Invalid credentials" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string GenerateJSONWebToken(LoginTbl user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("ProductDetails")]
        public async Task<IActionResult> ProductDetail()
        {
            var data = await _context.Products.ToListAsync();
            return Ok(data);
        }

        [HttpGet("getRegister")]
        public IActionResult Get()
        {
            try
            {
                var data = _context.Registers.ToList();
                if (data.Count == 0)
                {
                    return NotFound("Please insert the data...");
                }
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(Register registerModel)
        {
            var result = await _LoginRegister.RegisterAsync(registerModel);

            if (result)
            {
                return Ok(new { Message = "Registration successful" });
            }

            return BadRequest(new { Error = "Email already exists" });
        }

        [HttpGet("getRegister/{id}")]
        public IActionResult Edit(int id)
        {
            try
            {
                var register = _context.Registers.FirstOrDefault(r => r.Id == id);

                if (register == null)
                {
                    return NotFound($"Record with ID {id} not found.");
                }

                return Ok(register);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("register/{id}")]
        public async Task<IActionResult> Edit(int id, Register register)
        {
            try
            {
                if (id != register.Id)
                {
                    return BadRequest("ID mismatch in the request.");
                }

                // Validate and update the entity
                _context.Entry(register).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Data Updated Successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var register = _context.Registers.Find(id);

                if (register == null)
                {
                    return NotFound($"Record with ID {id} not found.");
                }

                _context.Registers.Remove(register);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Data Deleted Successfully..." });
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }

        }






    }
}
