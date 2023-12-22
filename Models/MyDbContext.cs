// Importing necessary namespace for Entity Framework Core
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

// Namespace for the data models of the application
namespace AmazonCloneMVC.Models
{
    // Class representing the database context for the application
    public class MyDbContext : IdentityDbContext
    {
        // Constructor taking DbContextOptions as a parameter (used for dependency injection)
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        { }

        // Default constructor (possibly used for design-time tools)
        public MyDbContext()
        {

        }

        // DbSet representing the 'Produits' table in the database
        public DbSet<Produit> Produits { get; set; }

        // DbSet representing the 'Categories' table in the database
        public DbSet<Categorie> Categories { get; set; }
    }
}
