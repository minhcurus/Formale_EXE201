using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Domain.Entities;

namespace Application.Interface
{
    public interface IUserAccountService
    {
        Task<ResultMessage> Register(RegisterDTO registerDTO);
        Task<ResultMessage> Login(LoginDTO loginDTO);
        Task<ResultMessage> ChangePassword(ChangePasswordDTO changePasswordDTO);
        Task<ResultMessage> ActiveAccount(ActiveAccountDTO accountDTO);
        Task<ResultMessage> ResetOtp(ResetOtpDTO resetOtpDTO);
        Task<ResultMessage> Logout(TokenDTO tokenDTO);
        Task<ResultMessage> GoogleLogin(string accessToken);
        Task<ResultMessage> LoginWithGoogleCode(string code);

    }
}
