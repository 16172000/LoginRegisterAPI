using LoginRegisterAPI.Interface;
using LoginRegisterAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginRegisterAPI.Repositry
{
    public class LoginRegisterRepositry : ILoginRegister
    {
        
        private readonly CoreProject5DbContext _context;

        public LoginRegisterRepositry(CoreProject5DbContext context)
        {
            _context = context;
        }

        public async Task<LoginTbl> LoginAsync(LoginTbl loginModel)
        {
            var user = await _context.Registers.FirstOrDefaultAsync(u => u.Email == loginModel.Email);

            if (user != null )
            {
                return new LoginTbl
                {
                    Id = user.Id,
                    Email = loginModel.Email
                };

            }

            return null;
        }

        public async Task<bool> RegisterAsync(Register registerModel)
        {
            if (await _context.Registers.AnyAsync(u => u.Email == registerModel.Email))
            {
                return false;
            }

            var data = new Register
            {
                Id = registerModel.Id,
                UserName = registerModel.UserName,
                Email = registerModel.Email,
                Password = registerModel.Password,
                ConfirmPassword = registerModel.Password,
                Dob = registerModel.Dob,
                State = registerModel.State,
                PhoneNumber = registerModel.PhoneNumber,
                Age = registerModel.Age
            };

            _context.Registers.Add(data);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}

