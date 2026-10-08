using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class InsertPerson_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {  
            string sp_InsertPerson = @"
                    CREATE PROCEDURE [dbo].[InsertPerson] (@PersonID uniqueidentifier,
                                @PersonName nvarchar(40),
                                @Email nvarchar(40),
                                @Gender nvarchar(10),
                                @DateOfBirth datetime2(7),
                                @Address nvarchar(200),
                                @CountryID uniqueidentifier,
                                @ReceivenewsLetters bit)
                    AS BEGIN 
                            INSERT INTO [dbo].[Persons]([PersonID], [PersonName], [Email], [Gender], [DateOfBirth], [Address], [CountryID], [ReceivenewsLetters])
                            VALUES (@PersonID, @PersonName, @Email, @Gender, @DateOfBirth, @Address, @CountryID, @ReceivenewsLetters)
                    END
                    ";
            migrationBuilder.Sql(sp_InsertPerson);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS [dbo].[InsertPerson]");
        }
    }
}
