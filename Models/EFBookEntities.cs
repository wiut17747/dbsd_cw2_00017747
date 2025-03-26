using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace dbsd_cw2_00017747.Models {
    public class EFBookEntities : DbContext {
        public EFBookEntities() : base("name=DefaultConnection") { }

        static EFBookEntities() {
            Database.SetInitializer<EFBookEntities>(null);
        }
        public DbSet<EFBook> EFBooks {
            get;
            set;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {

            modelBuilder.Entity<EFBook>().ToTable("EFBooks");

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}