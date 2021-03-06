﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using BugetApp.Models.Classes;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BugetApp.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Transaction> Transactions { get; set; }

        [InverseProperty("Creator")]
        public virtual ICollection<HouseHolds> CreatorUser { get; set; }

        [InverseProperty("JoinedUsers")]
        public virtual ICollection<HouseHolds> JoinedUsers { get; set; }

        public virtual ICollection<HouseHoldInvites> InvitedUser { get; set; }
        public ApplicationUser()
        {
            CreatorUser = new HashSet<HouseHolds>();
            JoinedUsers = new HashSet<HouseHolds>();
            InvitedUser = new HashSet<HouseHoldInvites>();
            Transactions = new HashSet<Transaction>();
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Transaction>().HasRequired(p => p.Category).WithMany(r => r.Transactions).WillCascadeOnDelete(false);
        }

        public DbSet<HouseHolds> HouseHolds { get; set; }

        public DbSet<HouseHoldInvites> HouseHoldInvites { get; set; } 

        public DbSet<Category> Categories { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Transaction> Transactions { get; set; }
    }
}