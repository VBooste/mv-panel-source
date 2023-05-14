using PanelOS.Models;
using System.Data.Entity;

namespace PanelOS.Helpers
{
    public class LauncherDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<CalibrationLobby> CalibrationLobbies { get; set; }
        public DbSet<BoostingLobby> BoostingLobbies { get; set; }

        public LauncherDbContext() : base("DefaultConnection") 
        {
            Database.SetInitializer<LauncherDbContext>(null);
        }        
    }
}