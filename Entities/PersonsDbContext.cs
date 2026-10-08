using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Pkcs;
using System.Text.Json;

namespace Entities
{
    public class PersonsDbContext: DbContext
    {
        public DbSet<Country> Countries { get; set; }
        public DbSet<Person> Persons { get; set; }
        public PersonsDbContext(DbContextOptions<PersonsDbContext> options):base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            string countriesJson= File.ReadAllText("countries.json");
            List<Country> countries= JsonSerializer.Deserialize<List<Country>>(countriesJson);
            string personsJson= File.ReadAllText("persons.json");
            List<Person> persons= JsonSerializer.Deserialize<List<Person>>(personsJson);
            modelBuilder.Entity<Country>().ToTable("Countries")
                .HasData(countries);
            modelBuilder.Entity<Person>().ToTable("Persons")
                .HasData(persons);

            //Fluent API 
            //modelBuilder.Entity<Person>()
            //    .HasIndex(p => p.PersonName)
            //    .IsUnique();

                

            modelBuilder.Entity<Person>()
               .Property(p => p.TIN)
               .HasColumnName("TaxIdentificationNumber")
               .HasColumnType("varchar(8)")
               .HasDefaultValue("ABC12345"); // the maximum number of character 

            modelBuilder.Entity<Person>().ToTable("Persons", table =>
            {
                table.HasCheckConstraint("CHK_TIN", "len([TaxIdentificationNumber])=8");
            });
            //Table Relations
            //modelBuilder.Entity<Person>(entity =>
            //{
            //    entity.HasOne<Country>(c => c.Country)
            //            .WithMany(p => p.Persons)
            //            .HasForeignKey(p => p.CountryID);
            //});
        }

        public List<Person> sp_GetAllPersons()
        {
           IQueryable<Person> persons= Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]");
            return persons.ToList();
            
        }
        public int sp_InsertPerson(Person person)
        {
            SqlParameter[] parameters = [
                new("@PersonID",person.PersonID),
                new("@PersonName",person.PersonName),
                new("@Email",person.Email),
                new("@Gender",person.Gender),
                new("@DateOfBirth",person.DateOfBirth),
                new("@Address",person.Address),
                new("@CountryID",person.CountryID),
                new("@ReceivenewsLetters",person.ReceiveNewsLetters)
                ];

           return  Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPerson] @PersonID,@PersonName, @Email, @Gender, @DateOfBirth , @Address, @CountryID, @ReceivenewsLetters", parameters);

        }

    }
}
