using Microsoft.EntityFrameworkCore;

namespace FTM.WebApi.Entities
{
    public class FtmDbContext : DbContext
    {
        public FtmDbContext()
        {

        }

        DbSet<RoomInfo> RoomInfos { get; set; }
        DbSet<CredentialInfo> CredentialInfos { get; set; }
    }
}
