using System.ComponentModel.DataAnnotations;

namespace CitiesManager.Core.Entites
{
    public class City
    {
        [Key]
        public Guid CityID { get; set; }

        public string? CityName { get; set; }
    }
}
