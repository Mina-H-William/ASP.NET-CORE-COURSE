using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    /// <summary>
    /// Domain Model for country.
    /// </summary>
    public class Country
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)] // for generating ID automatically if not provided
        public Guid CountryID { get; set; }

        public string? CountryName { get; set; }

        //public virtual ICollection<Person>? Persons { get; set; }

    }
}
