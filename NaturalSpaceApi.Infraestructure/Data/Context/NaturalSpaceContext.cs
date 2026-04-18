using Microsoft.EntityFrameworkCore;
using NaturalSpaceApi.Domain.Entities;
using NaturalSpaceApi.Infrastructure.Configurations;

namespace NaturalSpaceApi.Infrastructure.Data.Context
{
    public class NaturalSpaceContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<WorkSpace> WorkSpaces { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<ChannelMember> ChannelMembers { get; set; }
        public DbSet<UserWorkSpace> UserWorkSpaces { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Domain.Entities.File> Files { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public NaturalSpaceContext(DbContextOptions<NaturalSpaceContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

          
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new WorkSpaceConfiguration());
            modelBuilder.ApplyConfiguration(new ChannelConfiguration());
            modelBuilder.ApplyConfiguration(new ChannelMemberConfiguration());
            modelBuilder.ApplyConfiguration(new UserWorkSpaceConfiguration());
            modelBuilder.ApplyConfiguration(new MessageConfiguration());
            modelBuilder.ApplyConfiguration(new FileConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        }
    }
}
