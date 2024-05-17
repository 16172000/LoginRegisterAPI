using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using TicketProjectWEB.Models;

namespace TicketProjectWEB.Controllers
{
    [SessionAuthorize]
    public class ForgetController : Controller
    {
        private readonly CoreProject5DbContext _context;

        public ForgetController(CoreProject5DbContext context)
        {
            _context = context;
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            var PasswordResetToken = GenerateUniqueToken();

            StoreTokenInDatabase(email, PasswordResetToken);

            SendResetEmail(email, PasswordResetToken);

            return View("ResetPassword");
        }

        public IActionResult ResetPassword(string email, string PasswordResetToken)
        {
            var isValid = VerifyToken(email, PasswordResetToken);

            if (isValid)
            {
                return View();
            }
            else
            {
                return View("ResetPasswordError");
            }
        }



        [HttpPost]
        public IActionResult ResetPassword(ResetPassword re, string email)
        {
            var isValid = VerifyToken(re.Email, re.PasswordResetToken);

            if (isValid)
            {
                if (re.Password == re.ConfirmPassword)
                {

                    string salt = PasswordHelper.GenerateSalt();
                    string passwordHash = PasswordHelper.GeneratePasswordHash(re.Password, salt);

                    var user = _context.Registers.SingleOrDefault(u => u.Email == email);

                    if (user != null)
                    {
                        user.Password = passwordHash;
                        user.Salt = salt;

                        _context.SaveChanges();
                    }

                    TempData["AlertMessage"] = "Password reset successfully.";
                    TempData["AlertType"] = "Success";

                    return View("ResetPasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                    return View(); 
                }
            }
            else
            {
                TempData["AlertMessage"] = "Token is invalid or expired.";
                TempData["AlertType"] = "danger";

                return View("ResetPasswordError");
            }
        }

        private string GenerateUniqueToken()
        {
            return Guid.NewGuid().ToString();
        }
        private void StoreTokenInDatabase(string email, string PasswordResetToken)
        {
            var user = _context.Registers.SingleOrDefault(u => u.Email == email);
            if (user != null)
            {
                user.PasswordResetToken = PasswordResetToken;
                user.TokenExpiration = DateTime.Now.AddHours(1); 

               _context.SaveChanges();
            }
        }
        private void SendResetEmail(string email, string PasswordResetToken)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                UseDefaultCredentials = false,

                Credentials = new NetworkCredential("katyayanaman1617@gmail.com", "xaohybnstaasydbl"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("katyayanaman1617@gmail.com"),
                Subject = "Password Reset",
                Body = $"Below is the following Token to Reset Your PASSWORD: {PasswordResetToken}",
            };

            mailMessage.To.Add(email);

            smtpClient.Send(mailMessage);
        }
        private bool VerifyToken(string email, string PasswordResetToken)
        {
            var user = _context.Registers.SingleOrDefault(u => u.Email == email);

            if (user != null && user.PasswordResetToken == PasswordResetToken && user.TokenExpiration > DateTime.Now)
            {
                return true;
            }
            return false;
        }
    }
}
