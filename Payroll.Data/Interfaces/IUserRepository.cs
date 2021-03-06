﻿using Microsoft.AspNetCore.Identity;
using Payroll.Core;
using Payroll.Data.Helpers;
using Payroll.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Payroll.Data.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> EmailExists(string email);
        Task<bool> UsernameExists(string username);
        Task<AppUser> GetUserByEmail(string email);
        Task<bool> ConfirmPassword(AppUser user, string password);
        Task<bool> SaveNewUser(AppUser user, string password);
        Task<bool> DeleteUser(AppUser user);
        Task<bool> SaveChangesAsync();
        Task<bool> UpdateUser(AppUser user);

        Task<bool> ConfirmCurrentPassword(AppUser user, string password);
        Task<bool> UpdateUserPassword(AppUser user, string currentPassword, string newPassword);

        Task<AppUser> GetUser(string username, bool withTimestamps = false);
        Task<PagedList<AppUser>> GetAllUsers(PageParameters pageParameters);
        Task<PagedList<AppUser>> GetAllManagers(PageParameters pageParameters);
    }
}
