using DotNetCraft.DevTools.SimpleQueue.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNetCraft.DevTools.SimpleQueue.Repositories
{
    public class SimpleQueueContext: DbContext
    {
        public DbSet<ServerInfo> Servers { get; set; }

        public SimpleQueueContext(DbContextOptions<SimpleQueueContext> options): base(options)
        {
            Database.EnsureCreated();
        }

        #region Required
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServerInfo>()
                .ToTable("ServerInformation");

            modelBuilder.Entity<ServerInfo>()
                .HasKey(b => b.Id)
                .HasName("PK_Id");

            modelBuilder.Entity<ServerInfo>()
                .Property(b => b.Name)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<ServerInfo>()
                .Property(b => b.Description)
                .HasMaxLength(200)
                .IsRequired(false);

            modelBuilder.Entity<ServerInfo>()
                .Property(b => b.ServerExpiredTime)
                .IsRequired();
        }
        #endregion
    }
}
