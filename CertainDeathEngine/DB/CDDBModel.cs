using CertainDeathEngine.Models.User;
using CertainDeathEngine.WorldManager;
using log4net;
using System.Data.Entity;

namespace CertainDeathEngine.DB
{
    public class CDDBModel : DbContext
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // Set up the EF Tables
        public virtual DbSet<Score> Scores { get; set; }
        public virtual DbSet<CertainDeathUser> Users { get; set; }
        public virtual DbSet<GameWorldWrapperWrapper> Worlds { get; set; }
        public virtual DbSet<MyAppUser> FBUsers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Log.Debug("The CDDB Model is being created");

            // Set the Worlds not to deserialize with EF.
            modelBuilder.ComplexType<GameWorldWrapper>().Ignore(p => p.World);

            // Tell EF that the deserialization will happen in the EFSerialized property
            modelBuilder.ComplexType<GameWorldWrapper>()
                .Property(p => p.EfSerialized)
                .HasColumnName("GameWorldBinaryStuffs");

            // I did this because the score holds a dictionary of resources, and I want the total from all of them
            modelBuilder.Entity<Score>()
                .Property(p => p.TotalResources)
                .HasColumnName("TotalResources");
        }
    }
}