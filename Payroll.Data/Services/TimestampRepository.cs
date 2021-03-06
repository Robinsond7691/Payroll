﻿using Microsoft.EntityFrameworkCore;
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
    public class TimestampRepository : ITimestampRepository
    {
        private readonly PayrollContext _db;

        public TimestampRepository(PayrollContext db)
        {
            _db = db;
        }

        public async Task<bool> AddTimestamp(Jobsite jobsite, AppUser user, DateTimeOffset clockedIn, DateTimeOffset clockedOut)
        {
            var timestamp = new Timestamp
            {
                Jobsite = jobsite,
                AppUser = user,
                ClockedIn = false,
                ClockedInStamp = clockedIn,
                ClockedOutStamp = clockedOut
                
            };
            _db.Timestamps.Add(timestamp);

            return await _db.SaveChangesAsync() > 0;

        }

        public async Task<bool> DeleteTimestamp(int timestampId)
        {
            var timestamp = await _db.Timestamps.FirstOrDefaultAsync(t => t.TimestampId == timestampId);
            if (timestamp != null)
            {
            _db.Timestamps.Remove(timestamp);
            return await _db.SaveChangesAsync() > 0;
            } else
            {
                return false;
            }
        }

        public async Task<Timestamp> GetTimestamp(int timestampId)
        {
            //this is solely for the purpose of editing. There is no GetTimestamp route.
            var timestamp = await _db.Timestamps.FirstOrDefaultAsync(t => t.TimestampId == timestampId);
            return timestamp;            
        }

        public async Task<bool> ClockIn(Jobsite jobsite, AppUser user)
        {
            var timestamp = new Timestamp
            {
                Jobsite = jobsite,
                AppUser = user,
                ClockedIn = true,
                ClockedInStamp = DateTimeOffset.Now
            };
            _db.Timestamps.Add(timestamp);

            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> ClockInLunch(AppUser user)
        {
            var clockedInTimestamp = await this.GetClockedInTimestamp(user);
            clockedInTimestamp.LunchStamp = DateTimeOffset.Now;
            
            return await _db.SaveChangesAsync() > 0;
                    
        }

        public async Task<bool> ClockOut(AppUser user)
        {
            var clockedInTimestamp = await this.GetClockedInTimestamp(user);
            clockedInTimestamp.ClockedIn = false;
            clockedInTimestamp.ClockedOutStamp = DateTimeOffset.Now;

            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<ICollection<Timestamp>> GetTimestamps()
        {
            var query = _db.Timestamps
                .Include(t => t.AppUser)
                .Include(t => t.Jobsite);

            return await query.ToListAsync();
        }

        public async Task<ICollection<Timestamp>> GetTimestampsForUserByWorkDate(AppUser user,
            WorkHistoryParameters workHistoryParameters)
        {
            var query = _db.Timestamps
                .Include(t => t.AppUser)
                .Include(t => t.Jobsite)
                .Where(t => t.AppUser == user && 
                        t.ClockedInStamp >= workHistoryParameters.FromDate &&
                        t.ClockedInStamp <= workHistoryParameters.ToDate &&
                        t.ClockedIn == false)
                .OrderByDescending(t => t.ClockedInStamp);

            return await query.ToListAsync();
        }

        public async Task<PagedList<Timestamp>> GetTimestamps(
            TimestampParameters timestampParameters)
        {
            var query = _db.Timestamps
                .Include(t => t.AppUser)
                .Include(t => t.Jobsite)
                .Where(t => t.ClockedInStamp >= timestampParameters.FromDate && 
                        t.ClockedInStamp <= timestampParameters.ToDate &&
                        t.ClockedIn == false)
                .OrderByDescending(t => t.ClockedInStamp);

            return await PagedList<Timestamp>.ToPagedList(
                query, 
                timestampParameters.PageNumber, 
                timestampParameters.PageSize);
        }

        public async Task<ICollection<Timestamp>> GetTimestamps(WorkHistoryParameters workHistoryParameters)
        {
            var query = _db.Timestamps
                .Include(t => t.AppUser)
                .Include(t => t.Jobsite)
                .Where(t =>
                        t.ClockedInStamp >= workHistoryParameters.FromDate &&
                        t.ClockedInStamp <= workHistoryParameters.ToDate &&
                        t.ClockedIn == false)
                .OrderByDescending(t => t.ClockedInStamp);

            return await query.ToListAsync();
        }

        public async Task<Timestamp> GetClockedInTimestamp(AppUser user)
        {
            return await _db.Timestamps
                .Include(t => t.Jobsite)
                .SingleOrDefaultAsync(
                t => t.AppUserId == user.Id && t.ClockedIn == true);
        }
        
        public async Task<bool> JobsiteHasClockedInTimestamp(Jobsite jobsite)
        {
            return await _db.Timestamps
                .Include(t => t.Jobsite)
                .AnyAsync(
                t => t.Jobsite == jobsite && t.ClockedIn == true);
        }

        public async Task<PagedList<Timestamp>> TimestampsCurrentlyClockedInPaged(PageParameters pageParameters)
        {
            var query = _db.Timestamps
                .Where(t => t.ClockedIn == true)
                .Include(t => t.AppUser)
                .Include(t => t.Jobsite)
                .OrderBy(t => t.AppUser.DisplayName);

                return await PagedList<Timestamp>.ToPagedList(
                   query,
                   pageParameters.PageNumber,
                   pageParameters.PageSize);
        }

        public async Task<ICollection<Timestamp>> TimestampsCurrentlyClockedIn()
        {
            var query = _db.Timestamps
                .Where(t => t.ClockedIn == true)
                .Include(t => t.Jobsite)
                .OrderBy(t => t.Jobsite.Moniker);

            return await query.ToListAsync();
        }

        public async Task<PagedList<Timestamp>> GetTimestampsForJobByUser(
            AppUser user, string moniker, TimestampParameters timestampParameters)
        {
            var query = _db.Timestamps
                .Include(t => t.Jobsite)
                .Where(t =>
                t.AppUser == user &&
                t.Jobsite.Moniker == moniker &&
                t.ClockedInStamp >= timestampParameters.FromDate &&
                    t.ClockedInStamp <= timestampParameters.ToDate &&
                    t.ClockedIn == false)
                .OrderByDescending(t => t.ClockedInStamp);

            return await PagedList<Timestamp>.ToPagedList(
                query,
                timestampParameters.PageNumber,
                timestampParameters.PageSize); ;     
        }

        public async Task<PagedList<Timestamp>> GetTimestampsForUserByDate(AppUser user, TimestampParameters timestampParameters)
        {
            var query = _db.Timestamps
                .Include(t => t.Jobsite)
                .Where(t =>
                t.AppUser == user &&
                t.ClockedInStamp >= timestampParameters.FromDate &&
                    t.ClockedInStamp <= timestampParameters.ToDate &&
                    t.ClockedIn == false)
                .OrderByDescending(t => t.ClockedInStamp);

            return await PagedList<Timestamp>.ToPagedList(
                query,
                timestampParameters.PageNumber,
                timestampParameters.PageSize);
        }

        public async Task<PagedList<Timestamp>> GetTimestampsForJobByDate(Jobsite jobsite, TimestampParameters timestampParameters)
        {
            var query = _db.Timestamps
                .Include(t => t.AppUser)
                .Where(t => t.Jobsite == jobsite &&
                t.ClockedInStamp >= timestampParameters.FromDate &&
                t.ClockedInStamp <= timestampParameters.ToDate)
                .OrderByDescending(t => t.ClockedInStamp);

            return await PagedList<Timestamp>.ToPagedList(
                query,
                timestampParameters.PageNumber,
                timestampParameters.PageSize);
        }

        public async Task<ICollection<Timestamp>> GetTimestampsForJob(Jobsite jobsite)
        {
            return await _db.Timestamps
                .Include(t => t.AppUser)
                .Where(t => t.Jobsite == jobsite)
                .ToListAsync();
        }

        public async Task<Timestamp> GetUsersLastTimestamp(AppUser user)
        {
            var timestamp = _db.Timestamps
                .Include(t => t.Jobsite)
                .Where(t => t.AppUser == user)
                .OrderByDescending(t => t.ClockedInStamp);

            if (timestamp.Count() == 0 )
            {
                return null;
            }

            return await timestamp.FirstAsync();

        }

        public async Task<ICollection<Timestamp>> GetTimestampsUnpaged(TimestampParameters timestampParameters)
        {
            var query = _db.Timestamps
                .Include(t => t.Jobsite)
                .Where(t => t.ClockedInStamp >= timestampParameters.FromDate &&
                        t.ClockedInStamp <= timestampParameters.ToDate &&
                        t.ClockedIn == false)
                .OrderByDescending(t => t.ClockedInStamp);

            return await query.ToListAsync();
        }

        public async Task<bool> DeleteAllUserTimestamps(AppUser user)
        {
            var query = await _db.Timestamps
                            .Include(t => t.AppUser)
                            .Where(t => t.AppUser == user)
                            .ToListAsync();

             _db.RemoveRange(query);

            return (await _db.SaveChangesAsync()) > 0;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _db.SaveChangesAsync()) > 0;
        }
    }
}
