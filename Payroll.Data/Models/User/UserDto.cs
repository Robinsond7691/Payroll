﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Payroll.Data.Models
{
    public class UserDto
    {
        public string DisplayName { get; set; }
        public string Token { get; set; }
        public string Username { get; set; }
        public bool CurrentlyClockedIn { get; set; }
        public TimestampClockedInBasicDto ClockedInTimestamp { get; set; }
        public TimestampWithBasicJobsiteInfoDto LastJobsiteVisited { get; set; }
        public bool Manager { get; set; }
        public bool Admin { get; set; }
    }
}
