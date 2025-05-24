using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using VaccinceCenter.Repositories.Base;

namespace Infrastructure.Repository
{
    public class UserAccountRepository : GenericRepository<UserAccount>
    {
        public UserAccountRepository(AppDBContext context) : base(context) { }

        public async Task<UserAccount> GetLogin(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(e => e.Email == email && e.Password == password);
            return user;
        }

        public async Task<UserAccount> GetEmail(string email)
        {
            var check = await _context.Users.FirstOrDefaultAsync(e => e.Email == email);
            return check;
        }

        public async Task<UserAccount> GetByToken(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Token == token);
        }

        public async Task<UserAccount> GetOtp(string otp)
        {
            return await _context.Users.FirstOrDefaultAsync(e => e.otp == otp);
        }


    }
}
