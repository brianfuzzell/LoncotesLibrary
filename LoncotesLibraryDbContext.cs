using Microsoft.EntityFrameworkCore;
using LoncotesLibrary.Models;

public class LoncotesLibraryDbContext : DbContext
{

    public DbSet<Checkout> Checkouts { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<MaterialType> MaterialTypes { get; set; }
    public DbSet<Patron> Patrons { get; set; }

    public LoncotesLibraryDbContext(DbContextOptions<LoncotesLibraryDbContext> context) : base(context)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Checkout>().HasData(new Checkout[] { });

        modelBuilder.Entity<Genre>().HasData(new Genre[]
        {
        new Genre {Id = 1, Name = "History"},
        new Genre {Id = 2, Name = "Biography"},
        new Genre {Id = 3, Name = "Memoir"},
        new Genre {Id = 4, Name = "Fantasy"},
        new Genre {Id = 5, Name = "Science Fiction"}
        });

        modelBuilder.Entity<MaterialType>().HasData(new MaterialType[]
        {
        new MaterialType {Id = 1, Name = "Book", CheckoutDays = 14},
        new MaterialType {Id = 2, Name = "Periodical", CheckoutDays = 7},
        new MaterialType {Id = 3, Name = "CD", CheckoutDays = 7},
        new MaterialType {Id = 4, Name = "DVD", CheckoutDays = 3}
        });

        modelBuilder.Entity<Material>().HasData(new Material[]
        {
        new Material {Id = 1, MaterialName = "The Fellowship of the Ring", MaterialTypeId = 1, GenreId = 4, OutOfCirculationSince = null},
        new Material {Id = 2, MaterialName = "The Two Towers", MaterialTypeId = 1, GenreId = 4, OutOfCirculationSince = null},
        new Material {Id = 3, MaterialName = "Abbey Road", MaterialTypeId = 3, GenreId = 5, OutOfCirculationSince = null},
        new Material {Id = 4, MaterialName = "The Beatles Anthology", MaterialTypeId = 2, GenreId = 2, OutOfCirculationSince = null},
        new Material {Id = 5, MaterialName = "Churchill: A Life", MaterialTypeId = 1, GenreId = 2, OutOfCirculationSince = null},
        new Material {Id = 6, MaterialName = "FDR", MaterialTypeId = 1, GenreId = 2, OutOfCirculationSince = new DateTime(2015, 12, 10)},
        new Material {Id = 7, MaterialName = "Our Band Could Be Your Life", MaterialTypeId = 1, GenreId = 3, OutOfCirculationSince = null},
        new Material {Id = 8, MaterialName = "The Return of the King", MaterialTypeId = 1, GenreId = 4, OutOfCirculationSince = null},
        new Material {Id = 9, MaterialName = "No Direction Home (DVD)", MaterialTypeId = 4, GenreId = 2, OutOfCirculationSince = null},
        new Material {Id = 10, MaterialName = "The Second World War", MaterialTypeId = 1, GenreId = 1, OutOfCirculationSince = null}
        });

        modelBuilder.Entity<Patron>().HasData(new Patron[]
        {
        new Patron {Id = 1, FirstName = "Homer", LastName = "Simpson", Address = "742 Evergreen Terrace", Email = "homer@springfield.net", IsActive = false},
        new Patron {Id = 2, FirstName = "Marge", LastName = "Simpson", Address = "742 Evergreen Terrace", Email = "marge@springfield.net", IsActive = true},
        new Patron {Id = 3, FirstName = "Bart", LastName = "Simpson", Address = "742 Evergreen Terrace", Email = "bart@springfield.net", IsActive = true}
        });
    }
}