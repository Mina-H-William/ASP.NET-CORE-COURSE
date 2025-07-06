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
                CREATE PROCEDURE [dbo].[InsertPerson]
                (@PersonID uniqueidentifier, @PersonName nvarchar(40), 
                @Email nvarchar(40), @DateOfBirth datetime2(7), @Gender nvarchar(10),
                @CountryID uniqueidentifier, @Address nvarchar(200),
                @ReciveNewsLetters bit)
                AS BEGIN
                    INSERT INTO [dbo].[Persons] (PersonID,PersonName,Email,
                    DateOfBirth, Gender, CountryID, Address, ReciveNewsLetters) 
                    VALUES (@PersonID, @PersonName, @Email,@DateOfBirth,
                            @Gender, @CountryID, @Address, @ReciveNewsLetters)
                END
            ";
            migrationBuilder.Sql(sp_InsertPerson);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_InsertPerson = @"
                DROP PROCEDURE [dbo].[InserPerson]
            ";
            migrationBuilder.Sql(sp_InsertPerson);
        }
    }
}
