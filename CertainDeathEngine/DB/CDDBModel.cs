namespace CertainDeathEngine.DB
{
    using CertainDeathEngine.Models;
    using CertainDeathEngine.Models.User;
    using CertainDeathEngine.WorldManager;
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class CDDBModel : DbContext
    {
        public CDDBModel()
            : base("name=CDDBModel")
        {
        }

        public virtual DbSet<Score> Scores { get; set; }
        public virtual DbSet<CertainDeathUser> Users { get; set; }
        //public virtual DbSet<GameWorldWrapperWrapper> Worlds { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //storeInDB(modelBuilder);
            //modelBuilder.ComplexType<GameWorld>().Ignore(p => p.World);

            //modelBuilder.ComplexType<GameWorldWrapper>()
            //    .Property(p => p.EFSerialized)
            //    .HasColumnName("GameWorldBinaryStuffs");
        }

        //private static object storeInDB(DbModelBuilder modelBuilder)
        //{
        //    //modelBuilder.Entity<CertainDeathUser>().HasRequired(p => p.FBUser).WithMany;
        //    //modelBuilder.Entity<CertainDeathUser>().HasRequired(r => r.Id);
        //    //    //.Add<CDDBModel>;
        //}
    }

}