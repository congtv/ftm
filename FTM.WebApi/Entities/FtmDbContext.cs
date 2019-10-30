using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FTM.WebApi.Entities
{
    public class FtmDbContext : DbContext
    {
        public FtmDbContext(DbContextOptions<FtmDbContext> options) : base(options)
        {

        }
        public DbSet<RoomInfo> RoomInfos { get; set; }
        public DbSet<CredentialInfo> CredentialInfos { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlite("Filename=TestDatabase.db", options =>
        //    {
        //        options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
        //    });

        //    base.OnConfiguring(optionsBuilder);
        //}

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlite(ConnectionString);
        //    }
        //}
    }
}
