using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public static class OTPGenerator
    {
        public static string GenerateOTP(int length = 6)
        {
            const string chars = "0123456789";
            var otp = new char[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[length];
                rng.GetBytes(bytes);
                for (int i = 0; i < length; i++)
                {
                    otp[i] = chars[bytes[i] % chars.Length];
                }
            }
            return new string(otp);
        }
    }
}
