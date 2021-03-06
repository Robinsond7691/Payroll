﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Payroll.Core;
using Payroll.Data.Helpers;
using Payroll.Data.Interfaces;
using Payroll.Data.Models;
using Payroll.Data.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Data.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly PayrollContext _db;

        public UserRepository(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            PayrollContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }


        public async Task<AppUser> GetUser(string username, bool withTimestamps = false)
        {
            var query = _db.Users
                .Where(j => j.UserName == username);

            if (withTimestamps)
                query = _db.Users
                .Include(u => u.Timestamps)
                .ThenInclude(t => t.Jobsite)
                .Where(j => j.UserName == username);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<AppUser> GetUserByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> EmailExists(string email)
        {
            return await _db.Users.AnyAsync(user => user.Email == email);
        }

        public async Task<bool> UsernameExists(string username)
        {
            return await _db.Users.AnyAsync(user => user.UserName == username);
        }
        
        public async Task<bool> ConfirmPassword(AppUser user, string password)
        {
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (result.Succeeded)
                return true;
            //else
            return false;
        }

        public async Task<bool> SaveNewUser(AppUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
                return true;
            //else
            return false;
        }

        //Next two tasks for updating password - feature not implemented
        public async Task<bool> ConfirmCurrentPassword(AppUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }
        public async Task<bool> UpdateUserPassword(AppUser user, string currentPassword, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded)
                return true;
            //else
            return false;
        }

        public async Task<PagedList<AppUser>> GetAllUsers(PageParameters pageParameters)
        {
            var query = from u in _db.Users
                        select u;

            query = query
                .Where(u => u.Manager == false)
                .OrderBy(u => u.DisplayName);

            return await PagedList<AppUser>.ToPagedList(
                query, 
                pageParameters.PageNumber, 
                pageParameters.PageSize);
        }

        public async Task<bool> DeleteUser(AppUser user)
        {
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                return true;
            //else
            return false;
        }

        public async Task<bool> UpdateUser(AppUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return true;
            //else
            return false;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _db.SaveChangesAsync()) > 0;
        }

        public async Task<PagedList<AppUser>> GetAllManagers(PageParameters pageParameters)
        {
            var query = from u in _db.Users
                        select u;

            query = query
                .Where(u => u.Manager == true 
                            && u.Admin == false)
                .OrderBy(u => u.DisplayName);

            return await PagedList<AppUser>.ToPagedList(
                query,
                pageParameters.PageNumber,
                pageParameters.PageSize);
        }
    }
}
