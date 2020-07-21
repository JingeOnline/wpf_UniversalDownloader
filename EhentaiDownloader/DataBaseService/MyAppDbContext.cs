using EhentaiDownloader.Models;
using MySql.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
//using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhentaiDownloader.DataBaseService
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    class MyAppDbContext: DbContext
    {
        public MyAppDbContext() : base("MyAppDbContext")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<EbookPageModel> Blogs { get; set; }
    }
}
