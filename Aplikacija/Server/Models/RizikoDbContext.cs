using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Server.Models
{
    public class RizikoDbContext : IdentityDbContext<User>
    {
        //connection string je u appsettinngs.json
       /* public DbSet<IdentityUserClaim<string>> IdentityUserClaim { get; set; }
        public DbSet<IdentityUserRole<string>> IdentityUserRole { get; set; }
        public DbSet<IdentityUserLogin<string>> IdentityUserLogin { get; set; }*/
        public RizikoDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /* modelBuilder.Entity<IdentityUserClaim<string>>().HasKey(p => new { p.Id });
             modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(p => new { p.UserId });
             modelBuilder.Entity<IdentityUserRole<string>>().HasKey(p => new { p.UserId, p.RoleId });*/
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Game>().HasMany(p => p.Players).WithMany(m => m.GamesPlayed);
            modelBuilder.Entity<Game>().HasOne(w => w.Winner).WithMany()
                             .HasForeignKey("WinnerId");
          
        }
        public DbSet<User> User { get; set; }
        public DbSet<Leaderboard> Leaderboard { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<Continent> Continent { get; set; }
        public DbSet<Map> Map { get; set; }
        public DbSet<Province> Province { get; set; }
        public DbSet<Game> Game { get; set; }
    }
}
