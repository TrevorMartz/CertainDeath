using CertainDeathEngine.Models.User;
using CertainDeathEngine.WorldManager;
using System.Data.Entity;

namespace CertainDeathEngine.DB
{
    public class CDDBModel : DbContext
    {
        // Set up the EF Tables
        public virtual DbSet<Score> Scores { get; set; }
        public virtual DbSet<CertainDeathUser> Users { get; set; }
        public virtual DbSet<GameWorldWrapperWrapper> Worlds { get; set; }
        public virtual DbSet<MyAppUser> FBUsers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Set the Worlds not to deserialize with EF.
            modelBuilder.ComplexType<GameWorldWrapper>().Ignore(p => p.World);

            // Tell EF that the deserialization will happen in the EFSerialized property
            modelBuilder.ComplexType<GameWorldWrapper>()
                .Property(p => p.EFSerialized)
                .HasColumnName("GameWorldBinaryStuffs");
        }
    }
}