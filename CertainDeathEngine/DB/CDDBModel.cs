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
        //static private Initializer DbInitializer;

        //static CDDBModel()
        //{
        //    DbInitializer = new CDDBModel.Initializer();
        //    Database.SetInitializer<CDDBModel>(DbInitializer);
        //}
        //public class Initializer : IDatabaseInitializer<CDDBModel>
        //{
        //    public void InitializeDatabase(CDDBModel context)
        //    {
        //        string ddl = "CREATE TABLE TEST (c1 int);";
        //        context.Database.ExecuteSqlCommand(ddl);
        //    }
        //}

        public virtual DbSet<Score> Scores { get; set; }
        public virtual DbSet<CertainDeathUser> Users { get; set; }
        public virtual DbSet<GameWorldWrapperWrapper> Worlds { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Score>().ToTable("Scores");
            //modelBuilder.Entity<CertainDeathUser>().ToTable("Users");
            //modelBuilder.Entity<MyAppUser>().ToTable("FBUsers");
            //modelBuilder.Entity<GameWorldWrapperWrapper>().ToTable("Worlds");  

            //storeInDB(modelBuilder);
            modelBuilder.ComplexType<GameWorldWrapper>().Ignore(p => p.World);

            modelBuilder.ComplexType<GameWorldWrapper>()
                .Property(p => p.EFSerialized)
                .HasColumnName("GameWorldBinaryStuffs");
        }

        //private static object storeInDB(DbModelBuilder modelBuilder)
        //{
        //    //modelBuilder.Entity<CertainDeathUser>().HasRequired(p => p.FBUser).WithMany;
        //    //modelBuilder.Entity<CertainDeathUser>().HasRequired(r => r.Id);
        //    //    //.Add<CDDBModel>;
        //}
    }

}