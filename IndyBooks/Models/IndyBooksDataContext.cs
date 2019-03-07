using System;
using Microsoft.EntityFrameworkCore;

namespace IndyBooks.Models
{
    public class IndyBooksDataContext:DbContext
    {
        public IndyBooksDataContext(DbContextOptions<IndyBooksDataContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        //Define DbSets for Collections representing DB tables
        public DbSet<Book> Books { get; set; }
        public DbSet<Writer> Writers { get; set; }

        // Used to fine tune certain aspects of the Data model
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Book>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");
        }

        //On Visual Studio for Mac ONLY: use the references
        //TODO: For Mac Users ONLY: follow the references to get Docker setup and uncomment line 23 below
        // - https://www.ciclosoftware.com/2018/03/14/sql-server-with-net-core-and-entityframework-on-mac/
        // - https://docs.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker?view=sql-server-2017
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer("Server=localhost,1433; Database=IndyBooks;User=SA; Password=Pa$$word!");
        }
    }
}
