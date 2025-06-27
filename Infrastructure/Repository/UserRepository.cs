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
    public class UserRepository : GenericRepository<UserAccount>
    {
        public UserRepository(AppDBContext context) : base(context)
        {
        }

        public async Task<List<UserAccount>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<UserAccount> GetById(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(e => e.UserId == id);
        }

        public async Task<UserAccount> GetByToken(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(e => e.Token == token);
        }

        public async Task<UserAccount> UpdateUserPremium(UserAccount user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<UserAccount> UpdateUser(UserAccount user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

    }
}
