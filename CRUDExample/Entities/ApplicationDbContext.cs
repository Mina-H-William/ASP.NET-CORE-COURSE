using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json;

namespace Entities
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        { }

        public virtual DbSet<Country> Countries { get; set; }

        public virtual DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // by default entity framework use name of dbset as name of table 
            modelBuilder.Entity<Country>().ToTable("Countries"); // if i want to use a different table name than the property name
            modelBuilder.Entity<Person>().ToTable("Persons");

            // // this how can you add seed hard coded data to the database 
            //modelBuilder.Entity<Country>()
            //    .HasData(new List<Country>(){
            //        new Country() { CountryID = Guid.NewGuid(), CountryName = "Sample" },
            //        new Country() { CountryID = Guid.NewGuid(), CountryName = "Sample2" },
            //        });

            // or you can add seed in json file and use this code to load it
            string countriesJson = File.ReadAllText("countries.json");
            List<Country> Countries = JsonSerializer.Deserialize<List<Country>>(countriesJson) ?? new List<Country>();
            foreach (var country in Countries)
            {
                modelBuilder.Entity<Country>().HasData(country);
            }

            string personsJson = File.ReadAllText("persons.json");
            List<Person> persons = JsonSerializer.Deserialize<List<Person>>(personsJson) ?? new List<Person>();
            modelBuilder.Entity<Person>().HasData(persons);

            // Fluent API

            // this will generate a new guid for the country id if not provided
            //modelBuilder.Entity<Country>()
            //    .Property(c => c.CountryID)
            //    .HasDefaultValueSql("NEWID()"); // global unique identifier

            //modelBuilder.Entity<Country>()
            //    .Property(c => c.CountryID)
            //    .HasDefaultValueSql("NEWSEQUENTIALID()"); // best for index performance and unique per computer

            modelBuilder.Entity<Person>()
                .Property(p => p.TIN)
                .HasColumnName("TaxIdentificationNumber")
                .HasColumnType("varchar(8)")
                .HasDefaultValue("ABC12345");

            // to add a unique index to the TIN column
            //modelBuilder.Entity<Person>().HasIndex(c => c.TIN).IsUnique();

            // old way to add checkconstraint
            //modelBuilder.Entity<Person>().HasCheckConstraint("CHK_TIN", "len([TaxIdentificationNumber]) = 8");

            // new way to add checkconstraint
            modelBuilder.Entity<Person>()
                        .ToTable(tb => tb.HasCheckConstraint(
                            "CHK_TIN",
                            "LEN([TaxIdentificationNumber]) = 8"));

            // manually Table Relationships
            // with many can be empty () if other model (country) has no collection of this model (Person)
            // this setup not required as its enough to add navigation property in the model class and use include
            // method in the query
            //modelBuilder.Entity<Person>(entity =>
            //{
            //    entity.HasOne(p => p.Country) // Person has one Country
            //          .WithMany(c => c.Persons) // Country has many Persons
            //          .HasForeignKey(p => p.CountryID); // Foreign
            //});
        }

        public List<Person> sp_GetAllPersons()
        {
            // sotred procedure for retreiving data from the database (at least one select statement) use 
            // [dbset].FromSqlRaw(query as string, params[] of SqlParameter) return IQueryable<T> 

            return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        }

        public int sp_InsertPerson(Person person)
        {
            // sotred procedure for (Insert, Update, Delete) in the database use 
            // database.ExecuteSqlRaw(query as string, params[] of SqlParameter) return int (number of affected rows) 

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@PersonID", person.PersonID),
                new SqlParameter("@PersonName", person.PersonName),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("@DateOfBirth", person.DateOfBirth),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@CountryID", person.CountryID),
                new SqlParameter("@Address", person.Address),
                new SqlParameter("@ReciveNewsLetters", person.ReciveNewsLetters),
            };

            return Database.ExecuteSqlRaw(@"EXECUTE [dbo].[InsertPerson] @PersonID, 
                                            @PersonName, @Email, @DateOfBirth, @Gender,
                                            @CountryID, @Address, @ReciveNewsLetters", parameters);
        }


    }
}
