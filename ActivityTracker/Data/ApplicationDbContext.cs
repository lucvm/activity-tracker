﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ActivityTracker2.Models;

namespace ActivityTracker2.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserGroup>().HasKey(ug => new { ug.ApplicationUserID, ug.GroupID });
            builder.Entity<ApplicationUser>().ToTable("User");
            builder.Entity<Group>().ToTable("Group");
            builder.Entity<Activity>().ToTable("Activity");
            builder.Entity<LogEntry>().ToTable("LogEntry");
        }
    }
}