using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TicketProjectWEB.Models;

namespace TicketProjectWEB.Controllers
{
    public class AccountController : Controller
    {
        private CoreProject5DbContext _context;

        public AccountController()
        {
            _context = new CoreProject5DbContext();
        }

        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Register user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string salt = PasswordHelper.GenerateSalt();
                    string passwordHash = PasswordHelper.GeneratePasswordHash(user.Password, salt);

                    var newUser = new Register
                    {
                        UserName = user.UserName,
                        Email = user.Email,
                        Password = passwordHash,
                        Salt = salt,
                        ConfirmPassword = passwordHash,
                        Dob = user.Dob,
                        Age = user.Age,
                        State = user.State,
                        PhoneNumber = user.PhoneNumber,
                        PasswordResetToken = user.PasswordResetToken,
                        TokenExpiration = user.TokenExpiration

                    };
                    _context.Add(newUser);
                    _context.SaveChanges();

                    return RedirectToAction("Login");
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlException)
                    {
                        if (sqlException.Number == 2601 || sqlException.Number == 2627)
                        {
                            ModelState.AddModelError("Email", "The email address is already registered....");
                        }
                        else
                        {
                            throw;
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(user);
        }


        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginTbl loginModel)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Registers.FirstOrDefault(u => u.Email == loginModel.Email);

                if (user != null)
                {
                    string enteredPasswordHash = PasswordHelper.GeneratePasswordHash(loginModel.Password, user.Salt);

                    if (enteredPasswordHash == user.Password)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.UserName),
                        };

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        HttpContext.Session.SetString("UserName", user.UserName);

                        if (user.Email == "admin@gmail.com")
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                        else
                        {
                            return RedirectToAction("Index", "User");
                        }
                    }
                    else
                    {
                        ViewBag.Error = "Invalid password";
                    }
                }
                else
                {
                    ViewBag.Error = "User not found";
                }
            }

            return View(loginModel);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //HttpContext.Session.Remove("id");
            //HttpContext.Session.Remove("username");
            //HttpContext.Session.Remove("tokenno");
            //HttpContext.Session.Remove("emailid");
            //HttpContext.Session.Remove("deptid");
            //HttpContext.Session.Remove("plantid");
            HttpContext.Session.Clear();
            HttpContext.Response.Cookies.Delete(".WebTrainingRoom.Session");
            await HttpContext.SignOutAsync();

            return RedirectToAction("Login");
        }
    }
}
