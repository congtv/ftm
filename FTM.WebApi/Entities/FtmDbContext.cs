using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FTM.WebApi.Entities
{
    public class FtmDbContext : DbContext
    {
        private readonly string connectionString;
        public FtmDbContext(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public FtmDbContext(DbContextOptions<FtmDbContext> options) : base(options)
        {

        }
        public DbSet<FtmCalendarInfo> RoomInfos { get; set; }
        public DbSet<FtmTokenResponse> FtmTokenResponses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(connectionString != null)
            {
                optionsBuilder.UseSqlite(connectionString);
            }
            base.OnConfiguring(optionsBuilder);
        }
    }
}
