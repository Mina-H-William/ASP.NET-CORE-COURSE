using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    /// <summary>
    /// Person entity class.
    /// </summary>
    public class Person
    {
        [Key]
        public Guid PersonID { get; set; }

        [StringLength(40)] // nvarchar(40)
        //[Required]
        public string? PersonName { get; set; }

        [StringLength(40)] // nvarchar(40)
        public string? Email { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(10)] // nvarchar(10)
        public string? Gender { get; set; }

        [StringLength(200)] // nvarchar(200)
        public string? Address { get; set; }

        public bool ReciveNewsLetters { get; set; }

        public string? TIN { get; set; }

        public Guid? CountryID { get; set; }

        [ForeignKey("CountryID")]
        public virtual Country? Country { get; set; } // Navigation property to Country entity

        public override string ToString()
        {
            return @$"PersonID: {PersonID}, Person Name: {PersonName}, Email: {Email},
                     Date Of Birth: {DateOfBirth?.ToString("dddd MM yyyy")}, Gender: {Gender}, Address: {Address},
                     Receiving News Letters: {ReciveNewsLetters}, Country Name: {Country?.CountryName ?? "Not exist"}\n";
        }
    }
}
