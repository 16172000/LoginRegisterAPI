using LoginRegisterAPI.Models;

namespace LoginRegisterAPI.Interface
{
    public interface ILoginRegister
    {
        Task<LoginTbl> LoginAsync(LoginTbl loginModel);
        Task<bool> RegisterAsync(Register registerModel);
    }
}
