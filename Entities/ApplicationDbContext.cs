using Microsoft.EntityFrameworkCore;

namespace Entities
{
  public class ApplicationDbContext : DbContext
  {
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {

    }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Person> Persons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Country>().ToTable("Countries");

      modelBuilder.Entity<Person>().ToTable("Persons");

      // Seed data
      string countriesJson = System.IO.File.ReadAllText("countries.json");
      List<Country> countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);

      foreach (Country country in countries)
      {
        modelBuilder.Entity<Country>().HasData(country);
      }

      string personsJson = System.IO.File.ReadAllText("persons.json");
      List<Person> persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsJson);

      foreach (Person person in persons)
      {
        modelBuilder.Entity<Person>().HasData(person);
      }

      //Fluent API
      modelBuilder.Entity<Person>().Property(temp => temp.TIN)
        .HasColumnName("TaxIdentificationNumber")
        .HasColumnType("varchar(8)")
        .HasDefaultValue("ABC12345");

      modelBuilder.Entity<Person>().HasIndex(temp => temp.TIN).IsUnique();

      modelBuilder.Entity<Person>().HasCheckConstraint("CHK_TIN", "len([TaxIdentificationNumber]) = 8");

    }

    public List<Person> sp_GetAllPersons()
    {
      return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
    }
  }
}
