﻿using FCMS.Interfaces.Repository;
using FCMS.Model.DTOs;
using FCMS.Model.Entities;
using FCMS.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FCMS.Implementations.Repository
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User> Get(Expression<Func<User, bool>> expression)
        {
            return await _context.Users
                .Include(x => x.Farmer)
                .Include(x => x.Customer)
                .Include(x => x.Chats)
                .FirstOrDefaultAsync(expression);
        }

        public Task<BaseResponse<User>> GetByAddress(Expression<Func<User, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public async Task<User> LoginAsync(UserLoginRequestModel model) => await _context.Users.FirstOrDefaultAsync(x => x.Email == model.Email && x.Password == model.Password);


    }
}
