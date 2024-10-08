﻿using API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace API.Data
{
    public class Context: IdentityDbContext<AppUser>
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
            
        }

        public DbSet<UserLike> Likes => Set<UserLike>();
        public DbSet<Messages> Messages => Set<Messages>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserLike>()
                .HasKey(k => new { k.SourceUserId, k.TargetUserId });

            builder.Entity<UserLike>()
                .HasOne(s => s.SourceUser)
                .WithMany(l => l.LikedUsers)
                .HasForeignKey(l => l.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserLike>()
             .HasOne(s => s.TargetUser)
             .WithMany(l => l.LikedByUsers)
             .HasForeignKey(l => l.TargetUserId)
             .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Messages>()
               .HasOne(u => u.Recipient)
               .WithMany(m => m.MessagesReceived)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Messages>()
                .HasOne(u => u.Sender)
                .WithMany(m => m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
